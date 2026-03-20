using UnityEngine;
using TMPro; // TextMeshPro
using OnlineSystemReady.Core;

namespace OnlineSystemReady.UI
{
    /// <summary>
    /// Simple UI Manager to trigger the network connections.
    /// Also includes an OnGUI fallback for instant testing without Canvas setup.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI Panels (Optional for tests)")]
        public GameObject mainMenuPanel;
        public GameObject lanPanel;
        public GameObject eosPanel;

        // OnGUI Test menüsü için geçici giriş kod tutucuları
        private string _eosCodeInput = "";
        private string _lanCodeInput = "";

        private void Start()
        {
            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (lanPanel != null) lanPanel.SetActive(false);
            if (eosPanel != null) eosPanel.SetActive(false);
        }

        public void ShowLANPanel()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (lanPanel != null) lanPanel.SetActive(true);
        }

        public void OnLANHostClicked()
        {
            NetworkConnectionManager.Instance.StartLANHost();
            HideAllPanels();
        }

        public void OnLANClientClicked()
        {
            NetworkConnectionManager.Instance.StartLANClient("localhost");
            HideAllPanels();
        }

        private void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (lanPanel != null) lanPanel.SetActive(false);
            if (eosPanel != null) eosPanel.SetActive(false);
        }

        // --- OnGUI Fallback (Eğer UI Canvas atanmamışsa ekrana otomatik menü çizer) ---
        private void OnGUI()
        {
            // Eğer Inspector'dan Canvas UI bağlandıysa bu test menüsünü çizme
            if (mainMenuPanel != null && mainMenuPanel.activeSelf) return;

            // Eğer bağlantı kurulduysa HUD'u gizle
            if (FishNet.InstanceFinder.ClientManager != null && FishNet.InstanceFinder.ClientManager.Started) return;
            if (FishNet.InstanceFinder.ServerManager != null && FishNet.InstanceFinder.ServerManager.Started) return;

            GUILayout.BeginArea(new Rect(10, 10, 200, 300), GUI.skin.box);
            // --- YEREL AĞ (LAN) GUI ---
            GUILayout.Label("--- YEREL AĞ (LAN) ---");
            if (LANMatchmakingManager.Instance != null)
            {
                GUILayout.Label("Durum: " + LANMatchmakingManager.Instance.currentStatus);
                
                if (!string.IsNullOrEmpty(LANMatchmakingManager.Instance.currentRoomCode))
                {
                    GUI.color = Color.cyan;
                    GUILayout.Label("ODA KODUNUZ: " + LANMatchmakingManager.Instance.currentRoomCode);
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("LAN - Oda Kur"))
                {
                    LANMatchmakingManager.Instance.StartHostLAN();
                    HideAllPanels();
                }

                GUILayout.Space(5);
                GUILayout.Label("LAN Koda Bağlan:");
                _lanCodeInput = GUILayout.TextField(_lanCodeInput, 6);
                
                if (GUILayout.Button("LAN - Odaya Bağlan"))
                {
                    LANMatchmakingManager.Instance.StartClientLAN(_lanCodeInput);
                    HideAllPanels();
                }
            }
            else
            {
                GUILayout.Label("(LANMatchmakingManager Bulunamadı)");
            }

            GUILayout.Space(10);
            // --- EPIC ONLINE SERVICES GUI ---
            GUILayout.Label("--- ONLINE (EOS) ---");
            if (EOSMatchmakingManager.Instance != null)
            {
                GUILayout.Label("Durum: " + EOSMatchmakingManager.Instance.currentStatus);
                
                // Host olunca gösterilen kod
                if (!string.IsNullOrEmpty(EOSMatchmakingManager.Instance.currentRoomCode))
                {
                    GUI.color = Color.green;
                    GUILayout.Label("ODA KODUNUZ: " + EOSMatchmakingManager.Instance.currentRoomCode);
                    GUI.color = Color.white;
                }

                if (GUILayout.Button("Online - Host (Oda Kur)"))
                {
                    EOSMatchmakingManager.Instance.StartHostEOS();
                }

                GUILayout.Space(5);
                GUILayout.Label("Katılmak için 6 Haneli Kod:");
                _eosCodeInput = GUILayout.TextField(_eosCodeInput, 6);
                
                if (GUILayout.Button("Online - Client (Koda Bağlan)"))
                {
                    EOSMatchmakingManager.Instance.StartClientEOS(_eosCodeInput);
                }
            }
            else
            {
                GUILayout.Label("(EOSMatchmakingManager Bulunamadı)");
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Local Split-Screen"))
            {
                NetworkConnectionManager.Instance.StartSplitScreen();
                HideAllPanels();
            }

            GUILayout.EndArea();
        }
    }
}
