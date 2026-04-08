# 👤 Yeni Karakter Oluşturma Rehberi (Custom Character Guide)

Bu rehber, bu asset paketini kullanan bir geliştiricinin kendi 3D karakter modelini sisteme nasıl entegre edeceğini adım adım açıklar. Sistem, profesyonel FishNet (V4) tahmini (Prediction) ve smoothing altyapısını kullandığı için sadece bir "modeli değiştirip bırakmak" yeterli değildir; birkaç teknik ayar yapılması gerekir.

---

## 🚀 Hazırlık: Mevcut Karakteri Şablon Olarak Kullanma

Sıfırdan bir karakter oluşturmak yerine, halihazırda ağ senkronizasyonu (Client-Side Prediction) ayarlanmış olan `Player.prefab` dosyasını çoğaltarak başlamak en güvenli yoldur.

1. **Dosyayı Bulun:** `Assets/OnlineSystemReady/Prefabs/Player.prefab` dosyasına sağ tıklayıp **Duplicate (Ctrl+D)** yapın.
2. **Yeniden Adlandırın:** Yeni objenize `MyCustomPlayer` gibi bir isim verin.

---

## 🎨 Görüntü (Visuals) Değiştirme

Karakterin görünümünü değiştirmek için prefab'ın içindeki hiyerarşiye sadık kalmalısınız.

1. **Prefab'ı Açın:** `MyCustomPlayer` prefab'ına çift tıklayarak içine girin.
2. **Eski Görseli Silin:** Prefab içindeki `Visuals` objesinin altındaki mevcut 3D modelleri (küp, kapsül vb.) silin.
3. **Kendi Modelinizi Ekleyin:** Kendi 3D modelinizi (FBX/Prefab) `Visuals` objesinin bir **child'ı (alt objesi)** olacak şekilde sürükleyip bırakın.
4. **Hizalama:** Modelinizin ayaklarının (Pivot) tam merkezde (0,0,0) olduğundan emin olun.

---

## 🛠️ PlayerController Ayarları (Kritik Adım)

Sistemimiz, Host (Sunucu) ekranındaki titremeyi (Jitter) gidermek için "Host Smoothing" özelliği içerir. Bu özelliğin çalışması için `Visuals` objesinin script'e tanıtılması gerekir.

1. Prefab'ın en üstündeki (Root) objeyi seçin.
2. `PlayerController` script bileşenini bulun.
3. **Visuals:** Bu alana, prefab içindeki `Visuals` isimli GameObject'i sürükleyip bırakın. 
   > [!IMPORTANT]
   > Eğer bu referansı boş bırakırsanız, karakter hareket ederken diğer oyuncuların (veya Host'un) ekranında titreme yapabilir.

---

## 🏃 Animasyon Senkronizasyonu (NetworkAnimator)

Eğer karakterinizde yürüme, koşma gibi animasyonlar varsa, bunları ağda senkronize etmek için şu adımları izleyin:

1. Prefab'a (Modelinizin olduğu yere değil, en üste) FishNet'in **Network Animator** bileşenini ekleyin.
2. Karakterinizin kendi **Animator** bileşenini bu `Network Animator` içindeki ilgili alana sürükleyin.
3. Animasyon parametrelerini (Örn: `Speed`, `IsJumping`) script üzerinden `animator.SetFloat()` ile güncellediğinizde FishNet bunları tüm oyunculara otomatik yayar.

---

## 🌐 Yeni Karakteri Ağda Doğurma (Spawning)

Oluşturduğunuz yeni karakteri oyunda kullanabilmek için FishNet'e bunu tanıtmalısınız.

1. **Prefab Objects Listesi:** Proje dizinindeki `Assets/DefaultPrefabObjects.asset` dosyasını seçin.
2. Yeni oluşturduğunuz `MyCustomPlayer` prefab'ını buradaki listeye sürükleyip ekleyin. (FishNet'in bu objeyi ağ üzerinden spawn edebilmesi için bu zorunludur).
3. **NetworkManager Ayarı:** Sahnedeki veya prefab klasöründeki `NetworkManager` prefab'ını seçin.
4. `PlayerSpawner` bileşenindeki `Player Prefab` kısmına kendi yeni karakterinizi sürükleyin.

---

## 💡 İpucu: Karakter Seçim Sistemi (İleri Seviye)

Eğer projeyi alan kişi birden fazla karakter arasından seçim yaptırmak istiyorsa:
- Tüm karakter prefab'larını `DefaultPrefabObjects` listesine eklemelidir.
- Oyuncu odaya bağlandığında, sunucuya hangi ID'li karakteri istediğini bir `ServerRpc` ile bildirmeli ve sunucu o ID'ye karşılık gelen prefab'ı spawn etmelidir.

> [!TIP]
> Mevcut `PlayerController` scripti modülerdir. Fiziksel hareket (CharacterController) ve ağ mantığı (CSP) ayrılmıştır. Sadece görseli (Visuals) değiştirerek saniyeler içinde yeni bir online karakter yaratabilirsiniz!
