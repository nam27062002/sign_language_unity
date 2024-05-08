using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Assets.Scripts.UI.CameraUI
{
    public class CameraUIManager : MonoBehaviour
    {
        [SerializeField] private RawImage cameraRawImage;
        [SerializeField] private GameObject noCamera;
        private CameraStatus _previousCameraStatus = CameraStatus.None;
        private CameraStatus _currentCameraStatus = CameraStatus.None;
        private int _cameraDeviceIndex;
        public WebCamTexture WebCamTexture { get; private set; }
        
        private void Start()
        {
            cameraRawImage.transform.localScale = new Vector3(-1, 1, 1);
        }

        private void Update()
        {
            _currentCameraStatus = Utils.Utils.GetCameraStatus();
            if (_previousCameraStatus == _currentCameraStatus) return;
            HandleCameraDisplay(_currentCameraStatus);
            _previousCameraStatus = _currentCameraStatus;
        }
        
        // private void FixedUpdate()
        // {
        //     if (_cameraUIState != CameraUIState.Camera) return;
        //     switch (_sendContentType)
        //     {
        //         case SendContentType.HandTracking:
        //             TcpClientManager.Instance.SendData(WebCamTexture, (int)SendContentType.HandTracking);
        //             if (TcpClientManager.Instance.WebCamTextureBytes != null)
        //             {
        //                 _webCamTexture2D.LoadImage(TcpClientManager.Instance.WebCamTextureBytes);
        //                 cameraRawImage.texture = _webCamTexture2D;
        //             }
        //             break;
        //         
        //         case SendContentType.CheckHaveAnyHands:
        //             TcpClientManager.Instance.SendData(WebCamTexture, (int)SendContentType.CheckHaveAnyHands);
        //             break;
        //     }
        // }
        
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
        
        private void HandleCameraDisplay(CameraStatus cameraStatus)
        {
            switch (cameraStatus)
            {
                case CameraStatus.DoNotHave:
                    if (WebCamTexture != null) WebCamTexture.Stop();
                    noCamera.SetActive(true);
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
            noCamera.SetActive(false);
            WebCamTexture = new WebCamTexture(WebCamTexture.devices[_cameraDeviceIndex].name);
            cameraRawImage.texture = WebCamTexture;
            WebCamTexture.Play();
        }
        
    }
}
