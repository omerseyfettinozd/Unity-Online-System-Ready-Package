using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OnlineSystemReady.UI
{
    /// <summary>
    /// Tamamen bağımsız, sürükle-bırak (Drag) tabanlı sanal Joystick.
    /// Dokunulan noktadan parmak sürüklendikçe yön verir, bırakınca sıfırlanır.
    /// 
    /// Kullanım: MobileInputManager tarafından otomatik oluşturulur.
    /// Manuel kullanım: Herhangi bir Canvas üzerindeki bir UI Image objesine eklenebilir.
    /// 
    /// Çıkış: MobileJoystick.Input (statik Vector2) → PlayerController doğrudan okur.
    /// </summary>
    public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        /// <summary>
        /// Tüm sistemin okuması gereken statik joystick çıkışı.
        /// PlayerController bu değeri doğrudan okur.
        /// </summary>
        public static Vector2 Input { get; private set; } = Vector2.zero;

        [Header("Joystick Ayarları")]
        [Tooltip("Joystick topunun (knob) arka plandan ne kadar uzağa gidebileceği (piksel).")]
        public float handleRange = 60f;

        [Tooltip("Joystick'in girdi vermeye başlaması için gereken minimum eşik (Dead Zone). 0-1 arası.")]
        public float deadZone = 0.1f;

        // İç Referanslar
        private RectTransform _backgroundRect;  // Joystick arka planı (dış daire)
        private RectTransform _handleRect;       // Joystick topu (iç daire / knob)
        private Canvas _parentCanvas;
        private Camera _uiCamera;

        private Vector2 _inputVector = Vector2.zero;

        private void Start()
        {
            // Referansları otomatik bul
            _backgroundRect = GetComponent<RectTransform>();

            // İlk çocuk obje = handle (knob)
            if (transform.childCount > 0)
            {
                _handleRect = transform.GetChild(0).GetComponent<RectTransform>();
            }

            _parentCanvas = GetComponentInParent<Canvas>();
            if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                _uiCamera = _parentCanvas.worldCamera;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_backgroundRect == null || _handleRect == null) return;

            // Dokunma noktasını Joystick arka planının lokal koordinatına çevir
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _backgroundRect, eventData.position, _uiCamera, out localPoint
            );

            // Arka planın boyutuna göre normalize et (-1 ile 1 arası)
            Vector2 bgSize = _backgroundRect.sizeDelta;
            _inputVector = new Vector2(
                localPoint.x / (bgSize.x * 0.5f),
                localPoint.y / (bgSize.y * 0.5f)
            );

            // Vektörü 1'e sınırla (dairenin dışına çıkmasın)
            _inputVector = Vector2.ClampMagnitude(_inputVector, 1f);

            // Dead Zone kontrolü
            if (_inputVector.magnitude < deadZone)
            {
                _inputVector = Vector2.zero;
            }

            // Knob'u (topu) hareket ettir
            _handleRect.anchoredPosition = _inputVector * handleRange;

            // Statik çıkışı güncelle
            Input = _inputVector;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Parmak kaldırıldığında her şeyi sıfırla
            _inputVector = Vector2.zero;
            Input = Vector2.zero;

            if (_handleRect != null)
            {
                _handleRect.anchoredPosition = Vector2.zero;
            }
        }

        private void OnDisable()
        {
            // Script devre dışı kalırsa girdiyi sıfırla (PC moduna geçişte güvenlik)
            _inputVector = Vector2.zero;
            Input = Vector2.zero;
        }

        // --- PROGRAMATIK OLUŞTURUCU (Sprite Gerekmez!) ---
        /// <summary>
        /// Verilen Canvas'ın içine tamamen koddan bir Joystick oluşturur.
        /// Dış daire (arka plan) ve iç daire (knob/top) olarak iki adet UI Image kullanır.
        /// Herhangi bir sprite/asset gerektirmez.
        /// </summary>
        public static MobileJoystick CreateJoystick(Transform canvasTransform)
        {
            // 1) Arka Plan (Background) — Sol Alt Köşe
            GameObject bgObj = new GameObject("Joystick_Background");
            bgObj.transform.SetParent(canvasTransform, false);

            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0f);  // Sol Alt
            bgRect.anchorMax = new Vector2(0f, 0f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.anchoredPosition = new Vector2(160f, 160f); // Köşeden biraz içeri
            bgRect.sizeDelta = new Vector2(200f, 200f);

            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(1f, 1f, 1f, 0.25f); // Yarı saydam beyaz
            bgImage.raycastTarget = true;

            // Arka planı yuvarlak yapmak için Unity'nin varsayılan dairesel sprite'ını kullan
            // (Knob sprite'ı da aynı şekilde)
            // Unity'de "Knob" adlı built-in sprite, Resources altında mevcuttur.
            Sprite knobSprite = Resources.Load<Sprite>("unity_builtin_extra:UI/Skin/Knob.psd");
            // Eğer bulamazsa sadece kare kalır ama fonksiyonel olarak sorunsuz çalışır
            if (knobSprite != null) bgImage.sprite = knobSprite;

            // 2) Knob (Handle / İç Top)
            GameObject handleObj = new GameObject("Joystick_Handle");
            handleObj.transform.SetParent(bgObj.transform, false);

            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0.5f, 0.5f); // Tam ortada
            handleRect.anchorMax = new Vector2(0.5f, 0.5f);
            handleRect.pivot = new Vector2(0.5f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(80f, 80f);

            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0.6f); // Daha opak beyaz
            handleImage.raycastTarget = false; // Sadece arka plan dokunmayı yakalar

            if (knobSprite != null) handleImage.sprite = knobSprite;

            // 3) MobileJoystick bileşenini arka plana ekle
            MobileJoystick joystick = bgObj.AddComponent<MobileJoystick>();
            joystick.handleRange = 60f;
            joystick.deadZone = 0.1f;

            return joystick;
        }
    }
}
