using _Assets.Scripts.TCP;
using _Assets.Scripts.UI.CameraUI;
using UnityEngine;

namespace _Assets.Scripts.Cheats
{
    public class CheatManager : MonoBehaviour
    {
        public void SendCameraImage()
        {
            TcpClientManager.Instance.SendData(CameraUIManager.Instance.WebCamTexture, (int)SendContentType.HandTracking);
        }
    }
}
