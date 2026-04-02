# 🌐 Unity Online System Ready Package (FishNet + EOS)

*(Türkçe Çeviriler ve Kapsamlı Modül Anlatımları aşağıdadır.)*

**The Ultimate, Modular, UI-Agnostic Multiplayer Framework for Unity 6**

Welcome to the **Unity Online System Ready Package**. Built natively on **Unity 6000.3.9f1**, this architecture is a plug-and-play `.unitypackage` asset designed to bridge the incredible data-shuffling speed of **FishNet V4** with the global matchmaking servers of **PlayEveryWare EOS (Epic Online Services)**. 

By eliminating the necessity for tedious transport coding, NAT routing, and backend session management, this framework provides developers with an absolute AAA-tier multiplayer backbone out-of-the-box.

---

## 🏛 Core Architectures & Feature Deep-Dive

### 1. Cross-Platform Matchmaking & EOS P2P (Global)
This package uses a client-hosted relay approach. 
Instead of paying for expensive dedicated servers, players seamlessly host matches on their own devices. The backend creates an ephemeral hidden lobby on Epic Games servers and links an encrypted **6-Digit Invite Code**. Players connecting from Windows, Steam, PlayStation, or Xbox simply enter the code, bypassing all firewall or port forwarding rules entirely via `FishyEOS` transports.

### 2. Zero-Config Local Area Network (UDP Broadcasting)
Often, testing or local playing requires rigid IP typing. We eliminated this via the `LANMatchmakingManager.cs`. 
If a host opens a local game, the system emits an automated asynchronous **UDP Broadcast pulse** (`FISHNET_LAN|Code`). Another device on the same WiFi network trying to find that code simply listens to local socket pulses. The moment a match is caught, the IP address is instantly decoded and the `NetworkConnectionManager` violently swaps the transport bridge to intercept the local connection instantly.

### 3. Editor Clone Emulation (ParrelSync EOS Helper)
Developing multiplayer games locally usually creates a strict hardware conflict with EOS, as Epic Games recognizes the same machine hardware and rejects clone-to-clone P2P handshakes. 
Our `ParrelSyncEOSHelper.cs` acts as a middleman process. The absolute microsecond the project executes on a ParrelSync Clone environment, the script overrides Epic's hardware-ID fetching sequence, violently erasing the stored `DeviceId` and spoofing a completely unique machine ID hash. This effectively tricks Epic Servers into allowing fully functioning P2P tests on a single PC!

### 4. Split-Screen Couch Co-op Integration
Not every match has to be online. `SplitScreenManager.cs` instantly divides the viewport (`Rect` manipulation) to render vertical split-screening on a single device, allocating inputs strictly locally while avoiding FishNet latency traps for Player 2.

### 5. Defensive Network Security & Anti-Spam Rates
The `PlayerController.cs` houses an enterprise-grade `Time.time` chronometer filter acting as an RPC Rate Limiter. If malicious hackers construct input macros scaling thousands of clicks per second to overwhelm the Host CPU, our backend intercepts, drops duplicate/abusive request packets (`[ServerRpc]`), and prints a simple yellow security warning. Total Host Crash Immunity.

---

# 🌐 Unity Çevrimiçi Sistem Hazır Altyapı Paketi (Türkçe)

**Unity 6 İçin Geliştirilmiş, Nihai, Modüler Çapraz Platform Ağ Projesi**

**Unity 6000.3.9f1** merkezli bu yapı, oyun dünyasının en hızlı Ağ Kütüphanesi olan **FishNet V4** ile dünyanın neresinden oynandığının önemi olmayan devasa sunucu ağı **Epic Online Services (EOS)** gücünü asenkron olarak tek elde toplamaktadır. Geliştiricilerin arayüzden ve spagetti kodlardan arındırılmış sağlam bir çekirdeği anında projelerine import edebilmesi amaçlanmıştır.

---

## 🏛 Çekirdek Modüller ve Mimariler

### 1. Çapraz Oyun (Cross-Play) ve P2P Eşleştirme (Bulut)
Standart Unity projelerinde bir oyuncuya bağlanmak port açmayı veya VDS sunucu kiralamayı gerektirir. Bizim hazırladığımız `EOSMatchmakingManager.cs` altyapısı bu yükleri temizler:
- Kurucu (Host) odayı açtığı an Epic Games bulutuna bir tünel kazar ve rastgele oluşan **6 Haneli Davet Kodu** alır. 
- Arkadaşı (Steam, Xbox, Mobil vs.) bu numarayı arattığı an Epic sistemleri her iki bilgisayarın modem engelini (NAT Punch-through) özel yollarla kırarak onları birbirine bağlar ve oyuncular anında oyuna başlar. Tamamen ücretsiz uçtan uca iletişim.

### 2. IP İstemeyen Kusursuz Yerel Ağ (LAN Broadcast)
Aynı modeme bağlı iki oyuncunun oyun oynaması için birinin cmd açıp ipconfg/IPV4 kovalayıp adres yazması amatorcedir.
Bunun yerine `LANMatchmakingManager.cs` devreye girer. Host odayı kurduğunda, modem içerisindeki havaya her 1.5 saniyede bir telsiz dalgası (`FISHNET_LAN|DavetKodu` formatında UDP yayını) gönderilir. Odayı arayan telefon veya cihaz, şifrenin olduğu dalgayı yakaladığı salise hedef IP adresini çözer ve saniyelik bir şekilde taşıyıcı (Transport) değiştirilerek odaya anında bağlanılır.

### 3. Editor Geliştirme Sihirbazı (ParrelSync EOS Helper)
Oyun yaparken aynı bilgisayarda pencere çoğaltarak test yapmak standarttır (ParrelSync). Ancak EOS, aynı bilgisayardaki donanım kilitlerini okuyup iki aynı adamın kendine bağlanmasını kesinlikle yasaklayan sert güvenlik ağına sahiptir.
Bu yüzden projede `ParrelSyncEOSHelper.cs` (Gizli Tetikleyici) vardır. Bu dosya Editör arka planında klon çalıştığı an Epic Games'in kilitlerini kırarak *"Clone_*"* isminde yeni donanım (DeviceID) şifreleri üretir ve sunucuyu sizin iki farklı bilgisayardan girdiğinize inandırır! 

### 4. Beden Birleşimi (Local Co-Op ve Split-Screen)
Herkesin interneti olmak zorunda değil. Ağa dahil edilmeyen, tamamen yan yana oynama özelliği için `SplitScreenManager.cs` devrededir. Ekran yatay olarak ana kameralar tarafından ikiye bölünür, ikinci karakterin ağda senkronlanmasına gerek duyulmadan fiziksel bir klon yaratılır ve aynı evdeki iki insanın aynı cihazdan mükemmel fps ile oyun oynamasına zemin atan kod bloğu altyapıya dahil edilmiştir.

### 5. Sunucu Defansı: Kaba Kuvvet/Spam Ataklarına Koruma
Bir bilgisayar hem oynayıp hem sunucu olduğu (Host) durumlarda, hile açanlar aralıksız tuşlara basarak mermi komutunu (RPC) saniyede 10.000 defa ana makineye fırlatıp saniyeler içerisinde ana bilgisayarı kitleyip çökertebilir. 
Projemizdeki `PlayerController.cs` içerisinde her bir işlemi denetleyen **Zaman Filtresi (Rate Limiter)** inşa edilmiştir. Değişiklik paketleri (örn. isim değiştirme) saniyede yalnızca 1 kere filtreden geçebilir. Sınırdan fazla gelen binlerce işlem, makineyi daha yormadan çöpe atılır ve sunucu güvenlikle yoluna devam eder.

---

## 🛠 RELEASE NOTES (SÜRÜM NOTLARI)

### v0.2.2 - Documentation Extravaganza
- **Docs:** Readme page was profoundly rebuilt highlighting entire core codebase logic including `LANMatchmakingManager` (UDP Radio Broadcasting) and `ParrelSyncEOSHelper` (Device ID spoofing for dev-tests). 
- **Tests:** `project_testing_guide.md` explicitly defines Steamworks crash protections.

### v0.2.1 - Core Security & Flood Measures
- **Security:** Injected `NAME_CHANGE_COOLDOWN` macro-defense into `PlayerController.CmdSetPlayerName` `[ServerRpc]`.
- **Roadmap:** Discarded generic `NetworkTransform` scaling limitations to secure developer flexibility for diverse game projects (Big/Small character variants perfectly persist natively).

### v0.2.0 - Fundamental Code Health & Race Fixes
- **Modernization:** Decimated heavy `Update` loops, porting entirely to Event-Driven FishNet `SyncVar(OnChange)`.
- **API Uplifting:** Obsoleted all `FindObjectOfType` legacy engine scripts inside `EOSMatchmakingManager`, upgrading the entirety of the platform onto Unity 6's streamlined `FindFirstObjectByType`.
- **Async Reliability:** Constructed robust `OnClientConnectionState` delegates. Abrupt window closures (`Alt+F4`) invoke guaranteed `LeaveOrDestroyLobby` execution, eradicating ghost sessions from Epic's backend permanently.
- **Boot Race Condition:** Transitioned data replication commands safely into `OnStartClient` scopes to avoid `OnStartNetwork` timing discrepancies where Server Rpc invocations clashed with disconnected backend lifecycles.
