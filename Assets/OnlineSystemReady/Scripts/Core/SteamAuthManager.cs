using System;
using System.Collections;
using System.Text;
using UnityEngine;
using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices;

// Eğer Steamworks.NET yüklü değilse hata vermesin diye preprocessor directive (isteğe bağlı ama güvenli)
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

namespace OnlineSystemReady.Core
{
    /// <summary>
    /// Steam hesabıyla login olup alınan Ticket'ı yapılandırarak EOS Connect Interface'e (PlayEveryWare) köprüler.
    /// Eğer projede Steam açık değilse veya inisiyalize olamazsa sessizce bypass eder, o zaman normal DeviceToken auth'a düşülür.
    /// </summary>
    public class SteamAuthManager : MonoBehaviour
    {
        public static SteamAuthManager Instance;

        public bool IsSteamAvailable { get; private set; }
        public string SteamDisplayName { get; private set; } = "";

#if !DISABLESTEAMWORKS
        // Steam Ticket Callback'i
        private Callback<GetAuthSessionTicketResponse_t> _authSessionTicketResponseCallback;
        private Callback<GetTicketForWebApiResponse_t> _getTicketForWebApiResponseCallback;
        
        private string _hexSteamToken = string.Empty;
        private bool _isWaitingForToken = false;
        private bool _hasWebTicketResult = false;
#endif

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            TryInitSteam();
        }

        private void TryInitSteam()
        {
            // 🚨 BAŞLATICI ALGILAMA (LAUNCHER DETECTION) 🚨
            // Eğer oyun Epic Games Launcher üzerinden açıldıysa, arkada açık olan Steam'in devreye girmesini (çakışmasını) engelle.
            if (DidLaunchFromEpic())
            {
                IsSteamAvailable = false;
                Debug.Log("<color=magenta>[LauncherDetection] Oyun Epic Games Launcher'dan başlatıldı! Steam entegrasyonu tamamen devre dışı bırakılıyor.</color>");
                return;
            }

#if !DISABLESTEAMWORKS
            try
            {
                // Steam çalışıyorsa ve api uyumluysa Initialize et.
                // Not: Projede 'steam_appid.txt' yoksa veya Steam Client açık değilse burası false döner veya exception fırlatır.
                IsSteamAvailable = SteamAPI.Init();

                if (IsSteamAvailable)
                {
                    SteamDisplayName = SteamFriends.GetPersonaName();
                    Debug.Log($"[Steam] API Başarıyla Başlatıldı. Hoşgeldin {SteamDisplayName} (App ID: {SteamUtils.GetAppID()})");
                    
                    // Web API Ticket'ı bekleyen callback ataması
                    _getTicketForWebApiResponseCallback = Callback<GetTicketForWebApiResponse_t>.Create(OnGetTicketForWebApiResponse);
                }
                else
                {
                    Debug.LogWarning("[Steam] API Başlatılamadı (Steam kapalı ya da appid.txt yok). DeviceToken kullanılacak.");
                }
            }
            catch (Exception ex)
            {
                IsSteamAvailable = false;
                Debug.LogWarning("[Steam] API başlatma hatası: " + ex.Message + ". DeviceToken kullanılacak.");
            }
#else
            IsSteamAvailable = false;
            Debug.Log("[Steam] DISABLESTEAMWORKS aktif. Steam bypass ediliyor.");
#endif
        }

        private void OnDestroy()
        {
#if !DISABLESTEAMWORKS
            if (IsSteamAvailable)
            {
                SteamAPI.Shutdown();
            }
#endif
        }

#if !DISABLESTEAMWORKS
        private void Update()
        {
            if (IsSteamAvailable)
            {
                // Steam'den gelen callbackleri periyodik olarak okur
                SteamAPI.RunCallbacks(); 
            }
        }
#endif

        /// <summary>
        /// Steam Session Ticket alır ve bunu kullanarak EOS Connect'e dış auth talebinde bulunur.
        /// </summary>
        /// <param name="onSuccess">EOS Kimliği (ProductUserId) döndürür</param>
        /// <param name="onFail">Hata olduğunda çalışır ve hata metni döndürür</param>
        public IEnumerator LoginViaEOS(Action<ProductUserId> onSuccess, Action<string> onFail)
        {
            if (!IsSteamAvailable)
            {
                onFail?.Invoke("Steam API aktif değil.");
                yield break;
            }

#if !DISABLESTEAMWORKS
            Debug.Log("[SteamAuth] Steam Web API Token isteniyor...");
            
            // Sıfırlama
            _hexSteamToken = string.Empty;
            _isWaitingForToken = true;
            _hasWebTicketResult = false;

            // Epic Games tarafından beklenen spesifik Identifier:
            // (Steam Identity Provider ayarlarında ne tanımladıysanız aynısı olmalı. Genelde "epiconlineservices" olur)
            string identityToken = "epiconlineservices";

            // WebAPI ticket talebi yap (Asenkrondur)
            HAuthTicket hAuthTicket = SteamUser.GetAuthTicketForWebApi(identityToken);

            if (hAuthTicket == HAuthTicket.Invalid)
            {
                onFail?.Invoke("Steam Auth Ticket alınamadı (Invalid Handle).");
                yield break;
            }

            // Callback gelene kadar bekle
            float waitTimeout = 10f; // 10 saniye timeout (Steam yanıt vermezse donmamak için)
            float timer = 0;
            
            while (_isWaitingForToken && timer < waitTimeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (!_hasWebTicketResult || string.IsNullOrEmpty(_hexSteamToken))
            {
                onFail?.Invoke("Steam Web API Ticket Timeout veya hatalı sonuç!");
                yield break;
            }

            // Elimizde token HEX string'i var. Artık PlayEveryWare üzerinden EOS Connect'e girebiliriz.
            Debug.Log("[SteamAuth] Hex Token alındı, EOS Connect Interface'e gönderiliyor...");
            
            bool loginDone = false;
            ProductUserId resultingUserId = null;
            string loginError = string.Empty;

            var connectInterface = EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
            
            var credentials = new Epic.OnlineServices.Connect.Credentials
            {
                // PlayEveryWare pluginindeki Epic.OnlineServices namespace'i kullanılır.
                Type = ExternalCredentialType.SteamSessionTicket, // Veya SteamAppTicket (Eski)
                Token = _hexSteamToken
            };

            var loginOptions = new Epic.OnlineServices.Connect.LoginOptions
            {
                Credentials = credentials,
                UserLoginInfo = null
            };

            connectInterface.Login(ref loginOptions, null, (ref Epic.OnlineServices.Connect.LoginCallbackInfo info) =>
            {
                if (info.ResultCode == Result.Success)
                {
                    resultingUserId = info.LocalUserId;
                    loginDone = true;
                }
                else if (info.ResultCode == Result.InvalidUser || info.ResultCode == Result.NotFound)
                {
                    // Steam hesabı daha önce EOS ile eşleşmediyse (Headless account oluştur)
                    Debug.Log("[SteamAuth] Kullanıcı EOS'ta bulunamadı (InvalidUser), yeni Connect kullanıcısı oluşturuluyor...");
                    
                    var createOptions = new Epic.OnlineServices.Connect.CreateUserOptions
                    {
                        ContinuanceToken = info.ContinuanceToken
                    };

                    connectInterface.CreateUser(ref createOptions, null, (ref Epic.OnlineServices.Connect.CreateUserCallbackInfo createInfo) =>
                    {
                        if (createInfo.ResultCode == Result.Success)
                        {
                            resultingUserId = createInfo.LocalUserId;
                            loginDone = true;
                        }
                        else
                        {
                            loginError = "EOS CreateUser başarısız: " + createInfo.ResultCode;
                            loginDone = true;
                        }
                    });
                }
                else
                {
                    loginError = "EOS Connect Login başarısız: " + info.ResultCode;
                    loginDone = true;
                }
            });

            // EOS Yanıtını Bekle
            while (!loginDone)
            {
                yield return null;
            }

            if (resultingUserId != null && resultingUserId.IsValid())
            {
                Debug.Log("<color=green>[SteamAuth] Başarılı! Steam EOS'a bağlandı. ID: " + resultingUserId + "</color>");
                onSuccess?.Invoke(resultingUserId);
            }
            else
            {
                onFail?.Invoke(loginError);
            }

#else
            yield return null;
#endif
        }

#if !DISABLESTEAMWORKS
        private void OnGetTicketForWebApiResponse(GetTicketForWebApiResponse_t pCallback)
        {
            if (pCallback.m_eResult == EResult.k_EResultOK)
            {
                Debug.Log("[SteamAuth] GetTicketForWebApiResponse SUCCESS.");
                
                // Gelen byte dizisini hex string'e çeviriyoruz çünkü EOS token'ı HEX String olarak bekler.
                _hexSteamToken = ByteArrayToHexString(pCallback.m_rgubTicket);
                _hasWebTicketResult = true;
            }
            else
            {
                Debug.LogWarning("[SteamAuth] Steam Web API Ticket alma hatası. Sonuç: " + pCallback.m_eResult);
                _hasWebTicketResult = false;
            }
            _isWaitingForToken = false;
        }

        private string ByteArrayToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return string.Empty;

            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
#endif

        // --- PLATFORM ÇAKIŞMASI KORUMASI ---
        private bool DidLaunchFromEpic()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                // Epic Games Launcher oyunu başlatırken kesinlikle bu parametreleri gönderir.
                if (arg.Contains("-epicapp") || arg.Contains("-AUTH_TYPE=exchange_code"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
