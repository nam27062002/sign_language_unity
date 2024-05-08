using System;
using _Assets.Scripts.SignLanguage;
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

        public static string ConvertSignLanguageTypeToString(SignLanguageType type)
        {
            var s = type.ToString();
            return $"{s.ToUpper()}{s.ToLower()}";
        }
        public static float GetPercentPrediction(SignLanguageType type, bool haveAnyHands, float[] prediction)
        {
            if (!haveAnyHands) return 0;
            return prediction[(int)type];
        }
        public static (SignLanguageType, float) GetPrediction(bool haveAnyHands, float[] prediction)
        {
            if (!haveAnyHands)
                return (SignLanguageType.A, 0f); 

            var maxSignType = SignLanguageType.A; 
            var maxPrediction = prediction[0]; 

            for (var i = 1; i < prediction.Length; i++)
            {
                if (!(prediction[i] > maxPrediction)) continue;
                maxPrediction = prediction[i]; 
                maxSignType = (SignLanguageType)i;
            }

            return (maxSignType, maxPrediction);
        }

        
        public static SignLanguageType RandomSignLanguageType(SignLanguageType currentSignLanguageType)
        {
            var values = Enum.GetValues(typeof(SignLanguageType));
            var valuesArray = new SignLanguageType[values.Length - 1];
            var index = 0;
            for (var i = 0; i < values.Length; i++)
            {
                if ((SignLanguageType)values.GetValue(i) == currentSignLanguageType) continue;
                valuesArray[index] = (SignLanguageType)values.GetValue(i);
                index++;
            }
            return valuesArray[UnityEngine.Random.Range(0, valuesArray.Length)];
        }

    }
}