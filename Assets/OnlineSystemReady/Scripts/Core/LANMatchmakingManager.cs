using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace OnlineSystemReady.Core
{
    /// <summary>
    /// LAN (Yerel Ağ) için IP adresi girmeyi ortadan kaldırır. Radyo dalgası (UDP Broadcast) yayarak 6 haneli kodla eşleştirme sağlar.
    /// </summary>
    public class LANMatchmakingManager : MonoBehaviour
    {
        public static LANMatchmakingManager Instance;

        private const int BROADCAST_PORT = 47777;
        private UdpClient _udpBroadcaster;
        private UdpClient _udpListener;
        private Thread _listenerThread;
        private bool _isListening;
        private bool _isBroadcasting;

        private string _targetCodeToFind = "";
        private string _myHostCode = "";
        
        [Header("Durum (UI Feedback)")]
        public string currentStatus = "Bekleniyor...";
        public string currentRoomCode = "";

        // Ana Thread'e aktarılacak bulunan IP adresi
        private string _foundIpAddress = "";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            // Thread'den gelen Data sadece ana Thread'de Unity kodlarını tetikleyebilir
            if (!string.IsNullOrEmpty(_foundIpAddress))
            {
                string ip = _foundIpAddress;
                _foundIpAddress = ""; // Tetiklemeyi sıfırla
                StopListening();
                
                currentStatus = "Oda Bulundu! Bağlanılıyor: " + ip;
                NetworkConnectionManager.Instance.StartLANClient(ip);
            }
        }

        private void OnDestroy()
        {
            StopBroadcasting();
            StopListening();
        }

        // --- HOST (Oda Kurucu) İşlemleri ---
        public void StartHostLAN()
        {
            _myHostCode = UnityEngine.Random.Range(100000, 999999).ToString();
            currentRoomCode = _myHostCode;
            currentStatus = "LAN Odası Kuruldu! Kod: " + currentRoomCode;

            // FishNet Server'ı başlat
            NetworkConnectionManager.Instance.StartLANHost();

            // Saniyede bir kez yerel ağa "Ben Buradayım" diye UDP yayını başlat
            StartBroadcasting();
        }

        private void StartBroadcasting()
        {
            if (_isBroadcasting) return;
            _isBroadcasting = true;

            try
            {
                _udpBroadcaster = new UdpClient();
                _udpBroadcaster.EnableBroadcast = true;
                InvokeRepeating(nameof(BroadcastPulse), 0f, 1.5f); // Her 1.5 saniyede bir dalga yay
            }
            catch (Exception e)
            {
                Debug.LogError("[LANMatchmaking] Broadcast başlatılamadı: " + e.Message);
            }
        }

        private void BroadcastPulse()
        {
            if (!_isBroadcasting) return;
            
            // Gönderilecek mesaj formatı: "FISHNET_LAN|123456"
            string message = "FISHNET_LAN|" + _myHostCode;
            byte[] data = Encoding.UTF8.GetBytes(message);

            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, BROADCAST_PORT);
                _udpBroadcaster.Send(data, data.Length, endPoint);
            }
            catch { /* Sessizce yut, ağ meşgul olabilir */ }
        }

        public void StopBroadcasting()
        {
            _isBroadcasting = false;
            CancelInvoke(nameof(BroadcastPulse));
            if (_udpBroadcaster != null)
            {
                _udpBroadcaster.Close();
                _udpBroadcaster = null;
            }
        }

        // --- CLIENT (Katılımcı) İşlemleri ---
        public void StartClientLAN(string roomCode)
        {
            if (string.IsNullOrEmpty(roomCode) || roomCode.Length != 6)
            {
                currentStatus = "Lütfen 6 haneli kodu eksiksiz girin!";
                return;
            }

            _targetCodeToFind = roomCode;
            currentStatus = "Oda Aranıyor: " + roomCode;
            StartListening();
        }

        private void StartListening()
        {
            if (_isListening) return;
            _isListening = true;
            _foundIpAddress = "";

            _listenerThread = new Thread(ListenForBroadcast);
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
        }

        private void ListenForBroadcast()
        {
            try
            {
                _udpListener = new UdpClient();
                _udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _udpListener.Client.Bind(new IPEndPoint(IPAddress.Any, BROADCAST_PORT));

                while (_isListening)
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = _udpListener.Receive(ref remoteEP);
                    string message = Encoding.UTF8.GetString(data);

                    if (message.StartsWith("FISHNET_LAN|"))
                    {
                        string[] parts = message.Split('|');
                        if (parts.Length == 2 && parts[1] == _targetCodeToFind)
                        {
                            // Aradığımız kodu dinledik ve ip adresini (remoteEP.Address) ele geçirdik!
                            _foundIpAddress = remoteEP.Address.ToString();
                            break; // Döngüden çık
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_isListening) Debug.LogWarning("[LANMatchmaking] Listener hatası: " + e.Message);
            }
            finally
            {
                _isListening = false;
            }
        }

        public void StopListening()
        {
            _isListening = false;
            if (_udpListener != null)
            {
                _udpListener.Close();
                _udpListener = null;
            }
            if (_listenerThread != null && _listenerThread.IsAlive)
            {
                _listenerThread.Abort();
            }
        }
    }
}
