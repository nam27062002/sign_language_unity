using System.Collections;
using _Assets.Scripts.SignLanguage;
using _Assets.Scripts.Singleton;
using _Assets.Scripts.TCP;
using _Assets.Scripts.UI.CameraUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Assets.Scripts.UI.Menus
{
    public class Game1 : SingletonMonoBehavior<Game1>
    {
        [Header("Scriptable")]
        [SerializeField] private SignLanguageEntries signLanguageEntries;
        [SerializeField] private TcpData tcpData;
    
        [Header("Script")]
        [SerializeField] private CameraUIManager cameraUIManager;
    
        [Header("Button")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button tutorialButton;
    
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI letterText;
        [SerializeField] private TextMeshProUGUI percentText;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private TextMeshProUGUI countdownSkipText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI winCountdownText;
        
        [Header("Image")]
        [SerializeField] private Image signLanguageImage;
        [SerializeField] private Image iconSkipImage;
        [SerializeField] private Image questionMarkImage;
        
        [Header("View")]
        [SerializeField] private SignLanguageType currentSignLanguageType;

        [Header("Game Object")] 
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject resultPanel;
        
        [Header("Value")]
        [SerializeField] private int countdown = 120;
        [SerializeField] private int countdownSkip = 5;
        [SerializeField] private int newRoundCountdown = 3;
        
        private TcpClient _tcpClient;
        private float _percent;
        private SignLanguageType _signLanguageTypeSever;
        private int _score;
        private bool _canSendData;
        
        protected override void Awake()
        {
            base.Awake();
            _tcpClient = new TcpClient(tcpData);
        }
    
        private void Start()
        {
            StartCoroutine(SetUI());
            skipButton.onClick.AddListener(() =>
            {
                StartCoroutine(SetUI());
            });
            tutorialButton.onClick.AddListener(OnClickTutorialButton);
            StartCoroutine(CountDownTextHandler());
        }
        
        private void Update()
        {
            if (!_canSendData) return;
            SendData();
            SetPercent();
        }
        
        private void OnEnable()
        {
            _tcpClient.OnSendEventEvaluateAccuracy += TcpClientOnOnSendEventEvaluateAccuracy;
        }
        
        private void OnDisable()
        {
            _tcpClient.OnSendEventEvaluateAccuracy -= TcpClientOnOnSendEventEvaluateAccuracy;
        }

        private void OnDestroy()
        {
            _tcpClient.OnCloseConnect();
        }

        private IEnumerator SetUI()
        {
            percentText.text = "";
            winPanel.SetActive(true);
            resultPanel.SetActive(false);
            for (var i = newRoundCountdown; i > 0; i--)
            {
                winCountdownText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
            
            winPanel.SetActive(false);
            resultPanel.SetActive(true);
            
            LookedButtonSkip();
            tutorialButton.gameObject.SetActive(false);
            scoreText.text = _score.ToString();
            currentSignLanguageType = Utils.Utils.RandomSignLanguageType(currentSignLanguageType);
            signLanguageImage.sprite = signLanguageEntries.GetSignLanguage(currentSignLanguageType).sprite;
            questionMarkImage.gameObject.SetActive(true);
            signLanguageImage.gameObject.SetActive(false);
            letterText.text = Utils.Utils.ConvertSignLanguageTypeToString(currentSignLanguageType);
            StartCoroutine(CountDownButtonSkipHandler());
            _canSendData = true;
        }

        private IEnumerator CountDownTextHandler()
        {
            for (var i = countdown; i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
        }

        private IEnumerator CountDownButtonSkipHandler()
        {
            for (var i = countdownSkip; i > 0; i--)
            {
                countdownSkipText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
            ShowCountdownSkip();
            tutorialButton.gameObject.SetActive(true);
        }
        
        private void LookedButtonSkip()
        {
            skipButton.enabled = false;
            iconSkipImage.gameObject.SetActive(false);
            countdownSkipText.gameObject.SetActive(true);
        }

        private void ShowCountdownSkip()
        {
            skipButton.enabled = true;
            iconSkipImage.gameObject.SetActive(true);
            countdownSkipText.gameObject.SetActive(false);
        }
        
        private void SendData()
        {
            _tcpClient.SendData(cameraUIManager.WebCamTexture, SendContentType.EvaluateAccuracy);
        }
        
        private void SetPercent()
        {
            var formattedPercent = $"{_signLanguageTypeSever.ToString()}: {_percent * 100:F2}%";
            percentText.text = formattedPercent;
            var isMatch = _signLanguageTypeSever == currentSignLanguageType;
            percentText.color = isMatch ? Color.green : Color.red;
            if (!isMatch) return;
            _canSendData = false;
            _score += 10;
            scoreText.text = _score.ToString();
            StartCoroutine(SetUI());
        }
        
        private void TcpClientOnOnSendEventEvaluateAccuracy(object sender, TcpClient.OnSendEventEvaluateAccuracyArgs e)
        {
            (_signLanguageTypeSever, _percent) = Utils.Utils.GetPrediction(e.DetectHand, e.Prediction);
        }

        private void OnClickTutorialButton()
        {
            signLanguageImage.gameObject.SetActive(true);
            questionMarkImage.gameObject.SetActive(false);
        }
    }
}
