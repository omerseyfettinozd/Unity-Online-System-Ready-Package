# Proje İlerleme Durumu (Progress Log)

## Mevcut Durum (Current Status)
- [x] Proje yönetim ve dokümantasyon dosyaları (`agents.md`, `progress.md`, `todo.md`, `roadmap.md`) oluşturuldu.
- [x] Ağ altyapısı (Networking Framework) araştırması ve kurulumu tamamlandı.
- [x] EOS SDK (v6.0.2) ve FishyEOS Transport (v0.0.6) konfigüre edildi.
- [ ] NetworkManager prefab'ının oluşturulması aşamasına geçilecek.

## Tamamlanan Özellikler (Completed Features)
- FishNet V4, EOS Plugin v6.0.2, FishyEOS v0.0.6, ParrelSync paketleri kurulu.
- Epic Developer Portal'da product, client ve policy oluşturuldu.
- Unity EOS Configuration penceresi üzerinden tüm credential'lar (Product ID, Client ID/Secret, Sandbox ID, Deployment ID) girildi.
- `Assets/StreamingAssets/EOS/` altında platform config dosyaları oluştu.

## Günlük / Notlar (Log)
- **15 Mart 2026:** AI ajanları için rehber dosyalar oluşturuldu ve Unity 6000.3.9f1 sürümünde geliştirilecek modüler ağ sisteminin planlamasına başlandı.
- **15 Mart 2026:** Ağ paketi araştırması yapıldı. Geliştiriciye sıfır sunucu maliyeti çıkaracak, Client-Side Prediction ve Lag Compensation destekleyen, Steam/Epic crossplay uyumlu yapı araştırıldı. En iyi adaylar: FishNet + Epic Online Services (EOS) ve Unity Netcode for GameObjects (NGO) + EOS olarak belirlendi.
- **15 Mart 2026:** Pürüzsüz geliştirme süreci için projeye Git versiyon kontrol sistemi eklendi (`git init`). Unity gereksiz dosyalarının buluta yüklenmesini önlemek için `.gitignore` dosyası oluşturuldu. Sistem başarıyla GitHub'a `main` branch'i üzerinden aktarıldı (push).
- **18 Mart 2026:** Epic Developer Portal'da organizasyon ve product (`NetworkAssetsTest`) oluşturuldu. `Peer2PeerPolicy` client policy ve `NetworkAssets` client eklendi. Unity'de EOS Configuration penceresi üzerinden tüm credential'lar girildi ve kaydedildi.
