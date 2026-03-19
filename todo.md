# Yapılacaklar Listesi (To-Do List)

## Tamamlananlar
- [x] GitHub deposunun (repository) oluşturulup yerel projeye bağlanması ve ilk sürümün (commit/push) gönderilmesi.
- [x] Unity 6000.3.9f1 sürümü için en uygun multiplayer ağ çözümüne (örn: Netcode for GameObjects (NGO), Mirror, vb.) karar verilmesi. *(FishNet + EOS olarak seçildi)*

## Aşama 1: Paketlerin ve Altyapının Kurulumu (Networking Setup)
- [x] **FishNet Networking** paketinin Unity Package Manager kullanılarak (Git URL ile) projeye indirilmesi ve kurulması.
- [x] **Epic Online Services (EOS) SDK** ve **FishyEOS Transport** eklentisinin yüklenip ayarlanması. *(EOS Plugin v6.0.2, FishyEOS v0.0.6 kuruldu; Developer Portal credential'ları Unity'ye girildi)*
- [x] Kapsamlı multiplayer testleri yapabilmemiz için Unity Editörüne **ParrelSync** eklentisinin kurulması.
- [x] Oyunu yönetecek temel **NetworkManager** prefab'ının oluşturulması ve sahneye (veya kalıcı bir "Bootstrap" sahnesine) yerleştirilmesi.

## Aşama 2: Temel Arayüz (UI) ve Bağlantı Mantığı
- [x] **Main Menu Canvas:** Oyuncunun "Online Bağlan (EOS)", "Yerel Ağda Kur/Bağlan (LAN)" veya "Bölünmüş Ekran (Split-Screen)" seçeneklerini seçebileceği gelişmiş bir UI oluşturulması.
- [x] **Connection Manager:** Seçilen ağ moduna göre altyapıyı değiştiren C# yöneticisi (`NetworkConnectionManager.cs`). (Örn: LAN seçilirse FishNet'in varsayılan Tugboat transportunu, Online seçilirse FishyEOS transportunu kullanması.)
- [x] **Split-Screen Yöneticisi:** Tek ekranda birden fazla oyuncu eklendiğinde (Local Co-Op), ekranı ikiye bölen ve girdileri (Input System) oyunculara göre ayıran bir kamera/arayüz yöneticisi yazılması.

## Aşama 3: Karakter (Player) Senkronizasyonu ve Akıcılık
- [x] **Player Prefab:** 2D ve 3D altyapıyı destekleyecek, ağda kopyalanabilir (spawneable) standart bir oyuncu prefab'ı (Kapsül) yaratılması.
- [x] **Ağ Bileşenleri:** Oyuncuya `NetworkObject`, `NetworkTransform` ve gerekirse `PredictedRigidbody` gibi FishNet eşitleme scriptlerinin eklenmesi.
- [x] **Client-Side Prediction:** Oyuncu hareket kodunu kontrol eden `PlayerController.cs` scriptinin; "Replicate" (Girdiyi İşle) ve "Reconcile" (Sunucuyla Düzelt) metotlarına uygun şekilde yazılması. (Bu sayede 0 gecikme hissi olacak).
- [x] **Cross-Platform Girdi (Input):** Klavye/Mouse (PC), Gamepad (Konsol/PC) ve Dokunmatik Ekran (Mobil Joystick) kontrollerinin tek bir PlayerController'a entegre edilmesi.

## Aşama 4: Modülerleştirme ve Dışa Aktarma (Export)
- [x] Tüm oluşturulan yapıların, sahnelerin ve kodların bir klasör (Örn: `NetworkCore/` veya `OnlineSystemReady/`) altında bağımsız toplanması.
- [ ] `NetworkCore` klasörünün sorunsuz bir şekilde başka projelerde çalışıp çalışmadığının test edilmesi ve `.unitypackage` olarak dışa aktarılması.
