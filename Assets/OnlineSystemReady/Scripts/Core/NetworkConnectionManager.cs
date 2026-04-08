using System.Collections;
using UnityEngine;
using FishNet;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using System.Reflection;

namespace OnlineSystemReady.Core
{
    /// <summary>
    /// Handles switching between LAN (Tugboat), Online (Epic Online Services), and Split-Screen modes.
    /// </summary>
    public class NetworkConnectionManager : MonoBehaviour
    {
        public static NetworkConnectionManager Instance;

        [Header("Transports")]
        [Tooltip("Drag the Tugboat component from NetworkManager here.")]
        public Tugboat lanTransport;
        
        [Tooltip("Drag the FishyEOS component from NetworkManager here once installed.")]
        public Transport eosTransport;

        [Header("Managers")]
        public SplitScreenManager splitScreenManager;



        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // Inspector'da elle bağlanmadıysa, sahnedeki bileşenleri otomatik bul
            if (lanTransport == null)
            {
                lanTransport = UnityEngine.Object.FindFirstObjectByType<Tugboat>();
                if (lanTransport == null)
                {
                    // Tugboat da yoksa oluştur
                    lanTransport = gameObject.AddComponent<Tugboat>();
                }
                Debug.Log("[Network] Tugboat (LAN) transport otomatik bulundu/oluşturuldu.");
            }

            if (eosTransport == null)
            {
                // Önce sahnede ara
                foreach (var transport in UnityEngine.Object.FindObjectsByType<Transport>(FindObjectsSortMode.None))
                {
                    if (transport.GetType().Name.Contains("FishyEOS"))
                    {
                        eosTransport = transport;
                        Debug.Log("[Network] FishyEOS (Online) transport sahnede bulundu.");
                        break;
                    }
                }

                // Sahnede yoksa, Reflection ile zorla oluştur
                if (eosTransport == null)
                {
                    System.Type fishyEOSType = null;
                    foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                    {
                        fishyEOSType = asm.GetType("FishNet.Transporting.FishyEOSPlugin.FishyEOS");
                        if (fishyEOSType != null) break;
                    }

                    if (fishyEOSType != null)
                    {
                        var comp = gameObject.AddComponent(fishyEOSType);
                        eosTransport = comp as Transport;
                        Debug.Log("[Network] FishyEOS (Online) transport dinamik olarak oluşturuldu!");
                    }
                    else
                    {
                        Debug.LogWarning("[Network] FishyEOS paketi projede bulunamadı. Online mod çalışmayacak.");
                    }
                }
            }

            // SteamAuthManager sahneye otomatik eklensin (Mümkünse)
            if (SteamAuthManager.Instance == null)
            {
                var steamAuthObj = UnityEngine.Object.FindFirstObjectByType<SteamAuthManager>();
                if (steamAuthObj == null)
                {
                    gameObject.AddComponent<SteamAuthManager>();
                    Debug.Log("[Network] SteamAuthManager bileşeni dinamik olarak oluşturuldu!");
                }
            }
        }

        private void Start()
        {
            if (InstanceFinder.ClientManager != null)
                InstanceFinder.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
            if (InstanceFinder.ServerManager != null)
                InstanceFinder.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
        }

        private void OnDestroy()
        {
            if (InstanceFinder.ClientManager != null)
                InstanceFinder.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
            if (InstanceFinder.ServerManager != null)
                InstanceFinder.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
        }

        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                Debug.Log("<color=orange>[Network] İstemci (Client) bağlantısı Stopped durumuna geçti. Odayı terk ediyoruz...</color>");
                if (EOSMatchmakingManager.Instance != null && InstanceFinder.TransportManager.Transport == eosTransport)
                {
                    EOSMatchmakingManager.Instance.LeaveOrDestroyLobby();
                }
            }
        }

        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                Debug.Log("<color=red>[Network] Sunucu (Server) kapatıldı. Kurduğumuz odayı yıkıyoruz...</color>");
                if (EOSMatchmakingManager.Instance != null && InstanceFinder.TransportManager.Transport == eosTransport)
                {
                    EOSMatchmakingManager.Instance.LeaveOrDestroyLobby();
                }
            }
        }

        public void StartLANHost()
        {
            Debug.Log("[Network] Starting LAN Host on Tugboat...");
            SetTransport(lanTransport);
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        }

        public void StartLANClient(string ipAddress = "localhost")
        {
            Debug.Log($"[Network] Connecting to LAN at {ipAddress}...");
            SetTransport(lanTransport);
            lanTransport.SetClientAddress(ipAddress);
            InstanceFinder.ClientManager.StartConnection();
        }

        public void StartOnlineHost()
        {
            if (eosTransport == null)
            {
                Debug.LogError("[Network] EOS Transport is missing! Please assign it in the inspector.");
                return;
            }
            StartCoroutine(StartOnlineHostCoroutine());
        }

        private IEnumerator StartOnlineHostCoroutine()
        {
            // ParrelSync clone'da DeviceId sıfırla (farklı ProductUserId almak için)
            yield return EnsureUniqueDeviceId();

            Debug.Log("[Network] Starting Online Server via Epic Online Services...");
            SetTransport(eosTransport);
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
        }

        public void StartOnlineClient()
        {
            if (eosTransport == null)
            {
                Debug.LogError("[Network] EOS Transport is missing! Please assign it in the inspector.");
                return;
            }
            StartCoroutine(StartOnlineClientCoroutine());
        }

        private IEnumerator StartOnlineClientCoroutine()
        {
            // ParrelSync clone'da DeviceId sıfırla (farklı ProductUserId almak için)
            yield return EnsureUniqueDeviceId();

            Debug.Log("[Network] Searching for Online EOS Host...");
            SetTransport(eosTransport);
            InstanceFinder.ClientManager.StartConnection();
        }

        /// <summary>
        /// ParrelSync clone'da çalışıyorsak, eski DeviceId'yi silip yenisini oluşturur.
        /// Bu sayede her clone farklı bir ProductUserId alır ve EOS P2P düzgün çalışır.
        /// Sadece bir kez çalışır (session başına).
        /// </summary>
        private IEnumerator EnsureUniqueDeviceId()
        {
            if (ParrelSyncEOSHelper.IsClone())
            {
                Debug.Log("[Network] ParrelSync clone tespit edildi, DeviceId sıfırlanıyor...");
                yield return ParrelSyncEOSHelper.DeleteAndRecreateDeviceId();
            }
        }

        public void StartSplitScreen()
        {
            Debug.Log("[Network] Initializing Local Split-Screen...");
            SetTransport(lanTransport);
            
            // Start a local server without broadcasting to the internet
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();

            if (splitScreenManager != null)
            {
                splitScreenManager.EnableSplitScreen();
            }
        }

        /// <summary>
        /// Runtime'da transport değiştirirken FishNet'in tüm event pipeline'ını
        /// eski transport'tan çıkarıp yeni transport'a bağlar.
        /// 
        /// FishNet V4'te ServerManager ve ClientManager, transport seviyesindeki
        /// event'lere (OnServerReceivedData, OnRemoteConnectionState, OnClientReceivedData vb.)
        /// abone olur. Bu abonelikler doğrudan Transport nesnesine bağlıdır.
        /// Transport değiştirildiğinde eski abonelikler kaldırılıp yenileri eklenmezse
        /// yeni transport'un event'leri işlenmez ve karakter spawn olmaz.
        /// </summary>
        private void SetTransport(Transport newTransport)
        {
            if (newTransport == null || InstanceFinder.TransportManager == null) return;

            var tm = InstanceFinder.TransportManager;
            var nm = InstanceFinder.NetworkManager;

            // Zaten aynı transport aktifse bir şey yapma
            if (tm.Transport == newTransport) return;

            // 1) ServerManager: Eski transport'tan event aboneliklerini kaldır
            var serverSubMethod = nm.ServerManager.GetType().GetMethod(
                "SubscribeToTransport",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (serverSubMethod != null)
            {
                serverSubMethod.Invoke(nm.ServerManager, new object[] { false }); // Unsubscribe
            }

            // 2) ClientManager: Eski transport'tan event aboneliklerini kaldır
            var clientSubMethod = nm.ClientManager.GetType().GetMethod(
                "SubscribeToEvents",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (clientSubMethod != null)
            {
                clientSubMethod.Invoke(nm.ClientManager, new object[] { false }); // Unsubscribe
            }

            // 3) Transport referansını değiştir
            tm.Transport = newTransport;

            // 4) Yeni transport'u Initialize et (peer'larını hazırlar)
            newTransport.Initialize(nm, 0);

            // 5) ServerManager: Yeni transport'a event aboneliklerini ekle
            if (serverSubMethod != null)
            {
                serverSubMethod.Invoke(nm.ServerManager, new object[] { true }); // Subscribe
            }

            // 6) ClientManager: Yeni transport'a event aboneliklerini ekle
            if (clientSubMethod != null)
            {
                clientSubMethod.Invoke(nm.ClientManager, new object[] { true }); // Subscribe
            }

            Debug.Log("[Network] Transport değiştirildi: " + newTransport.GetType().Name);
        }
    }
}

