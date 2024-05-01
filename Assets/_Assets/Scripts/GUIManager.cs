using System;
using System.Collections.Generic;
using System.Linq;
using _Assets.Scripts.Singleton;
using UnityEngine;

namespace _Assets.Scripts
{
    public enum MenuName
    {
        OnBoarding,
    }
    
    public enum PopUpName
    {
        OnBoarding,
    }
    
    public class GUIManager : SingletonMonoBehavior<GUIManager>
    {
        [Serializable]
        public class Menus
        {
            public MenuName menuName;
            public GameObject prefab;
        }
        
        [Serializable]
        public class PopUps
        {
            public PopUpName popupName;
            public GameObject prefab;    
        }
        [SerializeField] private List<Menus> menusList;
        [SerializeField] private List<PopUps> popupsList;
        [SerializeField] private Transform menuLayout;
        [SerializeField] private Transform popupLayout;
        [SerializeField] private GameObject blocked;
        [SerializeField] private CanvasGroup menuCanvasGroup;
        
        public void OpenMenu(MenuName menuName)
        {
            menuCanvasGroup.interactable = true;
            blocked.SetActive(false);
            ClearAllChildObject(menuLayout);
            Instantiate(menusList.FirstOrDefault(p => p.menuName == menuName)?.prefab, menuLayout);
        }

        public void OpenPopUp(PopUpName popUpName)
        {
            menuCanvasGroup.interactable = false;
            blocked.SetActive(true);
            ClearAllChildObject(popupLayout);
            Instantiate(popupsList.FirstOrDefault(p => p.popupName == popUpName)?.prefab, popupLayout);
        }

        public void ClosePopup()
        {
            menuCanvasGroup.interactable = true;
            blocked.SetActive(false);
            ClearAllChildObject(popupLayout);
        }
        
        private static void ClearAllChildObject(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                Debug.LogWarning("Parent Transform is null.");
                return;
            }
            foreach (Transform child in parentTransform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
