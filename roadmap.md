# Yol Haritası (Roadmap)

## Faz 1: Temel Altyapı ve Kararlar (Phase 1: Foundation)
- **Hedef:** Unity 6000.3.9f1 sürümüne uygun ağ mimarisinin seçilmesi ve projeye entegrasyonu.
- **Çıktı:** Bağlantı kurulabilen boş bir sahne ve bağımsız (modüler) çalışan bir ağ yöneticisi sistemi.

## Faz 2: Çekirdek Ağ Senkronizasyonu (Phase 2: Core Sync)
- **Hedef:** Bir ağ oturumu (Host/Client) başlatabilme, oyuncu (Player) nesnesinin ağ üzerinde oluşturulması (Spawning) ve hareket senkronizasyonu.
- **Çıktı:** Birden fazla istemcinin bağlanabildiği, oyuncuların birbirini görebildiği ve hareketlerin eşitlendiği temel proje.

## Faz 3: Modülerlik ve Gelişmiş Özellikler (Phase 3: Modularity)
- **Hedef:** Sistemin herhangi bir spesifik oyuna (RPG, FPS vb.) bağlı olmadan, jenerik bir "Ağ Çekirdeği (Network Core)" haline getirilmesi. Objeler arası etkileşim (RPC/SyncVar vb.) için örnek script'ler oluşturulması.
- **Çıktı:** Tüm modüler ağ scriptlerini içeren temiz bir dosya hiyerarşisi.

## Faz 4: Dışa Aktarma ve Test (Phase 4: Export & Testing)
- **Hedef:** Oluşturulan bu online sistemin bir `.unitypackage` olarak dışa aktarılması ve sıfırdan oluşturulmuş farklı bir projeye sürükleyip bırakılarak çalışıp çalışmadığının kontrolü.
- **Çıktı:** Hata barındırmayan, kolay entegre edilebilir ve başka projelere aktarılabilir (Exportable) bir "Online System" aracı.
