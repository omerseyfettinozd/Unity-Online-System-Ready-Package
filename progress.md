# Proje İlerleme Durumu (Progress Log)

## Mevcut Durum (Current Status)
- [x] NetworkManager ve Player prefab'larının otomatik oluşturulması (`NetworkSetupWizard.cs`) kodlandı.
- [x] Host ekranında Client kapsüllerindeki titreme Slerp ile kusursuz süzülmeye (Jitter Fix) çevrildi.
- [x] Epic Games (EOS) ve LAN (Yerel Ağ) için IP girmeden tamamen "6 haneli sayısal kod" ile odaya katılma sistemi (Matchmaking) tamamlandı.
- [ ] Sıradaki Aşama: Mobil kontrollerin (Joystick/Touch) arayüze eklenmesi.

## Tamamlanan Özellikler (Completed Features)
- **EOSMatchmakingManager:** Epic SDK Lobi yapısını kullanan 6 haneli kod sistemi.
- **LANMatchmakingManager:** UDP Broadcast radyo dalgaları ile IP girmeyi bitiren özel sistem.
- **MainMenuManager:** 6 haneli kodların girildiği ve iki ağ modunun birleştirildiği saf arayüz.
- **ParrelSyncEOSHelper:** Aynı cihazdaki klonların (ParrelSync) EOS ağına bağlanabilmesi için otomatik olarak farklı DeviceId atanmasını sağlayan dinamik kimlik yöneticisi.
- **Dinamik Transport Yöneticisi:** Runtime'da LAN ve Online arası mod değiştirildiğinde FishNet event pipeline aboneliklerini (`SubscribeToTransport`) korur.

## Günlük / Notlar (Log)
- **18 Mart 2026:** Epic Developer Portal'da organizasyon ve product oluşturuldu.
- **20 Mart 2026:** Projenin belkemiği olan "6 Haneli Oda Kodu" sistemi hem İnternet (EOS) hem de Yerel Ağ (LAN) modları için sıfırdan yazıldı. Host tarafındaki FPS titremesi manuel Slerp Interpolation döngüsüyle çözüldü. Sistemin çalışır son hali GitHub'a yüklendi.
- **30 Mart 2026:** Geliştirme sürecine yeniden başlandı. Remote Client'ın P2P timeout yaşaması üzerine `NetworkConnectionManager`'a reflection tabanlı Transport-Swap sistemi eklendi, Host karakteri spawn edildi.
- **31 Mart 2026:** Client spawn hatası çözüldü. ParrelSync klonlarının aynı kimlikle bağlanmasını engelleyen `ParrelSyncEOSHelper` oluşturuldu. Event re-subscription ve EOS DeviceId sistemi başarıyla sisteme entegre edildi.