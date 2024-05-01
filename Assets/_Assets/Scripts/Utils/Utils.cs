using _Assets.Scripts.UI.CameraUI;
using UnityEngine;

namespace _Assets.Scripts.Utils
{
    public static class Utils
    {
        private static Texture2D _texture2D;

        public static Texture2D TextureToTexture2D(Texture texture)
        {
            if (_texture2D == null)
            {
                _texture2D = new Texture2D(texture.width, texture.height);
            }

            var renderTexture = RenderTexture.GetTemporary(
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear
            );
            Graphics.Blit(texture, renderTexture);
            var previous = RenderTexture.active;
            RenderTexture.active = renderTexture;
            _texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            _texture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);
            return _texture2D;
        }
        
        public static CameraStatus GetCameraStatus()
        {
            return WebCamTexture.devices.Length switch
            {
                0 => CameraStatus.DoNotHave,
                1 => CameraStatus.One,
                _ => CameraStatus.MoreThanOne
            };
        }
    }
}