# Proje İlerleme Durumu (Progress Log)

## Mevcut Durum (Current Status)
- [x] NetworkManager ve Player prefab'larının otomatik oluşturulması (`NetworkSetupWizard.cs`) kodlandı.
- [x] Host ekranında Client kapsüllerindeki titreme Slerp ile kusursuz süzülmeye (Jitter Fix) çevrildi.
- [x] Epic Games (EOS) ve LAN (Yerel Ağ) için IP girmeden tamamen "6 haneli sayısal kod" ile odaya katılma sistemi (Matchmaking) tamamlandı.
- [x] Mobil kontroller: Sanal Joystick ve Platform Algılama sistemi eklendi. PC'de otomatik gizlenir, Mobilde otomatik açılır.
- [ ] Sıradaki Aşama: Lobi Bekleme Odası (Ready Room) ve NameTag (İsimlik) senkronizasyonu.

## Tamamlanan Özellikler (Completed Features)
- **EOSMatchmakingManager:** Epic SDK Lobi yapısını kullanan 6 haneli kod sistemi.
- **LANMatchmakingManager:** UDP Broadcast radyo dalgaları ile IP girmeyi bitiren özel sistem.
- **MainMenuManager:** 6 haneli kodların girildiği ve iki ağ modunun birleştirildiği saf arayüz.
- **ParrelSyncEOSHelper:** Aynı cihazdaki klonların (ParrelSync) EOS ağına bağlanabilmesi için otomatik olarak farklı DeviceId atanmasını sağlayan dinamik kimlik yöneticisi.
- **Dinamik Transport Yöneticisi:** Runtime'da LAN ve Online arası mod değiştirildiğinde FishNet event pipeline aboneliklerini (`SubscribeToTransport`) korur.
- **SteamAuthManager:** Steamworks.NET üzerinden Steam hesabıyla giriş yapıp (Session Ticket alıp) bunu arka planda EOS hesabına eşleyen (External Auth) modül geliştirildi.
- **Dinamik UI Paneli:** OnGUI test arayüzü yeni özelliklere göre genişletildi ve Steam durum göstergesi eklendi.
- **MobileJoystick:** Sprite/asset gerektirmeden tamamen koddan oluşturulan, EventSystem tabanlı sanal dokunma joystick'i.
- **MobileInputManager:** Mobil/PC platformunu otomatik algılayıp joystick'i gösteren / gizleyen akıllı yönetici.

## Çözülen Hatalar (Bugs Fixed)
- [x] **Transport Swap Event Bug:** Çalışma anında LAN'dan EOS'a geçerken FishNet event aboneliklerinin kopması sorunu reflection ile çözüldü (`NetworkConnectionManager.SetTransport`).
- [x] **Steamworks Interface Version Bug:** Steamworks.NET v20+ ile kaldırılan `Constants` (SteamGameSearch vb.) hataları eklenti içerisinden temizlendi.
- [x] **SteamAppId Encoding Bug:** `steam_appid.txt` dosyasının UTF-8/UTF-16 olması nedeniyle Steam'in okuyamama sorunu ASCII formatına çekilerek çözüldü.

## Günlük / Notlar (Log)
- **18 Mart 2026:** Epic Developer Portal'da organizasyon ve product oluşturuldu.
- **20 Mart 2026:** Projenin belkemiği olan "6 Haneli Oda Kodu" sistemi hem İnternet (EOS) hem de Yerel Ağ (LAN) modları için sıfırdan yazıldı. Host tarafındaki FPS titremesi manuel Slerp Interpolation döngüsüyle çözüldü. Sistemin çalışır son hali GitHub'a yüklendi.
- **30 Mart 2026:** Geliştirme sürecine yeniden başlandı. Remote Client'ın P2P timeout yaşaması üzerine `NetworkConnectionManager`'a reflection tabanlı Transport-Swap sistemi eklendi, Host karakteri spawn edildi.
- **31 Mart 2026:** Client spawn hatası çözüldü. ParrelSync klonlarının aynı kimlikle bağlanmasını engelleyen `ParrelSyncEOSHelper` oluşturuldu. Event re-subscription ve EOS DeviceId sistemi başarıyla sisteme entegre edildi.
- **5 Nisan 2026:**
    - **Steam Entegrasyonu:** `Steamworks.NET` UPM ile projeye dahil edildi.
    - **SteamAuthManager:** Steam biletini (Ticket) alıp HEX stringe çeviren ve EOS Connect Interface'e aktaran köprü kodlandı.
    - **Otomatikleştirme:** `NetworkConnectionManager`'a sahnede eksik olan `SteamAuthManager`'ı otomatik oluşturma yeteneği eklendi.
    - **Hata Giderme:** Steamworks v1.59+ ile gelen interface version uyuşmazlığı ve `steam_appid.txt` encoding sorunları çözüldü.
    - **UI Revizyonu:** OnGUI paneli 250x450 boyutuna büyütülerek Steam durum göstergesi eklendi.
- **9 Nisan 2026:**
    - **Epic Display Name:** EOS UserInfo arayüzü ile Epic kullanıcı adı çekme sistemi eklendi. Steam/Epic/Anonim isim önceliklendirmesi kuruldu.
    - **Platform Çakışması Koruması (Launcher Detection):** Oyun Epic'ten açıldığında Steam modülünün devreye girmesini engelleyen komut satırı argüman algılama sistemi eklendi.
    - **Mobil Kontroller:** `MobileJoystick` (sanal dokunma joystick'i) ve `MobileInputManager` (platform algılama + Canvas otomatik oluşturucu) sıfırdan kodlandı. PlayerController'daki girdi sistemi Joystick > Gamepad > Klavye öncelik sırasına güncellendi.