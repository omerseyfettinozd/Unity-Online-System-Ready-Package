                  # Detaylı Uygulama Rehberi (Implementation Guide)

Bu doküman, `todo.md` dosyasında listelenen hedeflerin teknik olarak kod ve Unity Editor bazında *nasıl* uygulanacağını adım adım detaylandırmaktadır. Bu rehber AI ajanlarının ve geliştiricinin her görev için referans alacağı mimari kılavuzdur.

---

## 1. Paketlerin ve Altyapının Kurulumu (Networking Setup)

### FishNet Kurulumu
1. **İndirme Yöntemi:** FishNet, Unity Asset Store üzerinden veya GitHub'dan indirilerek projeye dahil edilecek. Hızlı versiyon takibi için FishNet'in `manifest.json` dosyasına eklenen GitHub linki üzerinden kurulması tercih edilecek.
2. **Yapılandırma:** FishNet kurulduğunda menüye `FishNet` adında bir sekme gelir. Buradan FishNet gereksinimlerinin yüklendiği ve hataların giderildiği doğrulanacak.

### Epic Online Services (EOS) & Platform Bağımsızlık (Cross-Platform)
1. **Dev Portal:** Epic Developer Portal üzerinden proje oluşturulacak (Client ID, Secret vb.).
2. **FishyEOS Transport:** FishNet ile ağ arasında köprü kuracak bu eklenti ile; Steam, Epic Games, konsollar (PlayStation, Xbox) ve Mobil (iOS/Android) oyuncuları **aynı lobide (Crossplay)** oynayabilecektir.
3. EOS, oyuncunun Steam veya Epic hesabından bağımsız bir arka plan zarı (Overlay) kurduğu için yayınlanan platform (Storefront) fark etmeksizin tüm oyuncular (P2P veya Relay üzerinden) birbirine bağlanabilir.
4. **Mobil (Android/iOS) Dengelemesi:** Mobil oyuncular için ekrana dokunmatik kontroller (On-Screen Joystick/Buttons) entegrasyonu Input System üzerinden "Eğer cihaz mobilse dokunmatik UI'ı aç" mantığıyla dahil edilecek.

### ParrelSync Test Eklentisi
1. Editörde birden fazla instance açarak host ve client arasındaki senkronizasyonu rahatça görmek için GitHub üzerinden `ParrelSync` Unity'e import edilecek.

### Steamworks.NET ve Steam Kimliği Entegrasyonu
2. (Opsiyonel) Steam oyuncularını EOS ağına "gerçek isimleriyle" sokmak için projenin `manifest.json` dosyasına Steamworks.NET bağımlılığı eklenmelidir. Sonrasında Epic Developer Portal üzerinden Identity Providers sekmesinden Steam App ID'si eklenmelidir. Oluşturulan `SteamAuthManager` scripti Steam kapalıysa ya da projede yoksa otomatik olarak (fallback) anonim EOS DeviceToken yöntemini kullanır.

### NetworkManager Prefab'ı Tasarımı
1. Boş bir GameObject oluşturulup adına `NetworkManager` verilecek.
2. Altına `NetworkManager` scripti eklenecek.
3. Oyuncu nesnesi oluşturulduğunda ağda doğmasını sağlamak için `PlayerSpawner` bileşeni de bu objeye eklenecek ve modüler kalması adına proje için açacağımız `NetworkCore/Prefabs` klasörüne sürüklenecek.

---

## 1.5 Hierarchy ve Inspector Kurulumu (Agent Referansı)
1. **NetworkManager Prefab'ı:**
   - Boş bir obje oluşturulacak (İsim: `NetworkManager`).
   - İçine `NetworkManager` bileşeni eklenecek.
   - İçine `Tugboat` (LAN) bileşeni eklenecek.
   - İçine `FishyEOS` (Online) bileşeni eklenecek ve Epic Portal'dan alınan kimlikler (Product ID, Sandbox ID) buraya girilecek.
   - `PlayerSpawner` bileşeni eklenip `NetworkManager`'a bağlanacak.
2. **OnlineSystemController:**
   - Boş bir obje (veya Manager objesinin alt objesi) oluşturulacak.
   - Kodladığımız `NetworkConnectionManager`, `SplitScreenManager` ve `MainMenuManager` scriptleri buraya atılacak.
   - Her scriptin inspector üzerindeki referansları birbirlerine (Tugboat, FishyEOS, UI Panelleri) sürüklenecek.

## 2. Temel Arayüz (UI) ve Bağlantı Mantığı (Connection Logic)

1. **Canvas Düzeni:** Sahneye bir Canvas eklenip ortasına "Online Bağlan (EOS)", "Yerel Ağ (LAN)" ve "Bölünmüş Ekran (Split-Screen)" butonları eklenecek.
2. **NetworkConnectionController.cs (Çoklu Ağ Yöneticisi):**
   - Bu script, FishNet'in `NetworkManager` referansını tutacak.
   - **LAN Modu:** Seçildiğinde, FishNet'in "Transport Manager" özelliği kullanılarak aktif transport `Tugboat` (FishNet'in yerel ağ transportu) olarak değiştirilecek ve ardından `StartConnection()` çağrılacak. IP adresi "localhost" veya yerel IP olacak.
   - **Online Modu (EOS):** Seçildiğinde, aktif transport `FishyEOS` olarak değiştirilip Epic sunucularına bağlanılacak.
   - **Split-Screen Modu:** Bu modda ağ bağlantısı başlatılmaz (veya gizli arka plan Host'u açılır). Oyuna sadece ikinci bir lokal Player Prefab'ı eklenir.

## 2.5 Split-Screen (Bölünmüş Ekran) ve Local Co-Op Altyapısı
1. **Input System:** Unity'nin yeni (veya eski) Input System'i kullanılarak `Player 1` (WASD) ve `Player 2` (Yön Tuşları veya Gamepad) ayrımları yapılacak.
2. **Kamera Bölme:** Oyuncu prefab'ı doğduğunda (Spawn), eğer Split-Screen modundaysak:
   - 1. Oyuncunun kamerasının `Viewport Rect` değeri Y ekseninde `H: 0.5` ve `Y: 0.5` (Üst yarı) yapılacak.
   - 2. Oyuncunun kamerasının değeri `H: 0.5` ve `Y: 0` (Alt yarı) olarak ayarlanacak.
3. Bu işlemler için yazılacak olan `SplitScreenManager.cs` scripti, ekrandaki oyuncu sayısına göre kameraları dinamik veya yatay/dikey bölecek şekilde modüler tasarlanacak.

---

## 3. Karakter (Player) Senkronizasyonu ve Akıcılık

Kusursuz akıcılıktaki CS:GO hissiyatını vermek için Client-Side Prediction mimarisini burada uygulayacağız.

1. **Prefab:** Oyuncuyu temsil eden Kapsül/Küp nesnesi oluşturulacak. Projede ileride animasyon vs eklenebilmesi için pivot noktası ayaklarda olacak şekilde ayarlanacak.
2. **Scriptler - NetworkObject:** FishNet'in ağda bu objeyi tanıması için `NetworkObject` bileşeni zorunludur, eklenecek. Ve `PlayerSpawner` dizisine eklenecek.
3. **PlayerController.cs (CSP Yazımı):**
   FishNet'in tahminleme yapısı `NetworkBehaviour` üzerinden şu şekilde kurulacaktır:
   - **`OnTick` ve `OnPostTick` Kullanımı:** Unity'nin `Update`'i yerine, FishNet ağ sistemiyle eş zamanlı çalışan tick mekanizması kullanılacak.
   - **`Replicate` Metodu:** Oyuncu fareye veya WASD tuşlarına bastığında (girdi verisi - Player Input), önce sadece oyuncunun makinesinde (Client) bu metod çağrılıp karakter fiziksel olarak hareket ettirilecek. Aynı anda bu tuş verisi gizlice Server'a gönderilecek.
   - **`Reconcile` Metodu:** Server da gelen WASD tuşlarıyla karakteri kendi tarafında hareket ettirip sonuç pozisyonunu bize geri dönecek. Bizim client, kendi tahmini pozisyonuyla Server'dan gelen pozisyonu kıyaslayacak. Eğper sapma (error) varsa, karakter Server'ın dediği yere usulca çekilecek (smoothing edilerek).
   - Bu sayede 0 milisaniye gecikmeli bir hareket hissi sağlanacak.

---

## 4. Modülerleştirme (Decoupling) ve Export İçin Hazırlık

Proje amacına uygun olarak, bu yazdığımız online sistem tamamen "Tak ve Çalıştır (Plug&Play)" mantığında olmalıdır.
1. Scriptler ve Assetler oluşturulurken, dış müdahaleler engellenmeli, sistemin çalışması sadece 1 adet `NetworkManager` prefab'ını sahneye koymaya indirilmelidir.
2. Tüm dosyalar `Assets/OnlineSystemReady/` gibi bir ana klasör altında toplanıp referansları buradan verilmelidir (Hata almamak için `Player` prefab'ı dahil her şey bu lokasyonda durmalıdır).
3. Geliştirme sonunda klasöre sağ tıklanıp "Export Package" seçeneği ile temiz bir `.unitypackage` oluşturulması planlanmaktadır. 
