# 🌐 Unity Online System Ready Package (FishNet + EOS)

*(TR version of this document is available at the bottom / Türkçe çevirisi sayfanın alt kısmındadır.)*

**An Advanced, Modular, and UI-Agnostic Cross-Platform Multiplayer Framework**

This project is a professional core infrastructure built on **Unity 6000.3.9f1**, utilizing **FishNet V4** and **Epic Online Services (EOS by PlayEveryWare)**. It is meticulously designed for developers to easily export via `.unitypackage` into any project, providing an immediate triple-A tier multiplayer environment without the headache of writing complex network architecture.

---

## 🚀 Key Features (English)

### 1. Seamless EOS P2P & Cross-Play Ready Matchmaking
- **Zero Port Forwarding:** Utilizes Epic Games servers for NAT punch-through. Players can connect instantly using a "6-Digit Invite Code" without modifying their router settings.
- **Native Cross-Platform:** Because the core relies on EOS, games built upon this package have native cross-play support integrating Steam, PlayStation, Xbox, and Nintendo Switch accounts out-of-the-box. (See `project_testing_guide.md` for SDK integration boundaries).
- **Graceful Disconnect & No Zombie Lobbies:** Robust hooks integrated into FishNet's `OnServerConnectionState` ensure that if a player force quits (Alt+F4) or loses connection, the dead "zombie" lobbies on Epic Games servers are automatically flushed out using asynchronous `LeaveOrDestroyLobby` commands.

### 2. Event-Driven & High-Performance Synchronization
- **Zero-Update Overhead:** Moving away from traditional, bulky `Update` synchronization loops, this package relies heavily on **FishNet SyncVar(OnChange)** variables. Data points (like player names or stats) only consume CPU cycles precisely when the source value is altered.
- **Optimized Engine APIs:** Fully updated for modern Unity 6 architecture. Slower API calls such as `FindObjectsOfType` have been completely stripped from the core module and replaced with highly optimized `FindFirstObjectByType(FindObjectsSortMode.None)` queries.

### 3. Server Security & Anti-Spam Architecture
- **Rpc Rate-Limiting (Cooldowns):** To protect Host servers from macro-wielding hackers and DDoS-style flood attacks, strict asynchronous time-gates (`Time.time` cooldown filters) are implemented natively inside foundational `[ServerRpc]` input points. Malicious requests are blocked securely before they impact the network stream.
- **Client Sync Assurance:** Network state race conditions were resolved by accurately delaying ServerRpc inputs until local ownership is verified internally via `OnStartClient()`.

---

## 🛠 File & Documentation Structure
- `roadmap.md` : Long-term development goals and architectural pivots.
- `todo.md` : Short-term technical objectives and package state checkpoints.
- `network_optimization_guide.md` : In-depth explanations of backend network decisions.
- `project_testing_guide.md` : QA rules regarding Steam App ID, SDK configurations, and crash-loop warnings.

---
*(Türkçe Versiyonu)*

# 🌐 Unity Çevrimiçi Sistem Hazır Altyapı Paketi (FishNet + EOS)

**Gelişmiş, Modüler ve Arayüzden Bağımsız Çapraz Platform Ağ Altyapısı**

Bu proje, **Unity 6000.3.9f1** motorunda inşa edilmiş olan; merkezinde **FishNet V4** ve **Epic Online Services (EOS)** kullanan profesyonel bir çekirdek altyapıdır (Framework). Geliştiricilerin saatlerce karmaşık bağlantı kodları yazmasına gerek kalmadan alıp diledikleri projeye "Unity Package" olarak dahil edebilecekleri AAA seviyesi bir modül olması amacıyla üretilmiştir.

---

## 🚀 Öne Çıkan Özellikler (Türkçe)

### 1. Pürüzsüz EOS P2P ve Çapraz Oyun (Cross-Play) Mimarisi 
- **Sıfır Port Yönlendirme Hatası:** Epic Games sunucu düğümleri sayesinde oyuncuların hiçbir modem (Router) port açma işlemi yapmasına gerek yoktur. Özel "6 Haneli Davet Kodu" altyapısıyla sadece şifre girilerek dünyanın öbür ucundaki sunucuya doğrudan bağlanılır.
- **Doğuştan Cross-Play (Örn: Steam):** Altyapı ana iletişim dili olarak EOS kullanıldığı için; bu paketi satın alan geliştirici arkaplanda projeyi doğrudan Steam, Xbox, PlayStation gibi mağazalarla bağlayabilir. (Önemli uyarılar için `project_testing_guide.md` kılavuzuna bakın).
- **Zarif Ağ Kopuşu (No-Zombie Lobbies):** Sunucuyu(Host) kuran veya odaya giren birisi oyununu aniden kapattığında veya interneti koptuğunda, Epic sunucularında asılı duran "Hayalet (Zombie) Odaları" silmek için `LeaveOrDestroyLobby` asenkron operasyonları devreye girer. Bağlantılar kopsa dahi arkaplanda çöp bırakılmaz.

### 2. Olay Tabanlı (Event-Driven) Üst Düzey Optimizasyon
- **Update Kullanımına Veda:** Performansı katleden `Update` içi senkronizasyon kontrolleri yerine **FishNet SyncVar(OnChange)** olay dinleyicileri (Hook) kullanılmıştır. Paket, oyuncu isminden karakterin can oranına kadar tüm verileri "Sadece değişiklik yapıldığı saniye" algılayarak sunucuda muazzam bir bant genişliği tasarrufu sağlar.
- **Unity 6 Mimari Uyumluğu:** Kod bloğundan eski nesil (`FindObjectsOfType` gibi) hantal tarama komutları kökten sökülüp atılmış, yerine Unity'nin yeni nesil motor tarayıcısı `FindFirstObjectByType(FindObjectsSortMode.None)` algoritması geçirilmiştir.

### 3. Sunucu Savunması (Anti-Spam / Saldırı Koruması)
- **RPC Spamlara Karşı Zaman Kilidi:** Kötü niyetli kişilerin Macro kullanarak saniyede on binlerce ağ komutu gönderip sunucuyu dondurması (DDoS/Flood engeli) ihtimaline karşı `[ServerRpc]` noktalarına kaba saniye kronometreleri (Cooldown algoritmaları) kodlanmıştır. Spam istekler, daha istemci ağındayken geri çevrilir.
- **Doğum/Uyanma Senkronik Hataları:** FishNet üzerinde İstemci-Sunucu yarışmalarına sebep olan ve objenin uyanışından (OnStartNetwork) kaynaklı zamanlama yarışmaları, yetki kontrolleri (Ownership) bazında garantilenerek çözülmüştür.

---

## 🛠 Dosya ve Oku-Beni (Dokümantasyon) Yapısı
Proje klasörünün içerisinde sistemin nasıl kullanılacağına dair Markdown rehberleri bulunmaktadır:
- `roadmap.md` : Uzun vadeli mimari hedefler ve eklenen sistemler.
- `todo.md` : Adım adım inşa süreçleri ve kalan hedefler listesi.
- `network_optimization_guide.md` : Paket yöneticisi ve teknik ağ pratikleri.
- `project_testing_guide.md` : Steam App ID boşken test yapma riskleri ve sistem entegrasyon kuralları.
