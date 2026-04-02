# Ağ Optimizasyon Rehberi (Temel Paket Uygulamaları)

Bu doküman, Unity + FishNet + EOS tabanlı modüler ağ sistemimizde, altyapıyı "her oyun türüne" kolayca uyarlanabilir, hafif ve performanslı tutmak için uygulanacak olan **temel ağ optimizasyonlarını** listeler.

---

## 1. Ağ Paket Kısıtlaması (Idle Network Sleep)

### Ne İşe Yarıyor?
Sistem şu anda oyuncunun tamamen hareketsiz olduğu ve oyuncu girdisinin (Input) sıfır olduğu durumlarda kestiği pakettir. Eğer kestiğiniz paketi tekrar kontrol etmez iseniz, karakteriniz olduğu yerde dursa dahi konum bilgisi sunucuya gönderilir hesaplanır ve harcatılır. Ağ paket kısıtlaması sadece konumda hareket veya girdide fiziksel bir değişim varsa paket gönderilmesini ifade eder.

### Nasıl Yapacağız?
`PlayerController.cs` betiğindeki `TimeManager_OnTick` ve `Move(...)` içerisinde;
```csharp
private void TimeManager_OnTick()
{
    if (base.IsOwner)
    {
        // Eğer hareket girdimiz sıfırsa ve karakterin mevcut hızı/ivmesi 0 ise
        if (_currentMoveInput == Vector2.zero) 
        {
             // Veri yollamayı atla ya da uykuda (idle) olduğunu belirten mini (boş) bir veri yolla
             return; 
        }
        
        MoveData data = new MoveData { MoveInput = _currentMoveInput };
        Move(data);
    }
}
```
gibi mantıksal kontroller ekleyerek ağı uykuya alma (sleeping) mekaniği entegre edilebilir. FishNet'te bu tip kesintilerden sonra eşitsizlik (desync) olmaması için iyi bir kontrol mekanizması olan son pozisyonu bir defaya mahsus sabitlemek gerekir.

### Faydası (Artısı)
* **Bant Genişliği Tasarrufu:** Lobide, menüde, veya savaş sırasında pusu kuran, pusuda bekleyen (hareket etmeyen) 10 oyuncu, ağ paket sayısını %95 oranında düşürtür.
* **Sunucu CPU Tasarrufu:** Gönderilmeyen paket, sunucuda Reconcile (düzeltme) hesabına da girmez. 

### Eksisi (Dezavantajı)
* **Geliştirme Zorluğu:** Sunucu karakterin haksız olduğunu sanabilecek kadar kesintiye uğrarsa (fiziksel fırlatmalar ve çarpışmalar dahil) paket gönderilmeyen aralıkta ışınlanmalar, asenkronluklar (Desynchronization) olabilir. Düşük yetenek sergileyen ve iyi yapılandırılmayan betiklerde sorunlara yol açar.

---

## 2. Görme Mesafesine Göre Ağ Budaması (Network Observers / Culling)

### Ne İşe Yarıyor?
Bir haritada çok farklı noktalarda birbirinden uzak insanların verilerinin birbirlerine aktarılmasının kesilmesine yarar. Temeli "Görmüyorsa hesaplama" mantığına dayanır.

### Nasıl Yapacağız?
1. Karakter (veya networke dahil edilen etkileşimli her obje) prefab'ının üzerindeki `Network Object` bileşenine gelin.
2. FishNet tarafından sağlanan **Distance Condition** (Mesafe Kuralı Şartı) adlı kuralı Observer bileşenlerine ekleyin.
3. Mesafe çapını belirleyin (örneğin kameranın max görüş mesafesi 100 metreyse radius: 100f olsun).
4. Bu objeler uzak mesafeye çıktıklarında diğer oyuncuların cihazına (Client'a) ağ verisi yollamayı (SyncVar, RPC dahil) durduracaktır ve ağda aktif olmayacaktır.

### Faydası (Artısı)
* **Çok Oyunculu Ölçeklendirme (Scalability):** 100 kişilik bir haritada herkes herkesin hareketini hesaplamaz. Geniş dünya haritalarında zorunluluktur. Bant genişliği dolmasını engeller, Clientlardaki FPS dropunu engeller.
* **Bölgesel Gizlilik (Cheat Protection):** Uzaktaki oyuncunun konum verisi yollanmadığı ve durduğu için Wallhack taramaları yapılmasını en azından yazılım tabanına kadar indirir (Sisteme verisi gelmediğinden çaldıramaz).

### Eksisi (Dezavantajı)
* **Oyun Mantığı Hataları:** Bir keskin nişancı 200 metreden ateş ettiğinde ama sizin "Distance Condition" ayarınız 100 metredeyse, o objeyi (adamı) haritada görmeyebilirsiniz ve birden bire hasar/mermi yemiş olabilirsiniz.
* **Sahne İçi Drop (Spike) Olasılığı:** Objeler görüş mesafesine gir-çık yaparken bir anda belirmeler ve ilk verilerin Client'ta yüklenmesinden dolayı anlık takılmalara (spawn drop) yol açabilir.

---

## 3. Güvenilmez Kanal (Unreliable Channel) Önceliği

### Ne İşe Yarıyor?
Bir verinin mutlaka ulaşması gereken yerlerde "Güvenilir", yolda kaybolsa da bir sonraki paketin durumu kurtaracağı anlarda "Güvenilmez" kanal (Channel.Unreliable) kullanılır. TCP vs UDP mimarisinin ağ düzeyindeki uygulamasıdır.

### Nasıl Yapacağız?
FishNet, hiçbir ayar belirtmezseniz paketleri standart olarak `Channel.Reliable` üzerinden (Kaybolana kadar bekle ve garantile mantığıyla) gönderir.
Bunu çok sık akan (update/tick benzeri) verilerde mutlaka Unreliable olarak işaretlemeliyiz. Mevcut `PlayerController.cs`'de yaptığımız gibi:
```csharp
// Sürekli akan "hareket" verisi, bir paket kaybolsa bile bir sonrakinde düzeleceği için UNRELIABLE olmalıdır.
[Replicate]
private void Move(MoveData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
{ ... }

// Ancak oyuncuya hasar (Damage) veya öldürme işlemi gibi sadece 1 kez işlenecek veriler RELIABLE olmalıdır.
[ServerRpc(Channel = Channel.Reliable)] // Zaten Channel varsayılan olarak budur.
public void CmdTakeDamage(int amount)
{ ... }
```

### Faydası (Artısı)
* **Gecikme (Lag) Önleme:** Arka arkaya atılan sürekli verilerde paket yolda düşerse Reliable (Güvenilir) kanal paketin gitmesi için beklerken akışı dondurup Ping patlamaları (lag spike) yaratır. Unreliable kullandığımızda oyun akışı, paketlerin kayıp telafisi (Loss Recovery) için durdurulmaz. Oyun hissiyatı asla bozulmaz. 

### Eksisi (Dezavantajı)
* Eğer Kritik bir paketi yanlışıkla Unreliable olarak atarsanız; o paket ağ dalgalanmalarına rast gelip kaybolursa ve bir daha yollanmazsa (örneğin kutuyu açan kişinin kutusu kendi ekranında açılırken diğer oyuncularda kutu kapalı olarak kalabilir) oyun döngüsü tamamen bozulup asenkron (Desync) batağına saplanabilir.
