using _Assets.Scripts.SignLanguage;
using _Assets.Scripts.Singleton;
using _Assets.Scripts.TCP;
using _Assets.Scripts.UI.CameraUI;
using _Assets.Scripts.UI.Items.Star;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Assets.Scripts.UI.Menus
{
    public class UIEvaluateAccuracy : SingletonMonoBehavior<UIEvaluateAccuracy>
    {
        [Header("Scriptable")]
        [SerializeField] private SignLanguageEntries signLanguageEntries;
        [SerializeField] private TcpData tcpData;
        
        [Header("Script")]
        [SerializeField] private StarManager starManager;
        [SerializeField] private CameraUIManager cameraUIManager;
        
        [Header("Button")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI letterText;
        [SerializeField] private TextMeshProUGUI percentText;
        
        [Header("Image")]
        [SerializeField] private Image signLanguageImage;
        
        [Header("View")]
        [SerializeField] private SignLanguageType currentSignLanguageType;
        private TcpClient _tcpClient;
        private float _percent;
        #region API Monobehaviour

        protected override void Awake()
        {
            base.Awake();
            _tcpClient = new TcpClient(tcpData);
        }

        private void Start()
        {
            currentSignLanguageType = SignLanguageType.A;
            SetUI();
            previousButton.onClick.AddListener(GetPreviousSignLanguage);
            nextButton.onClick.AddListener(GetNextSignLanguage);
        }

        private void Update()
        {
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
        #endregion
        

        private void SendData()
        {
            _tcpClient.SendData(cameraUIManager.WebCamTexture, SendContentType.EvaluateAccuracy);
        }
        
        private void SetUI()
        {
            signLanguageImage.sprite = signLanguageEntries.GetSignLanguage(currentSignLanguageType).sprite;
            letterText.text = Utils.Utils.ConvertSignLanguageTypeToString(currentSignLanguageType);
        }

        private void GetPreviousSignLanguage()
        {
            signLanguageEntries.GetPreviousSignLanguage(ref currentSignLanguageType);
            SetUI();
        }

        private void GetNextSignLanguage()
        {
            signLanguageEntries.GetNextSignLanguage(ref currentSignLanguageType);
            SetUI();
        }

        private void SetPercent()
        {
            var formattedPercent = $"{_percent * 100:F2}%";
            percentText.text = formattedPercent;
            starManager.SetPercent(_percent);
            percentText.color = _percent >= 0.7f ? Color.green : Color.red;
        }
        
        private void TcpClientOnOnSendEventEvaluateAccuracy(object sender, TcpClient.OnSendEventEvaluateAccuracyArgs e)
        {
            _percent = Utils.Utils.GetPercentPrediction(currentSignLanguageType, e.DetectHand, e.Prediction);
        }
        
    }
}
