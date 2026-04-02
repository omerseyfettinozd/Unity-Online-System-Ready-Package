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
- [x] **Transport Swap Event Bug:** Çalışma anında LAN'dan EOS'a geçerken FishNet event aboneliklerinin kopması sorunu reflection ile çözüldü (`NetworkConnectionManager.SetTransport`).

## Gelecek Planları / Temel Paket Ağ Optimizasyonları (Notlar)
- [x] **Event-Driven Ağ İletişimi:** Verilerin Update içerisinde kontrolü bırakılıp, performans için `[SyncVar(OnChange = nameof(Fonk))]` kanca (hook) metodlarına geçildi. (İlk örneği NameTag için yapıldı).
- [ ] **Gereksiz Transform İptalleri:** Ağ üzerindeki prefablarda, kullanılmayan Scale (Boyut) ve gereksiz Eksen Rotasyon güncellemeleri FishNet `NetworkTransform` ayarlarından tamamen kapatılıp bant genişliği tasarrufu kitlenecek.
- [ ] **RPC Spam Koruması:** Sunucu çöküşlerinin (Flood/Spam) önüne geçmek adına `[ServerRpc]` ile gelen tüm girdi/üretim komutlarına minik `Time.time` kaba bekleme süreleri (Cooldown) eklenecek.
## Paketi Tamamlayacak Temel Vitrin Özellikleri (UI & Lobi)
- [ ] **Ping ve Bağlantı UI:** Kullanıcı deneyimini güçlendirmek için anlık Ping (ms) gecikme süresinin ekrana yansıtılması.
- [ ] **Karakter İsim (NameTag) Senk.:** Epic Games üzerindeki kullanıcı adlarının objelerin üstünde (`[SyncVar]`) tüm oyuncularla senkronize olarak görünmesi.
- [x] **Zarif Ağ Kopuşu (Graceful Disconnect):** Modüler bir paket kuralı olarak; Host oyundan ayrıldığında veya internet koptuğunda arka planda otomatik lobi temizliği kodlandı.
- [ ] **Hazır Lobi (Ready Room):** Kod girildiğinde direkt aksiyona düşmek yerine oyuncuların bir bekleme odasında (Lobby) toplanıp Host'un `SceneManager` ile toplu bölüm başlattığı köprünün kurulması.