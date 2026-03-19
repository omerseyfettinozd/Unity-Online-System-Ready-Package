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

        [Header("Inputs")]
        public TMP_InputField lanIpInput;

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
            string ip = "localhost";
            if (lanIpInput != null && !string.IsNullOrWhiteSpace(lanIpInput.text))
            {
                ip = lanIpInput.text;
            }
            NetworkConnectionManager.Instance.StartLANClient(ip);
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
            GUILayout.Label("Network Test Menu");

            if (GUILayout.Button("LAN - Host (Sunucu)"))
            {
                OnLANHostClicked();
            }
            if (GUILayout.Button("LAN - Client (İstemci)"))
            {
                OnLANClientClicked();
            }
            if (GUILayout.Button("Local Split-Screen"))
            {
                NetworkConnectionManager.Instance.StartSplitScreen();
                HideAllPanels();
            }

            GUILayout.EndArea();
        }
    }
}
