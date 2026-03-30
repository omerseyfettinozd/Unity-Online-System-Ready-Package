using System.Collections;
using System.IO;
using Epic.OnlineServices;
using Epic.OnlineServices.Connect;
using UnityEngine;

namespace OnlineSystemReady.Core
{
    /// <summary>
    /// ParrelSync clone'larında farklı bir EOS ProductUserId oluşturulmasını sağlar.
    /// 
    /// Sorun: ParrelSync clone'ları ana proje ile aynı makinede çalıştığı için
    /// aynı EOS DeviceId'yi kullanır ve aynı ProductUserId'yi alır.
    /// EOS P2P aynı kullanıcının kendisine bağlanmasını desteklemediğinden,
    /// clone'dan host'a bağlantı başarısız olur.
    /// 
    /// Çözüm: Clone'da eski DeviceId silinip yeni bir tane oluşturulur,
    /// böylece farklı bir ProductUserId elde edilir.
    /// 
    /// Bu script, NetworkManager objesinin üzerine (veya herhangi bir oyun başlangıç objesine)
    /// eklenmeli ve Awake'te diğer EOS bileşenlerinden ÖNCE çalışmalıdır.
    /// Script Execution Order: -200 (EOSManager'dan önce)
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class ParrelSyncEOSHelper : MonoBehaviour
    {
        /// <summary>
        /// Clone olup olmadığımızı runtime'da tespit eder.
        /// ParrelSync.ClonesManager Editor-only olduğu için,
        /// proje kök dizininde ".clone" dosyasının varlığını kontrol ederiz.
        /// </summary>
        public static bool IsClone()
        {
            string projectPath = Application.dataPath.Replace("/Assets", "");
            string cloneFilePath = Path.Combine(projectPath, ".clone");
            return File.Exists(cloneFilePath);
        }

        /// <summary>
        /// Sadece bir kez çalışmasını sağlamak için global bayrak.
        /// </summary>
        public static bool hasResetDeviceIdThisSession = false;

        /// <summary>
        /// Mevcut DeviceId'yi siler. Silme işlemi tamamlandıktan sonra,
        /// FishyEOS otomatik olarak yeni bir DeviceId oluşturacak ve
        /// farklı bir ProductUserId elde edecektir.
        /// </summary>
        public static IEnumerator DeleteAndRecreateDeviceId()
        {
            if (hasResetDeviceIdThisSession) yield break;
            hasResetDeviceIdThisSession = true;

            var connectInterface = PlayEveryWare.EpicOnlineServices.EOSManager.Instance
                .GetEOSConnectInterface();
            
            if (connectInterface == null)
            {
                Debug.LogWarning("[ParrelSyncEOS] EOS Connect Interface bulunamadı. DeviceId silinemedi.");
                yield break;
            }

            bool deleteCompleted = false;
            Result deleteResult = Result.UnexpectedError;

            var deleteOptions = new DeleteDeviceIdOptions();
            connectInterface.DeleteDeviceId(ref deleteOptions, null, (ref DeleteDeviceIdCallbackInfo data) =>
            {
                deleteResult = data.ResultCode;
                deleteCompleted = true;
            });

            // Silme işleminin tamamlanmasını bekle
            while (!deleteCompleted)
                yield return null;

            if (deleteResult == Result.Success || deleteResult == Result.NotFound)
            {
                Debug.Log($"[ParrelSyncEOS] Clone'da eski DeviceId silindi. Sonuç: {deleteResult}");
                
                // Yeni DeviceId oluştur
                bool createCompleted = false;
                Result createResult = Result.UnexpectedError;
                
                var createOptions = new CreateDeviceIdOptions
                {
                    DeviceModel = $"Clone_{SystemInfo.deviceName}_{System.Diagnostics.Process.GetCurrentProcess().Id}"
                };
                connectInterface.CreateDeviceId(ref createOptions, null, (ref CreateDeviceIdCallbackInfo data) =>
                {
                    createResult = data.ResultCode;
                    createCompleted = true;
                });

                while (!createCompleted)
                    yield return null;

                if (createResult == Result.Success)
                {
                    Debug.Log("[ParrelSyncEOS] Clone için yeni DeviceId oluşturuldu! Farklı bir ProductUserId alınacak.");
                }
                else
                {
                    Debug.LogError($"[ParrelSyncEOS] Yeni DeviceId oluşturulamadı: {createResult}");
                }
            }
            else
            {
                Debug.LogError($"[ParrelSyncEOS] DeviceId silinemedi: {deleteResult}");
            }
        }
    }
}
