# 🌐 Unity Online System Ready Package (FishNet + EOS)

*(TR version of this exhaustive document is available at the bottom / Kapsamlı teknik dokümanın Türkçe sürümü sayfanın alt kısmındadır.)*

**An Advanced, Modular, and UI-Agnostic Cross-Platform Multiplayer Framework**

Welcome to the **Unity Online System Ready Package**. Built primarily on **Unity 6000.3.9f1**, this framework leverages the sheer processing speed of **FishNet V4** combined with the global reach of **Epic Online Services (PlayEveryWare EOS)**. This package is explicitly designed to act as a plug-and-play architectural foundation for developers seeking an AAA-tier multiplayer environment without the heavy lifting of writing transport protocols, matchmaking logic, and anti-spam structures from scratch.

---

## 🏛 Architecture & Core Philosophies (English)

### 1. Seamless Matchmaking & Cross-Platform Synergy
Unlike traditional Unity networking where developers must wrestle with Port Forwarding, NAT punch-through constraints, and dedicated server hosting maintenance, this package implements a highly effective **Client-Hosted P2P Model** routed completely through Epic Games' master relay servers.
- **The 6-Digit Code System:** Hosts initiate a session securely, and the EOS backend instantly generates a globally accessible 6-digit invite code. Any client, no matter if they are interacting on Windows, PlayStation, Xbox, or Steam, can input this string to effortlessly bypass standard DNS boundaries and route locally into the host's scene.
- **Dynamic Transport Swapping:** We rely heavily on abstracted connections. The `NetworkConnectionManager` logically halts local IP transports (for UI menus) and forcefully injects the `FishyEOS` network transport layer the precise microsecond a cross-platform connection is verified by Epic.

### 2. Event-Driven Optimization (Strict Zero-Update Policy)
The traditional `Update()` loop is the nemesis of network bandwidth and scale. This package functionally eradicates its usage for state syncs.
- **SyncVar Network Hooks:** Crucial runtime variables inside `PlayerController.cs` (such as player names, status conditions, or future attributes) utilize FishNet's `[SyncVar(OnChange = nameof(Callback))]`. Practically, if 150 players are standing perfectly still inside a level, the network synchronization for those properties emits **0 Kilobytes/sec**. Data bursts occur strictly when structural mutations invoke them.
- **Modern API Refactoring:** Older Unity lookup pipelines (like `FindObjectsOfType`) induce catastrophic Garbage Collection (GC) latency spikes on high player counts. The codebase strictly commands Unity 6’s memory-efficient `FindFirstObjectByType(FindObjectsSortMode.None)` algorithm to ensure ultra-fast object resolutions during dynamic network instantiation.

### 3. Fortified Server Security & Defense Mechanisms
Public multiplayer P2P arrays are immediately vulnerable to flood abuse. This core network prevents crash exploits transparently.
- **Rate-Limited RPCs (Built-in Cooldowns):** Every mutation request dispatched by a client to the authority host (`[ServerRpc]`) is met with a server-side `Time.time` gated interval. Even if a malicious user unleashes a macro to trigger 5,000 "Action/Shoot/Change Name" commands per second, the framework ingests exactly *one* valid request per tick threshold, filtering out the remaining spam safely and raising a yellow `[Network Security]` flag for admins. This completely neutralizes packet flood/DDoS style engine freezes.
- **Client Race Condition Prevention:** Preemptive ServerRpc data is dynamically halted until genuine local scope verification is achieved via `OnStartClient()`. This mathematically guarantees that the active Client Socket is fully operational before executing initial authority handshakes, stopping phantom data loops.

### 4. Zombie Lobby Cleanup (Graceful Disconnections)
A common oversight in P2P libraries is the generation of "Ghost/Zombie Lobbies". When a Host forcibly interrupts standard execution via `Alt+F4` or encounters sudden total packet loss, the lobby entity lingers virtually accessible on the global matchmaking database.
- **The Asynchronous Solution:** This architecture actively subscribes network lifecycle hooks into FishNet's `OnServerConnectionState`. The absolute second a transport socket enters a `Stopped` sequence, an unyielding background execution targets `LeaveOrDestroyLobby`. This flawlessly and synchronously purges the physical and literal session tracks from the Epic Games Cloud without risking thread deadlock.

---
## 📗 Implementation Guides & SDK Documents
We have drafted absolute necessities in root level documentation files to direct custom development:
*   **project_testing_guide.md**: **[CRITICAL WARNING]** Read this immediately before attempting to flip "Integrated Platform Management Flags" regarding Steam configurations within the Unity Editor. Ignoring this might throw endless App-ID initialization crash loops.
*   **roadmap.md** & **todo.md**: The macro-task lists containing deliberate architecture omissions (e.g., maintaining full sync capability via `NetworkTransform` Scale features to ensure maximal package modularity for differing game philosophies).
*   **network_optimization_guide.md**: Extensive breakdown of the principles deployed.

---

*(Türkçe Versiyonu)*

# 🌐 Unity Çevrimiçi Sistem Hazır Altyapı Paketi (FishNet + EOS)

**Gelişmiş, Modüler ve Arayüzden Bağımsız (UI-Agnostic) Çapraz Platform Ağ Altyapısı**

**Unity Online System Ready Package** projesine hoş geldiniz. Tamamen **Unity 6000.3.9f1** merkezine entegre edilmiş bu olağanüstü iskelet mimarisi; **FishNet V4**'ün rakipsiz veri işleme hızı ile **Epic Online Services (EOS)**'in eşsiz küresel sunucu ağını (Relays) kombine eder. Bu varlık paketi (Asset), saatlerce uğraş gerektiren oturum açma, yönlendirici (Router) delme, port izinleri ve sunucu güvenliği altyapılarını tek tıkta çözen "Tak Çalıştır" bir şablon olması vizyonuyla hazırlanmıştır. Geliştiricilere tertemiz bir AAA temel hediye eder.

---

## 🏛 Mimari Değerler ve Alt Sistem Prensibi (Türkçe)

### 1. Pürüzsüz EOS Matchmaking ve Gerçek Cross-Play (Çapraz Oyun) Deneyimi
Geleneksel Unity sistemlerinde iki oyuncunun uzaktan eşleşmesi için oyuncuların modem panellerine girip TCP/UDP portları açması, veya geliştiricinin pahalı uzak sunucular (VDS) kiralaması gerekir. Bu paket, her şeyi Epic Catcher ve Local-Host sisteminde P2P olarak kökten çözer.
- **6 Haneli Kriptolu Odalar:** Altyapıyı başlatan kurucu (Host) arkaplanda saliseler içerisinde Epic Games bulutuyla konuşarak dışa kapalı bir lobi açar ve evrensel "6 Haneli" bir oturum kodu talep eder. O an dünyanın bambaşka bir kıtasında Xbox, PlayStation, Steam ya da Switch kullanan arkadaşı sadece o 6 haneli kodu inputa girerek engelsiz bir şekilde oturuma dahil olur.
- **Dinamik Taşıyıcı Gücü (Transport Swap):** Mimari tasarımsal bir deha kullanır. `NetworkConnectionManager` ana menü yüklerini yerel portta tutarken, uzaktan gelen bir komutta sistemi susturup bir saniyenin binde biri sürede Epic Games'in ağ protokolü olan `FishyEOS` modülünü motora zerk eder. Kusursuz bir geçiş hissi yaratılır.

### 2. Olay Tabanlı Optimizasyon (Sıfır Update Algoritması)
Multiplayer oyun geliştiricilerinin düştüğü en büyük hata olan "Saniyede 60 kez sorgu sorma" (`Update()` metodu) hastalığı bu projeden tamamen kazınmıştır.
- **FishNet SyncVar Mimarisi:** Oyuncunun adı, hayati değerleri veya cephanesi gibi kritik veriler yalnızca kanca/uyarıcı (Hook - `[SyncVar(OnChange)]`) yapısına kodlanmıştır. Sonuç olarak eğer odada 150 oyuncu sabit duruyorsa sunucunun bant genişliği kullanımı tamamen **0 Byte** civarıdır. Komut sadece "fiziksel bir değişim" olduğu asenkron anlarda ağa basılır ve işlemci serin kalır.
- **Hantal Arama Yüklerinden Kurtuluş:** Unity 6 devriminin arkasında bırakmak zorunda olduğu, Garbage Collector (Bellegin Çöp Kutusu) patlamalarına yol acan tüm geri kalmış obje tarama kalıpları silinmiştir. Sistemin kalbi, sahne içi donmaları kökten kesen yüksek optimizasyonlu `FindFirstObjectByType(FindObjectsSortMode.None)` ile tekrar dizayn edilmiştir.

### 3. Sunucu Savunması ve Çelik Yelek (Anti-Spam Mimarisi)
Bir oyunu halka açtığınız gün siber tehditler altındadır. Bizim kurguladığımız Host sistemi özel saldırıları en baştan kilitler.
- **Ağ Seli Korumalı Zaman Kilidi (RPC Rate Limiting):** C++ flood atakları ve macro spam saldırıları, paketin içindeki `[ServerRpc]` kanallarında direkt engellenir. Bir oyuncu sisteme bir "Değişim/Aktarım" isteğinde bulunduğunda, arkaplanda işleyen güçlü bir `Time.time` kaba kronometresi kontrolünden geçirilir. İstek kapasitesi ne olursa olsun saniyede filtreli geçer, kalan her şey reddedilerek konsola sarı etiketli **[Network Security] Spam Warning** uyarısı bastırır. Oda asla çökmez.
- **Doğum/Uyanma Sırası ve Race Condition Defansı:** Balon uyarılarını yaratan aceleci Network uyanmaları durdurulmuştur. Karakter lokal kimliğine bürünmeden önce veriler kilitlenir, `OnStartClient` tetikleyicisi garanti olmadan herhangi bir veri buluta bırakılmaz.

### 4. Hayalet Lobileri Yok Eden Zarif Kopuş Seremonisi (Graceful Disconnect)
Klasik internet projelerinde oyun kurucunun bilgisayarı çöktüğünde veya fişi çektiğinde (`Alt+F4`), kurduğu "Hazır Lobi" ana eşleştirme havuzunda saatlerce asgari olarak kirli verilerle listelenmeye devam eder. 
- **Temizlik Tetikleyicisi Çözümü:** İskeletimiz, FishNet'in koptuğunu hisseden global kancalara (`OnServerConnectionState`) sıkı sıkıya bağlıdır. Taşıma katmanı koptuğu an sismik bir sarsıntı gibi durumu hisseden mimari, arkasından acil durum prosedürü çalıştırır. Asenkron tasarlanmış `LeaveOrDestroyLobby` emri buluta uçarak lobiyi, verileri ve oyuncuları tertemiz de-aktif eder; hafızada (zombie instance) tortu kalmaz.

---
## 📗 Kapsamlı Proje Belgeleri ve Rehber Evraklar
Paketin doğası, kuralları ve test süreçlerine yönelik okunması mecburi evraklar ana dizindedir:
*   **`project_testing_guide.md` : [HAYATİ UYARI]** Unity içerisinde PlayEveryWare eklentisinde App ID (Steam kimliği) girmeden platform değiştirmeyi denerseniz yaşanacak "Sonsuz Crash Döngüleri" ve çözüm şemaları.
*   **`roadmap.md` & `todo.md` :** Neden `NetworkTransform` üzerindeki scale (boyut) engellerinin bilerek tasarımsal olarak es geçildiği gibi çok özel vizyonel paket üretme metotları içerir.
*   **`network_optimization_guide.md` :** Sunucu kalbi kurarken kod yazma pratikleri ve paket entegrasyon ödevlerini belgeler. 
