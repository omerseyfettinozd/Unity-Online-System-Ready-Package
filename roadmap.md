# Yol Haritası (Roadmap)

## Faz 1: Temel Altyapı ve Kararlar (Phase 1: Foundation)
- **Hedef:** Unity 6000.3.9f1 sürümüne uygun ağ mimarisinin seçilmesi ve projeye entegrasyonu.
- **Çıktı:** Bağlantı kurulabilen boş bir sahne ve bağımsız (modüler) çalışan bir ağ yöneticisi sistemi.

## Faz 2: Çekirdek Ağ Senkronizasyonu ve Çoklu Erişim (Phase 2: Core Sync & Access)
- **Hedef:** İnternet üzerinden (EOS), Yerel Ağ üzerinden (LAN) veya aynı bilgisayarda bölünmüş ekran (Split-Screen) olarak bağlantı kurulabilmesi. Oyuncu nesnesinin ağ üzerinde oluşturulması ve hareket senkronizasyonu.
- **Çıktı:** Farklı bağlantı tiplerinde de (Online/LAN/Local) sorunsuz çalışan oyuncu oluşturma (Spawning) ve hareket sistemi.

## Faz 3: Çoklu Platform (Cross-Platform) & Modülerlik (Phase 3: Crossplay)
- **Hedef:** Sistemin Steam, Epic Games Store, Konsollar (Xbox/PS) ve Mobil (iOS/Android) cihazlarda ortak çalışabilmesi. Cihaza göre girdi (Input) yöntemlerinin dinamik değişmesi (Mobilde Joystick, PC'de Klavye/Oyun Kolu).
- **Çıktı:** Herhangi bir oyun türüne bağlı olmayan, farklı mağaza ve platformlardaki oyuncuları aynı sunucuda buluşturabilen (Crossplay) temiz bir altyapı klasörü.

## Faz 4: Dışa Aktarma ve Test (Phase 4: Export & Testing)
- **Hedef:** Oluşturulan bu online sistemin bir `.unitypackage` olarak dışa aktarılması ve sıfırdan oluşturulmuş farklı bir projeye sürükleyip bırakılarak çalışıp çalışmadığının kontrolü.
- **Çıktı:** Hata barındırmayan, kolay entegre edilebilir ve başka projelere aktarılabilir (Exportable) bir "Online System" aracı.
