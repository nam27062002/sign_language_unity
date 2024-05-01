using System.Collections;
using _Assets.Scripts.SignLanguage;
using _Assets.Scripts.Singleton;
using _Assets.Scripts.TCP;
using _Assets.Scripts.UI.CameraUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Assets.Scripts.UI.Popups
{
    public class PopUpOnBoardingManager : SingletonMonoBehavior<PopUpOnBoardingManager>
    {
        [SerializeField] private TextMeshProUGUI proUGUI;
        [SerializeField] private Button startButton;
        [SerializeField] private string[] texts;
        [SerializeField] private Image circleImage;
        [SerializeField] private Image circleImage2;
        [SerializeField] private float countdownTutorial;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private SignLanguageType signLanguageType;
        
        private bool _isFinishFirstTutorial;
        private bool _canCountDownUpdate;
        private int _index = -1;
        private float _fillAmount;
        private bool _isCountdownRunning;

        private enum FollowTutorial
        {
            Prepare,
            Start,
            LearnFirstSign
        }

        private FollowTutorial _followTutorial = FollowTutorial.Prepare;
        private void Start()
        {
            circleImage.gameObject.SetActive(false);
            UpdateText();
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void Update()
        {
            OnOnHaveAnyHands();
        }

        private void OnStartButtonClicked()
        {
            if (_followTutorial == FollowTutorial.Prepare)
            {
                UpdateText();
                _canCountDownUpdate = true;
                CameraUIManager.Instance.SetSendContentType(SendContentType.CheckHaveAnyHands);
                startButton.gameObject.SetActive(false);
            }

            else if (_followTutorial == FollowTutorial.LearnFirstSign)
            {
                CameraUIManager.Instance.SetSendContentType(SendContentType.HandTracking);
            }
        }
        
        private void UpdateText()
        {
            _index++;
            proUGUI.text = texts[_index];
        }
        
        
        private void OnOnHaveAnyHands()
        {
            if (!_canCountDownUpdate || _isFinishFirstTutorial) return;
            if (!_isCountdownRunning && TcpClientManager.Instance.HaveAnyHands)
            {
                StartCoroutine(CircleActive());
            }
            if (!TcpClientManager.Instance.HaveAnyHands)
            {
                _isCountdownRunning = false;
            }
        }
        
        private IEnumerator CircleActive()
        {
            _isCountdownRunning = true;
            circleImage.gameObject.SetActive(true);
            var elapsedTime = 0f;
            var duration = countdownTutorial;
            while (elapsedTime < duration && _isCountdownRunning)
            {
                _fillAmount = elapsedTime / duration;
                circleImage.fillAmount = _fillAmount;
                circleImage2.fillAmount = _fillAmount;
            
                elapsedTime += Time.deltaTime;
                yield return null; 
            }
            
            if (_isCountdownRunning)
            {
                _fillAmount = 1f;
                circleImage.fillAmount = 1f;
                circleImage2.fillAmount = 1f;
                _isFinishFirstTutorial = true;
                circleImage.gameObject.SetActive(false);
                UpdateText();
                _followTutorial = FollowTutorial.LearnFirstSign;
                startButton.gameObject.SetActive(true);
                buttonText.text = "Ready";
                CameraUIManager.Instance.ShowSignLanguage(GameManager.Instance.signLanguageEntries.GetSignLanguage(signLanguageType).sprite);
            }
            else
            {
                circleImage.gameObject.SetActive(false);
                _fillAmount = 0f;
                circleImage.fillAmount = 0f;
                circleImage2.fillAmount = 0f;
            }
        }
    }
}
