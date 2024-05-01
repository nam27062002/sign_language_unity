using System;
using System.Collections.Generic;
using _Assets.Scripts.Singleton;
using _Assets.Scripts.TCP;
using UnityEngine;
using UnityEngine.UI;

namespace _Assets.Scripts.UI.CameraUI
{
    public enum CameraUIState
    {
        Camera,
        NoCamera,
        SignLanguage
    }
    
    public class CameraUIManager : SingletonMonoBehavior<CameraUIManager>
    {
        [SerializeField] private RawImage cameraRawImage;
        [SerializeField] private GameObject noCamera;
        [SerializeField] private GameObject signLanguage;
        [SerializeField] private Image signLanguageImage;
        
        private CameraUIState _cameraUIState;
        private Dictionary<CameraUIState, GameObject> _stateObjects;
        private CameraStatus _previousCameraStatus = CameraStatus.None;
        private CameraStatus _currentCameraStatus = CameraStatus.None;
        private int _cameraDeviceIndex;
        private Texture2D _webCamTexture2D;
        private SendContentType _sendContentType;
        
        public WebCamTexture WebCamTexture { get; private set; }
        
        private void Start()
        {
            _stateObjects = new Dictionary<CameraUIState, GameObject>
            {
                { CameraUIState.Camera, cameraRawImage.gameObject },
                { CameraUIState.NoCamera, noCamera },
                { CameraUIState.SignLanguage, signLanguage }
            };
            _sendContentType = SendContentType.None;
            cameraRawImage.transform.localScale = new Vector3(-1, 1, 1);
            _webCamTexture2D = new Texture2D(16, 9);
        }

        private void Update()
        {
            if (_cameraUIState != CameraUIState.Camera) return;
            _currentCameraStatus = Utils.Utils.GetCameraStatus();
            if (_previousCameraStatus == _currentCameraStatus) return;
            HandleCameraDisplay(_currentCameraStatus);
            _previousCameraStatus = _currentCameraStatus;
        }
        
        private void FixedUpdate()
        {
            if (_cameraUIState != CameraUIState.Camera) return;
            switch (_sendContentType)
            {
                case SendContentType.HandTracking:
                    TcpClientManager.Instance.SendData(WebCamTexture, (int)SendContentType.HandTracking);
                    if (TcpClientManager.Instance.WebCamTextureBytes != null)
                    {
                        _webCamTexture2D.LoadImage(TcpClientManager.Instance.WebCamTextureBytes);
                        cameraRawImage.texture = _webCamTexture2D;
                    }
                    break;
                
                case SendContentType.CheckHaveAnyHands:
                    TcpClientManager.Instance.SendData(WebCamTexture, (int)SendContentType.CheckHaveAnyHands);
                    break;
            }
        }
        
        private void OnDisable()
        {
            StopWebcam();
        }
        
        private void OnDestroy()
        {
            StopWebcam();
        }

        private void StopWebcam()
        {
            if (WebCamTexture != null) WebCamTexture.Stop();
        }
        
        private void ChangeCameraUIState(CameraUIState state)
        {
            if (_cameraUIState == state) return;
            if (state != CameraUIState.Camera)
            {
                StopWebcam();
            }
            else
            {
                _previousCameraStatus = CameraStatus.None;
                _currentCameraStatus = CameraStatus.None;
            }
            foreach (var kvp in _stateObjects)
            {
                kvp.Value.SetActive(kvp.Key == state);
            }
            _cameraUIState = state;
        }

        public void SetSendContentType(SendContentType sendContentType)
        {
            ChangeCameraUIState(CameraUIState.Camera);
            _sendContentType = sendContentType;
        }
        
        private void HandleCameraDisplay(CameraStatus cameraStatus)
        {
            switch (cameraStatus)
            {
                case CameraStatus.DoNotHave:
                    if (WebCamTexture != null) WebCamTexture.Stop();
                    ChangeCameraUIState(CameraUIState.NoCamera);
                    break;
                case CameraStatus.One:
                case CameraStatus.MoreThanOne:
                    ActivateCamera();
                    break;
                case CameraStatus.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cameraStatus), cameraStatus, null);
            }
        }
        
        private void ActivateCamera()
        {
            WebCamTexture = new WebCamTexture(WebCamTexture.devices[_cameraDeviceIndex].name);
            if (Mathf.Abs((int)_sendContentType % 2) == 1) cameraRawImage.texture = WebCamTexture;
            WebCamTexture.Play();
        }

        public void ShowSignLanguage(Sprite sprite)
        {
            ChangeCameraUIState(CameraUIState.SignLanguage);
            signLanguageImage.sprite = sprite;
        }
    }
}
