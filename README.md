# Unity Online System Ready Package

**Gelişmiş, Modüler ve Performanslı Çapraz Platform Ağ Altyapısı**

Bu proje, Unity 6000.3.9f1 sürümünde başka projelere kolayca `.unitypackage` olarak aktarılabilecek profesyonel bir online/multiplayer sistem altyapısıdır.

## 🚀 Öne Çıkan Özellikler (Mevcut Altyapı)

### 1. Epic Online Services (EOS) & FishNet V4 Entegrasyonu
- **Sıfır Port Yönlendirme:** Oyuncular modem ayarlarıyla uğraşmadan "6 Haneli Davet Kodları" ile birbirlerine internet üzerinden doğrudan bağlanabilirler. 
- **Yerleşik Cross-Play (Çapraz Oyun):** Altyapı EOS P2P mimarisi sayesinde Steam SDK ile anında çapraz oyun olarak entegre olabilir (Riskler için projedeki test belgesine bakınız).

### 2. Event-Driven (Olay Tabanlı) Yüksek Performanslı Senkronizasyon
- İşlemciyi sömüren `Update` verileri tamamen kaldırılarak yerine **FishNet SyncVar(OnChange)** mimarisi eklenmiştir. Paket, verileri (ör: isim, yetenek) yalnızca "değiştiği an" yakalayarak %0'a yakın işlemci tüketir.

### 3. Sunucu Güvenliği (Anti-Spam / Anti-Flood Koruması)
- Kötü niyetli oyuncuların (Hile/Macro) saniyede binlerce veri talebi atarak odayı (Host'u) kitlemesinin önüne geçebilmek adına tüm veri aktarım merkezlerine (`[ServerRpc]`) **Zaman Kilidi (Cooldown)** korumaları dahil edilmiştir.

### 4. Zarif Ağ Kopuşu (Graceful Disconnect & No-Zombies)
- Bir oyuncunun aniden interneti kesildiğinde veya `Alt+F4` çektiğinde sunucuda sonsuza kadar açık kalan hayalet lobilerin (Zombie Lobbies) oluşması engellenmiştir. Bağlantı dedektörleri kopuş anında bulut sistemine komut yollayarak oyun odasını tertemiz bir şekilde sonlandırır.

---

## 🛠️ Sürüm Notları (Release Notes)

### v0.2.1 - Security & Testing Update
- **Eklenenler:** İstemci taraflı sunucu iletişim çağrılarına (`PlayerController.cs`) ağ seli koruması eklendi.
- **Eklenenler:** Steam Cross-Play Entegrasyonunun risklerini ve Unity çökme uyarılarını detaylandıran `project_testing_guide.md` dokümanı projeye işlendi.
- **Düzeltilenler:** Projedeki "Transform Optimizasyon" zorunluluğu, varlığı başka projelere uyarlayacak geliştiricileri kısıtlamamak (Esneklik vizyonu) adına listeden iptal edildi.

### v0.2.0 - Core Engine Optimization Update
- **Eklenenler:** Event-Driven ağ kancası altyapısı ve NameTag mantıksal zemini atıldı.
- **Eklenenler:** Epic Games eşleştirmelerine Lobi Yıkım komutları (`LeaveOrDestroyLobby`) asenkron FishNet döngülerine entegre edildi.
- **Düzeltilenler:** Eski sürüm kalan Unity API'leri temizlendi. Konsoldaki tüm Obsolete uyarıları giderilerek projedeki nesne taramaları `FindFirstObjectByType(FindObjectsSortMode.None)` komutuna dönüştürülüp hızlandırıldı.
- **Düzeltilenler:** `OnStartNetwork` metodundaki İstemci/Sunucu yarışından (Race Condition) kaynaklanan FishNet geç uyanma (Client Not Active) hatası kalıcı olarak düzeltildi.

## 📄 Dokümantasyon Dosyaları
Proje içerisinde sistemi öğrenmek isteyen geliştiriciler için detaylı Markdown kılavuzları bulunur:
- `roadmap.md` & `todo.md` : Geliştirme haritaları.
- `network_optimization_guide.md` : Ağ senkronizasyonu hakkında teknik tavsiyeler.
- `project_testing_guide.md` : Steam yetki girişleri ve SDK test adımları.
