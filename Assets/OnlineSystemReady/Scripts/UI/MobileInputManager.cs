using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OnlineSystemReady.UI
{
    /// <summary>
    /// Mobil cihazlarda otomatik olarak ekrana sanal Joystick çıkaran,
    /// PC/Konsolda ise joystick'i gizleyen platform algılama yöneticisi.
    /// 
    /// Bu script NetworkManager objesine (veya herhangi bir sahne objesine) eklenir.
    /// Awake'te platforma göre kendini açar/kapatır.
    /// 
    /// Joystick görselleri tamamen koddan oluşturulur, sprite veya asset gerekmez.
    /// </summary>
    public class MobileInputManager : MonoBehaviour
    {
        public static MobileInputManager Instance;

        [Header("Ayarlar")]
        [Tooltip("True ise Editor'da bile joystick'i gösterir (test amaçlı).")]
        public bool forceShowInEditor = false;

        // Oluşturulan Canvas ve Joystick referansları
        private Canvas _mobileCanvas;
        private MobileJoystick _joystick;
        private GameObject _eventSystemObj;

        /// <summary>
        /// Joystick'in şu an ekranda görünür olup olmadığını döndürür.
        /// </summary>
        public bool IsJoystickActive => _mobileCanvas != null && _mobileCanvas.gameObject.activeSelf;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // Platform algılama: Mobil mi değil mi?
            bool isMobile = IsMobilePlatform();

            if (isMobile || forceShowInEditor)
            {
                CreateMobileUI();
                Debug.Log("<color=green>[MobileInput] Mobil platform algılandı! Sanal Joystick oluşturuldu.</color>");
            }
            else
            {
                Debug.Log("[MobileInput] PC/Konsol platformu algılandı. Joystick gizli.");
            }
        }

        /// <summary>
        /// Platformun mobil olup olmadığını tespit eder.
        /// Unity'nin Application.isMobilePlatform'u + Input System'in Touchscreen kontrolü.
        /// </summary>
        private bool IsMobilePlatform()
        {
            // 1) Doğrudan mobil platform kontrolü (Android, iOS)
            if (Application.isMobilePlatform) return true;

            // 2) Runtime platform kontrolü (Daha güvenilir)
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return true;
            }

            // 3) Input System ile dokunmatik ekran kontrolü (Hibrit cihazlar: Surface, Tablet PC vb.)
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Touchscreen.current != null)
            {
                return true;
            }
#endif

            return false;
        }

        /// <summary>
        /// Mobil kontrol arayüzünü (Canvas + EventSystem + Joystick) tamamen koddan oluşturur.
        /// </summary>
        private void CreateMobileUI()
        {
            // 1) EventSystem kontrolü (Sahnede zaten varsa tekrar oluşturma)
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                _eventSystemObj = new GameObject("EventSystem_Mobile");
                _eventSystemObj.transform.SetParent(transform);
                _eventSystemObj.AddComponent<EventSystem>();

                // Input System'e göre doğru Input Module'ü ekle
#if ENABLE_INPUT_SYSTEM
                _eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                _eventSystemObj.AddComponent<StandaloneInputModule>();
#endif
            }

            // 2) Canvas oluştur (Ekranın en üstüne çizilecek katman)
            GameObject canvasObj = new GameObject("MobileControls_Canvas");
            canvasObj.transform.SetParent(transform);

            _mobileCanvas = canvasObj.AddComponent<Canvas>();
            _mobileCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _mobileCanvas.sortingOrder = 100; // Her şeyin üstünde

            // Canvas Scaler (Farklı ekran boyutlarında uyum)
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f; // Hem yatay hem dikey dengeleme

            canvasObj.AddComponent<GraphicRaycaster>();

            // 3) Joystick'i Canvas'ın içine oluştur
            _joystick = MobileJoystick.CreateJoystick(canvasObj.transform);
        }

        /// <summary>
        /// Joystick'i runtime'da açıp kapatmak için (Ayarlar menüsü vb.).
        /// </summary>
        public void SetJoystickVisible(bool visible)
        {
            if (_mobileCanvas != null)
            {
                _mobileCanvas.gameObject.SetActive(visible);
            }
        }

        private void OnDestroy()
        {
            // Statik girdiyi temizle
            // MobileJoystick.Input zaten OnDisable'da sıfırlanıyor
        }
    }
}
