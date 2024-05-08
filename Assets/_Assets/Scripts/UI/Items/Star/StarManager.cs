using System.Collections.Generic;
using UnityEngine;

namespace _Assets.Scripts.UI.Items.Star
{
    public class StarManager : MonoBehaviour
    {
        [SerializeField] private List<SingleStarManager> singleStarManagers;
        [Range(0,1)] [SerializeField] private float percent;
        
        private void Start()
        {
            SetStarUI();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetStarUI();
        }
#endif
        public void SetPercent(float value)
        {
            percent = value;
            SetStarUI();
        }
        private void SetStarUI()
        {
            var numStarsToLight = Mathf.FloorToInt(singleStarManagers.Count * percent);
            for (var i = 0; i < singleStarManagers.Count; i++)
            {
                if (i < numStarsToLight)
                {
                    singleStarManagers[i].SetFillAmount(1f);
                }
                else if (i == numStarsToLight && numStarsToLight < singleStarManagers.Count)
                {
                    var lastStarFillAmount = (singleStarManagers.Count * percent) - numStarsToLight;
                    singleStarManagers[i].SetFillAmount(lastStarFillAmount);
                }
                else
                {
                    singleStarManagers[i].SetFillAmount(0f);
                }
            }
        }
    }
}