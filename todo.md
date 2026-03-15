# Yapılacaklar Listesi (To-Do List)

## Tamamlananlar
- [x] GitHub deposunun (repository) oluşturulup yerel projeye bağlanması ve ilk sürümün (commit/push) gönderilmesi.
- [x] Unity 6000.3.9f1 sürümü için en uygun multiplayer ağ çözümüne (örn: Netcode for GameObjects (NGO), Mirror, vb.) karar verilmesi. *(FishNet + EOS olarak seçildi)*

## Aşama 1: Paketlerin ve Altyapının Kurulumu (Networking Setup)
- [ ] **FishNet Networking** paketinin Unity Package Manager kullanılarak (Git URL ile) projeye indirilmesi ve kurulması.
- [ ] **Epic Online Services (EOS) SDK** ve **FishyEOS Transport** eklentisinin yüklenip ayarlanması.
- [ ] Kapsamlı multiplayer testleri yapabilmemiz için Unity Editörüne **ParrelSync** eklentisinin kurulması.
- [ ] Oyunu yönetecek temel **NetworkManager** prefab'ının oluşturulması ve sahneye (veya kalıcı bir "Bootstrap" sahnesine) yerleştirilmesi.

## Aşama 2: Temel Arayüz (UI) ve Bağlantı Mantığı
- [ ] **Main Menu Canvas:** Oyuncunun "Host (Oyunu Kur)" veya "Client (Oyuna Katıl)" diyebileceği basit bir UI yapısı oluşturulması.
- [ ] **Connection Manager:** Yazılan arayüzdeki butonlara tıklanınca EOS üzerinden bağlantıyı başlatacak ve FishNet'e entegre olacak basit bir C# yöneticisi (`NetworkConnectionManager.cs`) yazılması.

## Aşama 3: Karakter (Player) Senkronizasyonu ve Akıcılık
- [ ] **Player Prefab:** 2D ve 3D altyapıyı destekleyecek, ağda kopyalanabilir (spawneable) standart bir oyuncu prefab'ı (Kapsül) yaratılması.
- [ ] **Ağ Bileşenleri:** Oyuncuya `NetworkObject`, `NetworkTransform` ve gerekirse `PredictedRigidbody` gibi FishNet eşitleme scriptlerinin eklenmesi.
- [ ] **Client-Side Prediction:** Oyuncu hareket kodunu kontrol eden `PlayerController.cs` scriptinin; "Replicate" (Girdiyi İşle) ve "Reconcile" (Sunucuyla Düzelt) metotlarına uygun şekilde yazılması. (Bu sayede 0 gecikme hissi olacak).

## Aşama 4: Modülerleştirme ve Dışa Aktarma (Export)
- [ ] Tüm oluşturulan yapıların, sahnelerin ve kodların bir klasör (Örn: `NetworkCore/`) altında bağımsız toplanması.
- [ ] `NetworkCore` klasörünün sorunsuz bir şekilde başka projelerde çalışıp çalışmadığının test edilmesi ve `.unitypackage` olarak dışa aktarılması.
