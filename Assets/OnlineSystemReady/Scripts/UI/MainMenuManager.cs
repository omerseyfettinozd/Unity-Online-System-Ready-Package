using UnityEngine;
using TMPro; // TextMeshPro for modern Unity UI
using OnlineSystemReady.Core;

namespace OnlineSystemReady.UI
{
    /// <summary>
    /// Simple UI Manager to trigger the network connections.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI Panels")]
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
            mainMenuPanel.SetActive(true);
            lanPanel.SetActive(false);
            eosPanel.SetActive(false);
        }

        // ---------- LAN ----------
        public void ShowLANPanel()
        {
            mainMenuPanel.SetActive(false);
            lanPanel.SetActive(true);
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

        // ---------- EOS ----------
        public void ShowEOSPanel()
        {
            mainMenuPanel.SetActive(false);
            eosPanel.SetActive(true);
        }

        public void OnEOSHostClicked()
        {
            NetworkConnectionManager.Instance.StartOnlineHost();
            HideAllPanels();
        }

        public void OnEOSClientClicked()
        {
            NetworkConnectionManager.Instance.StartOnlineClient();
            HideAllPanels();
        }

        // ---------- SPLIT SCREEN ----------
        public void OnSplitScreenClicked()
        {
            NetworkConnectionManager.Instance.StartSplitScreen();
            HideAllPanels();
        }

        private void HideAllPanels()
        {
            mainMenuPanel.SetActive(false);
            lanPanel.SetActive(false);
            eosPanel.SetActive(false);
        }
    }
}
