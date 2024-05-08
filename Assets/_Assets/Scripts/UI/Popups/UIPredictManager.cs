using System;
using _Assets.Scripts.SignLanguage;
using _Assets.Scripts.Singleton;
using _Assets.Scripts.TCP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Assets.Scripts.UI.Popups
{
    public class UIPredictManager : SingletonMonoBehavior<UIPredictManager>
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [Range(0, 1)] [SerializeField] private float minimumPrecision = 0.7f;

        private SignLanguageType _signLanguageType;

        public void Init(SignLanguageType signLanguageType)
        {
            _signLanguageType = signLanguageType;
            Enable = true;
        }

        private void SetImage(string letter, float precision)
        {
            if (letter == null) return;
            var signType = (SignLanguageType)Enum.Parse(typeof(SignLanguageType), letter);
            image.sprite = GameManager.Instance.signLanguageEntries.GetSignLanguage(signType).sprite;
            
            textMeshProUGUI.text = signType == _signLanguageType ? $"<color=green>{letter}</color> - " : $"<color=red>{letter}</color> - ";
            
            if (precision >= minimumPrecision)
            {
                textMeshProUGUI.text += $"<color=green>{(precision * 100):F2}%</color>";
            }
            else
            {
                textMeshProUGUI.text += $"<color=red>{(precision * 100):F2}%</color>";
            }
        }

        private void Update()
        {
            if (!Enable) return;
            SetImage(TcpClientManager.Instance.Label, TcpClientManager.Instance.Confidence);
        }

        public bool Enable
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
}
}
