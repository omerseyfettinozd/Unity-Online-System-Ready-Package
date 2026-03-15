using UnityEngine;
using FishNet;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;

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
            Debug.Log("[Network] Searching for Online EOS Host...");
            SetTransport(eosTransport);
            InstanceFinder.ClientManager.StartConnection();
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

        private void SetTransport(Transport newTransport)
        {
            if (newTransport != null && InstanceFinder.TransportManager != null)
            {
                InstanceFinder.TransportManager.Transport = newTransport;
            }
        }
    }
}
