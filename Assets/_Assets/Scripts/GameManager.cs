using System;
using _Assets.Scripts.SignLanguage;
using _Assets.Scripts.Singleton;
using UnityEngine;

namespace _Assets.Scripts
{
    public class GameManager : SingletonMonoBehavior<GameManager>
    {
        public SignLanguageEntries signLanguageEntries;
        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60; // 60 FPS
        }

        private void Start()
        {
            LoadFirstMenu();
        }


        private void LoadFirstMenu()
        {
            GUIManager.Instance.OpenMenu(MenuName.OnBoarding);
        }



        #region EventButton

        public void OnOpenCameraClicked()
        {
            GUIManager.Instance.OpenPopUp(PopUpName.OnBoarding);
        }

        public void OnClosePopUpClicked()
        {
            GUIManager.Instance.ClosePopup();
        }
        #endregion
    }
}
