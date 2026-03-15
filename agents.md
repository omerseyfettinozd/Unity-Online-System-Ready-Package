# Network Assets - AI Agents Guide

## Proje Amacı (Project Goal)
Bu projenin amacı, Unity 6000.3.9f1 sürümünde başka bir projeye kolayca aktarılabilecek (export edilebilir) modüler bir online/multiplayer sistem altyapısı kurmaktır.

## Temel Kurallar (Core Rules)
- **Sürüm:** Unity 6000.3.9f1
- **Modülerlik:** Sistem, başka projelere `.unitypackage` veya klasör kopyalama yoluyla kolayca aktarılabilecek şekilde diğer sistemlerden bağımsız (decoupled) tasarlanmalıdır.
- **Ağ Çözümü:** (Kullanılacak ağ çözümü belirlendiğinde buraya eklenecektir: örn. Netcode for GameObjects, Mirror, Photon Fusion vs.)
- **Kodlama Standartları:** Temiz kod ve SOLID prensiplerine uyulmalı, ağ senkronizasyonu (SyncVar, RPC) optimize edilmelidir.

## Dosya Yapısı ve Proje Yönetimi
Süreci yönetmek için aşağıdaki dosyalar kullanılır:
- `roadmap.md`: Uzun vadeli hedefler, fazlar ve proje kilometre taşları.
- `todo.md`: Yapılacaklar listesi, kısa vadeli ve anlık görevler.
- `progress.md`: Tamamlanan işlerin günlüğü ve projenin o anki durumu.
