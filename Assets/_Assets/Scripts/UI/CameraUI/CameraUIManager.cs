using System;
using _Assets.Scripts.Singleton;
using _Assets.Scripts.TCP;
using UnityEngine;
using UnityEngine.UI;

namespace _Assets.Scripts.UI.CameraUI
{
    public class CameraUIManager : SingletonMonoBehavior<CameraUIManager>
    {
        [SerializeField] private RawImage cameraRawImage;
        [SerializeField] private Texture disabledCameraTexture;
        [SerializeField] private CameraStatus previousCameraStatus = CameraStatus.None;
        [SerializeField] private CameraStatus currentCameraStatus = CameraStatus.None;

        private bool _canUseWebCamNormal = true;
        private SendContentType _sendContentType = SendContentType.None;
        private int _cameraDeviceIndex;
        private Texture2D _webCamTexture2D;
        private WebCamTexture _webCamTexture;
        
        public Texture TextureCamera => _webCamTexture;

        private void Start()
        {
            cameraRawImage.transform.localScale = new Vector3(-1, 1, 1);
            _webCamTexture2D = new Texture2D(1, 1);
        }

        private void Update()
        {
            currentCameraStatus = Utils.Utils.GetCameraStatus();
            if (previousCameraStatus == currentCameraStatus) return;
            HandleCameraDisplay(currentCameraStatus);
            previousCameraStatus = currentCameraStatus;
        }
        
        private void FixedUpdate()
        {
            switch (_sendContentType)
            {
                case SendContentType.HandTracking:
                    TcpClientManager.Instance.SendData(_webCamTexture, (int)SendContentType.HandTracking);
                    if (TcpClientManager.Instance.WebCamTextureBytes != null)
                    {
                        _webCamTexture2D.LoadImage(TcpClientManager.Instance.WebCamTextureBytes);
                        cameraRawImage.texture = _webCamTexture2D;
                    }
                    break;
                case SendContentType.CheckHaveAnyHands:
                    TcpClientManager.Instance.SendData(_webCamTexture, (int)SendContentType.CheckHaveAnyHands);
                    break;
            }
        }

        public void HandTrackingHandler()
        {
            gameObject.SetActive(true);
            _sendContentType = SendContentType.HandTracking;
            _canUseWebCamNormal = false;
        }
        
        private void OnDisable()
        {
            _webCamTexture.Stop();
        }

        private void HandleCameraDisplay(CameraStatus cameraStatus)
        {
            switch (cameraStatus)
            {
                case CameraStatus.DoNotHave:
                    _webCamTexture.Stop();
                    cameraRawImage.texture = disabledCameraTexture;
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
            _webCamTexture = new WebCamTexture(WebCamTexture.devices[_cameraDeviceIndex].name);
            if (_canUseWebCamNormal) cameraRawImage.texture = _webCamTexture;
            _webCamTexture.Play();
        }

        public void SetEventCheckHaveAnyHands()
        {
            _sendContentType = SendContentType.CheckHaveAnyHands;
            _canUseWebCamNormal = true;
        }
        
        private void OnDestroy()
        {
            _webCamTexture.Stop();
        }
    }
}
