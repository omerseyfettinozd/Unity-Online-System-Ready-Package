# To-Do List

## Acil (Kısa Vadeli) Görevler
- [x] **Host Visual Jitter:** Host ekranındaki senkronizasyon titremesinin Slerp ile kusursuz süzülmeye çevrilmesi.
- [x] **Online Koda Katıl (EOS):** Epic Games altyapısı üzerinden 6 haneli numerik kodla oda kurma/bulma.
- [x] **LAN Koda Katıl (UDP):** Aynı internetteki cihazların IP girmeden 6 haneli numerik kodla birbirini bulması.

## Gelecek (Orta Vadeli) Görevler
- [ ] Mobil uyumluluk: Joystick, Touch Area ve Action Butonlarının arayüze eklenmesi.
- [ ] Mobil girdilerin yeni Unity Input System ile köprülenmesi.

## Neredeyse Biten (Uzun Vadeli) Görevler
- [ ] Tam bağımsız, başkasına kopyalandığı an çalışan `.unitypackage` çıktısının (Export) alınması.

## Çözülen Hatalar (Bugs Fixed)
- [x] **EOS Remote Client Spawning:** ParrelSync clone'larında aynı `ProductUserId` alınması sorunu çözüldü (`ParrelSyncEOSHelper`).
- [x] **Transport Swap Event Bug:** Çalışma anında LAN'dan EOS'a geçerken FishNet event aboneliklerinin kopması sorunu reflection ile çözüldü (`NetworkConnectionManager.SetTransport`).