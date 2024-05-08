using UnityEngine;
using UnityEngine.UI;

namespace _Assets.Scripts.UI.Items.Star
{
    public class SingleStarManager : MonoBehaviour
    {
        [Range(0,1)] [SerializeField] private float fillAmount = 1;
        [SerializeField] private Image unlockStar;
        [SerializeField] private Image lockStar;

        public void SetFillAmount(float amount)
        {
            fillAmount = amount;
            unlockStar.fillAmount = fillAmount;
            lockStar.fillAmount = 1 - fillAmount;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            SetFillAmount(fillAmount);
        }
#endif
    }
}
