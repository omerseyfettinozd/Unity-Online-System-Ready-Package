# Network Assets - AI Agents Guide

## Proje Amacı (Project Goal)
Bu projenin amacı, Unity 6000.3.9f1 sürümünde başka bir projeye kolayca aktarılabilecek (export edilebilir) modüler bir online/multiplayer sistem altyapısı kurmaktır.

## Temel Kurallar (Core Rules)
- **Sürüm:** Unity 6000.3.9f1
- **Modülerlik:** Sistem, başka projelere `.unitypackage` veya klasör kopyalama yoluyla kolayca aktarılabilecek şekilde diğer sistemlerden bağımsız (decoupled) tasarlanmalıdır.
- **Ağ Çözümü:** FishNet (V4) + Epic Online Services (EOS by PlayEveryWare).
- **Kodlama Standartları:** Temiz kod ve SOLID prensiplerine uyulmalı, ağ senkronizasyonu (SyncVar, RPC) optimize edilmelidir.

## 🚨 YAPAY ZEKA KURALI - Sürüm Uyumluluğu (Version Compatibility Rule) 🚨
***BU KURAL HİÇBİR ZAMAN İHLAL EDİLEMEZ:*** Projeye dahil edilen tüm eklentilerin (Örn: FishNet, EOS, ParrelSync, Unity Input System) kendilerine has API versiyonları vardır. Yapay zeka ajanları (AI), sisteme *HERHANGİ BİR* kod yazmadan önce paketin **GÜNCEL SÜRÜM DOKÜMANTASYONUNU (Release Notes / Version History)** kontrol etmek ZORUNDADIR. (Örn: FishNet V3 ile V4 arasındaki `CreateReconcile` farklılıkları gibi). "Varsayılan" veya eski bilgilerle kod yazılamaz; her zaman varlığın (asset) paketteki kurulu güncel versiyonuyla uyumlu kod yazılacaktır.

## Dosya Yapısı ve Proje Yönetimi
Süreci yönetmek için aşağıdaki dosyalar kullanılır:
- `roadmap.md`: Uzun vadeli hedefler, fazlar ve proje kilometre taşları.
- `todo.md`: Yapılacaklar listesi, kısa vadeli ve anlık görevler.
- `progress.md`: Tamamlanan işlerin günlüğü ve projenin o anki durumu.
