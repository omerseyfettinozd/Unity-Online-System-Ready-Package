# Yapılması Gereken Testler (QA & Testing Guide)

Bu belge, paketin ileri seviye entegrasyonlarını test ederken kullanılması gereken aşamaları ve özellikle uzak durulması gereken eksik işlemleri listeler.

## 1. Steam Entegrasyonu Testi

**DURUM:** ⚠️ *Kesinlikle Ön Hazırlık Gerektirir*

Bu ağ paketinin (Asset) içerisindeki Epic Online Services (EOS) mimarisi, doğuştan Steam Cross-Play desteğine sahiptir. Ancak projenizde resmi bir Steam SDK'sı yüklü değilken ayarları açmaya/test etmeye çalışmak motoru sonsuz bir çökme (crash) döngüsüne sokacaktır.

### 🚀 Steam Testine Başlamadan Önce Kurallar
Steam entegrasyonunu denemek istiyorsanız (veya bu paketi alan geliştirici deneyecekse) şu adımların tamamlandığından emin olunmalıdır:

1. Geliştiricinin Steam Partner portalından onaylanmış gerçek bir **Steam App ID** (Oyun Kimliği) numarası almış olması gerekir.
2. Projeye açık kaynaklı `Facepunch.Steamworks` veya `Steamworks.NET` SDK paketlerinden birisinin manuel import edilmiş olması gerekir.
3. Unity üst menüsünden `EOS Plugin -> EOS Configuration` (veya `Tools -> Epic Online Services`) ayar menüsüne girilmiş olmalıdır.
4. **FLAGS** sekmesinin altındaki `Integrated Platform Management Flags` kutucuğu **"Library Managed By SDK"** yapılarak Steam yetkileri devredilmiş olmalıdır.
5. Açılan alt menülerden ulaşılan platform bölümüne bizzat Steam Kimliği işlenmiş olmalıdır.

> [!WARNING]  
> **Kritik Uyarı:** Eğer oyununuz henüz Steam üzerinde yayımlanmadıysa ve elinizde resmi bir "App ID" yoksa, EOS ayarlarındaki **Integrated Platform Management Flags** menüsüne kesinlikle dokunmayın. Orada her zaman **`Nothing`** yazmak zorundadır. Aksi halde sistem arkaplanda Steam platformuna bağlanmaya çalışır ve bulamadığı için çöker.
