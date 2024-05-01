using System;
using UnityEngine;

namespace _Assets.Scripts
{
    public class PrefabLookUp : MonoBehaviour
    {
        [SerializeField] private GameObject[] prefabs;

        private void Awake()
        {
            foreach (var item in prefabs)
            {
                Instantiate(item);
            }
        }
    }
}
