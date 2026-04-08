using System.Collections;
using UnityEngine;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices;
using FishNet;
using System.Reflection;

namespace OnlineSystemReady.Core
{
    /// <summary>
    /// Epic Games (EOS) Lobby Yöneticisi: IP karmasını kaldırır ve 6 haneli kod ile eşleştirme sağlar.
    /// </summary>
    public class EOSMatchmakingManager : MonoBehaviour
    {
        public static EOSMatchmakingManager Instance;
        private LobbyInterface _lobbyInterface;
        private ProductUserId _localUserId;
        private string _currentLobbyId;
        
        [Header("Geri Bildirimler (Test & UI)")]
        public string currentStatus = "Bekleniyor...";
        public string currentRoomCode = "";

        private bool _isLobbyOwner = false;
        
        public string EpicDisplayName { get; private set; } = "";
        private EpicAccountId _localEpicAccountId;



        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // Güvenlik Kamerası: Epic Games arka plan çekirdeği (EOSManager) sahnede var mı? Yoksa zorla ekle!
            if (UnityEngine.Object.FindFirstObjectByType<PlayEveryWare.EpicOnlineServices.EOSManager>() == null)
            {
                gameObject.AddComponent<PlayEveryWare.EpicOnlineServices.EOSManager>();
            }
        }

        // --- GİRİŞ (Auth) / Yeni Cihaz Kaydı ---
        /// <summary>
        /// ParrelSync clone'da DeviceId sıfırladıktan sonra EOS Connect'e giriş yapar.
        /// Clone tespiti yapılmazsa doğrudan giriş yapar.
        /// </summary>
        private IEnumerator LoginToEOSCoroutine(System.Action onSuccess)
        {
            // ParrelSync clone'da DeviceId sıfırla (farklı ProductUserId almak için)
            if (ParrelSyncEOSHelper.IsClone())
            {
                Debug.Log("[EOSMatchmaking] ParrelSync clone tespit edildi, DeviceId sıfırlanıyor...");
                yield return ParrelSyncEOSHelper.DeleteAndRecreateDeviceId();
            }

            // --- Steam Kimlik Doğrulaması (Eğer açıksa ve varsa) ---
            if (SteamAuthManager.Instance != null && SteamAuthManager.Instance.IsSteamAvailable)
            {
                bool steamDone = false;
                bool steamSuccess = false;
                
                currentStatus = "Steam ile EOS'a Giriş Yapılıyor...";
                
                yield return SteamAuthManager.Instance.LoginViaEOS(
                    (userId) => 
                    { 
                        _localUserId = userId; 
                        steamSuccess = true; 
                        steamDone = true; 
                    },
                    (error) => 
                    { 
                        Debug.LogWarning("[EOSMatchmaking] Steam auth başarısız, DeviceToken'a düşülüyor: " + error); 
                        steamDone = true; 
                    }
                );
                
                // Steam auth sonlanana kadar bekle
                while (!steamDone) yield return null;
                
                // Eğer Steam başarılırsa, doğrudan lobi arayüzünü alıp devam et (DeviceToken'i es geç)
                if (steamSuccess)
                {
                    _lobbyInterface = EOSManager.Instance.GetEOSPlatformInterface().GetLobbyInterface();
                    onSuccess?.Invoke();
                    yield break; // Coroutine'i bitir
                }
            }

            // Normal EOS giriş akışı (Steam kapalıysa veya hata verdiyse - Fallback)
            currentStatus = "EOS Giriş Yapılıyor (DeviceToken)...";
            string playerName = "Player_" + Random.Range(100, 999);

            bool loginDone = false;
            EOSManager.Instance.StartConnectLoginWithDeviceToken(playerName, (loginInfo) => 
            {
                if (loginInfo.ResultCode == Result.Success) 
                {
                    _localUserId = loginInfo.LocalUserId;
                    _lobbyInterface = EOSManager.Instance.GetEOSPlatformInterface().GetLobbyInterface();
                    
                    // Connect girişi başarılıysa, eğer bir Epic hesabı bağlıysa ismini çekmeye çalış
                    QueryEpicDisplayName();

                    loginDone = true;
                    onSuccess?.Invoke();
                }
                else if (loginInfo.ResultCode == Result.InvalidUser || loginInfo.ResultCode == Result.NotFound)
                {
                    currentStatus = "Yeni Cihaz Kaydı Açılıyor...";
                    var connectInterface = EOSManager.Instance.GetEOSPlatformInterface().GetConnectInterface();
                    var createOptions = new Epic.OnlineServices.Connect.CreateDeviceIdOptions() { DeviceModel = SystemInfo.deviceModel };
                    
                    connectInterface.CreateDeviceId(ref createOptions, null, (ref Epic.OnlineServices.Connect.CreateDeviceIdCallbackInfo createInfo) => 
                    {
                        if (createInfo.ResultCode == Result.Success)
                        {
                            currentStatus = "Kayıt Başarılı, Giriş Bekleniyor...";
                            // Kayıt başarılı, tekrar giriş isteği yolluyoruz
                            EOSManager.Instance.StartConnectLoginWithDeviceToken(playerName, (retryInfo) => 
                            {
                                if (retryInfo.ResultCode == Result.Success)
                                {
                                    _localUserId = retryInfo.LocalUserId;
                                    _lobbyInterface = EOSManager.Instance.GetEOSPlatformInterface().GetLobbyInterface();
                                    onSuccess?.Invoke();
                                }
                                else
                                {
                                    currentStatus = "Giriş Başarısız (Tekrar): " + retryInfo.ResultCode;
                                }
                                loginDone = true;
                            });
                        }
                        else
                        {
                            currentStatus = "Cihaz Kaydı Başarısız: " + createInfo.ResultCode;
                            loginDone = true;
                        }
                    });
                }
                else
                {
                    currentStatus = "Giriş Başarısız: " + loginInfo.ResultCode;
                    loginDone = true;
                }
            });

            // Giriş tamamlanana kadar bekle (coroutine bağlamında)
            while (!loginDone)
                yield return null;
        }

        // --- HOST (Oda Kurucu) ---
        public void StartHostEOS()
        {
            StartCoroutine(LoginToEOSCoroutine(() => 
            {
                CreateLobby();
            }));
        }

        private void CreateLobby()
        {
            _isLobbyOwner = true;
            currentStatus = "Oda Kuruluyor...";
            var createOptions = new CreateLobbyOptions()
            {
                LocalUserId = _localUserId,
                MaxLobbyMembers = 4,
                PermissionLevel = LobbyPermissionLevel.Publicadvertised,
                PresenceEnabled = false,
                BucketId = "Matchmaking"
            };

            _lobbyInterface.CreateLobby(ref createOptions, null, (ref CreateLobbyCallbackInfo info) => 
            {
                if (info.ResultCode != Result.Success) {
                    currentStatus = "Oda Kurulamadı!";
                    return;
                }
                _currentLobbyId = info.LobbyId;
                
                // --- 6 Haneli Oda Kodu Belirleme ---
                string roomCode = Random.Range(100000, 999999).ToString();
                currentRoomCode = roomCode;

                Debug.Log("[EOSMatchmaking] İnternet (Epic Games) Odası Başarıyla Kuruldu!");
                Debug.Log("<color=green>--- BİRİNCİL EOS DAVET KODU: " + roomCode + " ---</color>");
                
                var attrData = new AttributeData() { Key = "RoomCode", Value = roomCode };
                var modOptions = new UpdateLobbyModificationOptions() { LobbyId = _currentLobbyId, LocalUserId = _localUserId };
                _lobbyInterface.UpdateLobbyModification(ref modOptions, out LobbyModification modHandle);
                
                var addAttrOptions = new LobbyModificationAddAttributeOptions() { Attribute = attrData, Visibility = LobbyAttributeVisibility.Public };
                modHandle.AddAttribute(ref addAttrOptions);

                // Client'in kime bağlanacağını bilmesi için Host ID'sini de lobiye ekliyoruz
                var hostData = new AttributeData() { Key = "Host_ID", Value = _localUserId.ToString() };
                var addHostOptions = new LobbyModificationAddAttributeOptions() { Attribute = hostData, Visibility = LobbyAttributeVisibility.Public };
                modHandle.AddAttribute(ref addHostOptions);
                
                var updateOptions = new UpdateLobbyOptions() { LobbyModificationHandle = modHandle };
                _lobbyInterface.UpdateLobby(ref updateOptions, null, (ref UpdateLobbyCallbackInfo updateInfo) => 
                {
                    currentStatus = "Oda Kuruldu! Kod: " + roomCode;

                    // FishyEOS'un RemoteProductUserId kısmını kendimiz (Host) olarak ayarlayıp FishNet ağını açıyoruz
                    SetFishyEOSRemoteId(_localUserId.ToString());
                    NetworkConnectionManager.Instance.StartOnlineHost();
                });
            });
        }

        // --- CLIENT (Katılımcı) ---
        public void StartClientEOS(string roomCode)
        {
            if (string.IsNullOrEmpty(roomCode) || roomCode.Length != 6)
            {
                currentStatus = "Lütfen 6 haneli kodu eksiksiz girin!";
                return;
            }

            StartCoroutine(LoginToEOSCoroutine(() => 
            {
                SearchLobby(roomCode);
            }));
        }

        private void SearchLobby(string roomCode)
        {
            currentStatus = "Oda Aranıyor: " + roomCode;
            var searchOptions = new CreateLobbySearchOptions() { MaxResults = 10 };
            _lobbyInterface.CreateLobbySearch(ref searchOptions, out LobbySearch searchHandle);
            
            // Lobi Arama Filtresi: Sadece RoomCode = "123456" olanları bul
            var attrData = new AttributeData() { Key = "RoomCode", Value = roomCode };
            var paramOptions = new LobbySearchSetParameterOptions() { Parameter = attrData, ComparisonOp = ComparisonOp.Equal };
            searchHandle.SetParameter(ref paramOptions);
            
            var findOptions = new LobbySearchFindOptions() { LocalUserId = _localUserId };
            searchHandle.Find(ref findOptions, null, (ref LobbySearchFindCallbackInfo info) => 
            {
                if (info.ResultCode != Result.Success) {
                    currentStatus = "Arama Başarısız!";
                    return;
                }

                var countOptions = new LobbySearchGetSearchResultCountOptions();
                uint count = searchHandle.GetSearchResultCount(ref countOptions);
                
                if (count == 0) {
                    currentStatus = "Geçersiz Kod: " + roomCode + " adına lobi bulunamadı.";
                    return;
                }

                // Ekrana eşleşen ilk lobi sonuçlarını kopyala
                var resultOptions = new LobbySearchCopySearchResultByIndexOptions() { LobbyIndex = 0 };
                searchHandle.CopySearchResultByIndex(ref resultOptions, out LobbyDetails lobbyDetails);
                
                // Lobi yöneticisinin Epic hesabını (Host_ID) attributelerden çek
                var getAttrOptions = new LobbyDetailsCopyAttributeByKeyOptions() { AttrKey = "Host_ID" };
                lobbyDetails.CopyAttributeByKey(ref getAttrOptions, out Epic.OnlineServices.Lobby.Attribute? hostAttr);
                
                string hostIdString = hostAttr?.Data?.Value.AsUtf8;
                
                // Lobinin gerçek ID'sini (LeaveLobby yapabilmek için) önbelleğe al
                var lobbyInfoOptions = new LobbyDetailsCopyInfoOptions();
                lobbyDetails.CopyInfo(ref lobbyInfoOptions, out LobbyDetailsInfo? lobbyInfo);
                if (lobbyInfo.HasValue) _currentLobbyId = lobbyInfo.Value.LobbyId;

                // Odaya gir (İsteğe bağlı, zorunlu değil ama takım listesi için eklenebilir)
                var joinOptions = new JoinLobbyOptions() { LocalUserId = _localUserId, LobbyDetailsHandle = lobbyDetails, PresenceEnabled = false };
                _lobbyInterface.JoinLobby(ref joinOptions, null, (ref JoinLobbyCallbackInfo joinInfo) => 
                {
                    currentStatus = "Odaya Bağlanılıyor...";
                    
                    // P2P (NAT) delme işlemi için bağlanılacak Host IDsini FishNet'e ver.
                    SetFishyEOSRemoteId(hostIdString);
                    NetworkConnectionManager.Instance.StartOnlineClient();
                    currentStatus = "Odaya Katıldınız!";
                });
            });
        }

        // Reflection kullanarak farklı FishyEOS versiyonlarında bile çökmeden Remote User ID atama
        private void SetFishyEOSRemoteId(string hostId)
        {
            var eosTransport = NetworkConnectionManager.Instance.eosTransport;
            if (eosTransport == null) return;

            var type = eosTransport.GetType();
            FieldInfo field = type.GetField("RemoteProductUserId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(eosTransport, hostId);
            }
            else
            {
                PropertyInfo prop = type.GetProperty("RemoteProductUserId", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null) prop.SetValue(eosTransport, hostId, null);
            }
        }

        // --- GRACEFUL DISCONNECT (Lobi Temizliği) ---
        public void LeaveOrDestroyLobby()
        {
            if (_lobbyInterface == null || string.IsNullOrEmpty(_currentLobbyId)) return;

            if (_isLobbyOwner)
            {
                var destroyOptions = new DestroyLobbyOptions() { LocalUserId = _localUserId, LobbyId = _currentLobbyId };
                _lobbyInterface.DestroyLobby(ref destroyOptions, null, (ref DestroyLobbyCallbackInfo info) => 
                {
                    Debug.Log("<color=red>[EOSMatchmaking] Lobi başarıyla YIKILDI (Destroy): </color>" + info.ResultCode);
                });
            }
            else
            {
                var leaveOptions = new LeaveLobbyOptions() { LocalUserId = _localUserId, LobbyId = _currentLobbyId };
                _lobbyInterface.LeaveLobby(ref leaveOptions, null, (ref LeaveLobbyCallbackInfo info) => 
                {
                    Debug.Log("<color=orange>[EOSMatchmaking] Lobiden başarıyla AYRILINDI (Leave): </color>" + info.ResultCode);
                });
            }
            
            _currentLobbyId = string.Empty;
            currentRoomCode = "";
            _isLobbyOwner = false;
            currentStatus = "Bağlantı Kesildi (İşlem Tamam).";
        }
        // --- EPIC KULLANICI ADI SORGULAMA ---
        private void QueryEpicDisplayName()
        {
            var platformInterface = EOSManager.Instance.GetEOSPlatformInterface();
            if (platformInterface == null) return;

            _localEpicAccountId = EOSManager.Instance.GetLocalUserId();
            if (_localEpicAccountId == null || !_localEpicAccountId.IsValid())
            {
                Debug.Log("[EOSMatchmaking] Epic Account ID bulunamadı. Anonim devam ediliyor.");
                return;
            }

            var userInfoInterface = platformInterface.GetUserInfoInterface();
            var queryOptions = new Epic.OnlineServices.UserInfo.QueryUserInfoOptions()
            {
                LocalUserId = _localEpicAccountId,
                TargetUserId = _localEpicAccountId
            };

            userInfoInterface.QueryUserInfo(ref queryOptions, null, (ref Epic.OnlineServices.UserInfo.QueryUserInfoCallbackInfo info) =>
            {
                if (info.ResultCode == Result.Success)
                {
                    var copyOptions = new Epic.OnlineServices.UserInfo.CopyUserInfoOptions()
                    {
                        LocalUserId = _localEpicAccountId,
                        TargetUserId = _localEpicAccountId
                    };

                    userInfoInterface.CopyUserInfo(ref copyOptions, out Epic.OnlineServices.UserInfo.UserInfoData? data);
                    if (data.HasValue)
                    {
                        EpicDisplayName = data.Value.DisplayName;
                        Debug.Log("<color=cyan>[EOSMatchmaking] Hoşgeldin Epic Oyuncusu: " + EpicDisplayName + "</color>");
                    }
                }
                else
                {
                    Debug.LogWarning("[EOSMatchmaking] Epic kullanıcı adı sorgulama başarısız: " + info.ResultCode);
                }
            });
        }
    }
}
