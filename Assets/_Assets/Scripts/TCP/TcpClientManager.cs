using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using _Assets.Scripts.Singleton;
using UnityEngine;

namespace _Assets.Scripts.TCP
{
    public class TcpClientManager : SingletonMonoBehavior<TcpClientManager>
    {
        [SerializeField] private string serverIP;
        [SerializeField] private int serverPort;
        [SerializeField] [Range(0,0.2f)] private float sendInterval;
        
        private System.Net.Sockets.TcpClient _client;
        private NetworkStream _stream;
        private readonly byte[] _receiveBuffer = new byte[262144]; 
        private Dictionary<int, DateTime> _lastSendTimes;
        private TimeSpan _sendInterval;

        #region TCP

        public byte[] WebCamTextureBytes { get; private set; }
        public bool HaveAnyHands { get; private set; }

        public string Label { get; private set; }
        public float Confidence { get; private set; }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _lastSendTimes = new Dictionary<int, DateTime>();
            _sendInterval = TimeSpan.FromSeconds(sendInterval);
        }
        
        private void Start()
        {
            try
            {
                _client = new System.Net.Sockets.TcpClient(serverIP, serverPort);
                _stream = _client.GetStream();
                StartReceiving();
                Debug.Log("Connected to server successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError("Error connecting to server: " + e.Message);
            }
        }
        
        #region SendData

        public void SendData(Texture texture, int id)
        {
            if (texture == null) return;
            if (!CanSendRequest(id)) return;
            var data = Utils.Utils.TextureToTexture2D(texture);
            var idBytes = BitConverter.GetBytes(id);
            Array.Reverse(idBytes);
            var dataBytes = data.EncodeToJPG(); 
            Task.Run(() => SendData(idBytes, dataBytes));
            
        }
        
        private void SendData(byte[] idBytes, byte[] dataBytes)
        {   
            var combinedBytes = new byte[idBytes.Length + dataBytes.Length];
            Array.Copy(idBytes, 0, combinedBytes, 0, idBytes.Length);
            Array.Copy(dataBytes, 0, combinedBytes, idBytes.Length, dataBytes.Length);
            _stream.Write(combinedBytes, 0, combinedBytes.Length);
            _stream.Flush();
        }

        #endregion
        
        #region ReceiveData

        private void StartReceiving()
        {
            Task.Run(() => _stream.BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, ReceiveCallback, null));
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var bytesRead = _stream.EndRead(result);
                if (bytesRead <= 0)
                {
                    Debug.Log("Connection closed by server.");
                    CloseConnectionAndQuit();
                    return;
                }
                var idBytes = new byte[4];
                Array.Copy(_receiveBuffer, idBytes, 4);
                Array.Reverse(idBytes);
                var receivedID = BitConverter.ToInt32(idBytes, 0);
                var imageDataLength = bytesRead - 4;
                var receivedImageData = new byte[imageDataLength];
                Array.Copy(_receiveBuffer, 4, receivedImageData, 0, imageDataLength);
                HandleReceivedData(receivedID, receivedImageData);
                StartReceiving();
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving data: " + e.Message);
                CloseConnectionAndQuit();
            }
        }
        
        private void CloseConnectionAndQuit()
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
            Application.Quit();
        }

        private void HandleReceivedData(int id, byte[] data)
        {
            switch (id)
            {
                case (int)SendContentType.HandTracking:
                    Label = Encoding.UTF8.GetString(data, 0, 1);
                    Confidence = BitConverter.ToSingle(data, 1);
                    var imageData = new byte[data.Length - 5];
                    Array.Copy(data, 5, imageData, 0, imageData.Length);
                    WebCamTextureBytes = imageData;
                    break;
                case (int)SendContentType.CheckHaveAnyHands:
                    HaveAnyHands = BitConverter.ToBoolean(data);
                    break;
            }
        }


        #endregion
        
        private bool CanSendRequest(int id)
        {
            if (_lastSendTimes.ContainsKey(id) && DateTime.Now - _lastSendTimes[id] < _sendInterval) return false;
            _lastSendTimes[id] = DateTime.Now;
            return true;
        }
        
        private void OnDestroy()
        {
            _client?.Close();
        }
    }
}
