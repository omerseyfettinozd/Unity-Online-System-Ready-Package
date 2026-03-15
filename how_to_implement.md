# Detaylı Uygulama Rehberi (Implementation Guide)

Bu doküman, `todo.md` dosyasında listelenen hedeflerin teknik olarak kod ve Unity Editor bazında *nasıl* uygulanacağını adım adım detaylandırmaktadır. Bu rehber AI ajanlarının ve geliştiricinin her görev için referans alacağı mimari kılavuzdur.

---

## 1. Paketlerin ve Altyapının Kurulumu (Networking Setup)

### FishNet Kurulumu
1. **İndirme Yöntemi:** FishNet, Unity Asset Store üzerinden veya GitHub'dan indirilerek projeye dahil edilecek. Hızlı versiyon takibi için FishNet'in `manifest.json` dosyasına eklenen GitHub linki üzerinden kurulması tercih edilecek.
2. **Yapılandırma:** FishNet kurulduğunda menüye `FishNet` adında bir sekme gelir. Buradan FishNet gereksinimlerinin yüklendiği ve hataların giderildiği doğrulanacak.

### Epic Online Services (EOS) & FishyEOS
1. **Dev Portal:** Epic Developer Portal üzerinden proje oluşturulacak (Client ID, Client Secret, Product ID vb. bilgileri alınacak).
2. **FishyEOS Transport:** FishNet ile EOS arasındaki köprüyü sağlayan "FishyEOS" paketi GitHub'dan (veya unitypackage olarak) entegre edilecek.
3. Nakil aracısı (Transport layer), varsayılan `Tugboat` yerine `FishyEOS` olarak seçilip, Epic portalından alınan ID'ler "NetworkManager" prefab'ı üzerindeki transport kısmına yazılacak.

### ParrelSync Test Eklentisi
1. Editörde birden fazla instance açarak host ve client arasındaki senkronizasyonu rahatça görmek için GitHub üzerinden `ParrelSync` Unity'e import edilecek.

### NetworkManager Prefab'ı Tasarımı
1. Boş bir GameObject oluşturulup adına `NetworkManager` verilecek.
2. Altına `NetworkManager` scripti eklenecek.
3. Oyuncu nesnesi oluşturulduğunda ağda doğmasını sağlamak için `PlayerSpawner` bileşeni de bu objeye eklenecek ve modüler kalması adına proje için açacağımız `NetworkCore/Prefabs` klasörüne sürüklenecek.

---

## 2. Temel Arayüz (UI) ve Bağlantı Mantığı (Connection Logic)

1. **Canvas Düzeni:** Sahneye bir Canvas eklenip ortasına "Host (Server+Client)" ve "Join (Client)" butonları eklenecek.
2. **NetworkConnectionController.cs:** Bu adla oluşturulacak script, "Host" butonuna basıldığında FishNet'in `InstanceFinder.ServerManager.StartConnection()` methodunu, "Join" için `InstanceFinder.ClientManager.StartConnection()` methodunu (EOS lobi ID'sine veya P2P koduna/şifresine bağlanarak) çağıracak. Script `NetworkManager`'a bağlanıp prefab güncellenecek.

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
