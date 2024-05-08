using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace _Assets.Scripts.TCP
{
    public class TcpClient
    {
        private readonly NetworkStream _stream;
        private readonly System.Net.Sockets.TcpClient _client;
        private readonly byte[] _receiveBuffer; 
        private readonly Dictionary<int, DateTime> _lastSendTimes;
        private readonly TimeSpan _sendInterval;

        #region Event

        public event EventHandler<OnSendEventEvaluateAccuracyArgs> OnSendEventEvaluateAccuracy;
        public class OnSendEventEvaluateAccuracyArgs : EventArgs
        {
            public bool DetectHand;
            public float[] Prediction;
        }

        #endregion
        
        public TcpClient(TcpData tcpData)
        {
            _receiveBuffer = new byte[tcpData.buffer]; 
            _lastSendTimes = new Dictionary<int, DateTime>();
            _sendInterval = TimeSpan.FromSeconds(tcpData.sendInterval);
            
            try
            {
                _client = new System.Net.Sockets.TcpClient(tcpData.serverIP, tcpData.serverPort);
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

        public void SendData(Texture texture, SendContentType type)
        {
            if (texture == null) return;
            var id = (int)type;
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

            }
        }
        
        private void HandleReceivedData(int id, byte[] data)
        {
            switch (id)
            {
                case (int)SendContentType.HandTracking:
                    // Label = Encoding.UTF8.GetString(data, 0, 1);
                    // Confidence = BitConverter.ToSingle(data, 1);
                    // var imageData = new byte[data.Length - 5];
                    // Array.Copy(data, 5, imageData, 0, imageData.Length);
                    // WebCamTextureBytes = imageData;
                    break;
                case (int)SendContentType.CheckHaveAnyHands:
                    // HaveAnyHands = BitConverter.ToBoolean(data);
                    break;
                case (int)SendContentType.EvaluateAccuracy:
                    var detectHand = data[0] == 1;
                    var prediction = new float[(data.Length - 1) / sizeof(float)];
                    Buffer.BlockCopy(data, 1, prediction, 0, data.Length - 1);
                    OnSendEventEvaluateAccuracy?.Invoke(this, new OnSendEventEvaluateAccuracyArgs
                    {
                        DetectHand = detectHand,
                        Prediction = prediction
                    });
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

        public void OnCloseConnect()
        {
            _client?.Close();
        }
    }
}