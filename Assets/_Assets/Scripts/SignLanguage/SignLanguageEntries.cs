using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Assets.Scripts.SignLanguage
{
    [CreateAssetMenu()]
    public class SignLanguageEntries : ScriptableObject
    {
        [Serializable]
        public class SignLanguage
        {
            public SignLanguageType type;
            public Sprite sprite;
        }

        public List<SignLanguage> signLanguages;

        public SignLanguage GetSignLanguage(SignLanguageType type)
        {
            return signLanguages.FirstOrDefault(p => p.type == type);
        }

        public void GetPreviousSignLanguage(ref SignLanguageType type)
        {
            var languageType = type;
            var currentIndex = signLanguages.FindIndex(p => p.type == languageType);
            if (currentIndex == -1) return;
            var previousIndex = (currentIndex == 0) ? signLanguages.Count - 1 : currentIndex - 1;
            type = signLanguages[previousIndex].type;
        }

        public void GetNextSignLanguage(ref SignLanguageType type)
        {
            var languageType = type;
            var currentIndex = signLanguages.FindIndex(p => p.type == languageType);
            if (currentIndex == -1) return; 
            var nextIndex = (currentIndex == signLanguages.Count - 1) ? 0 : currentIndex + 1;
            type = signLanguages[nextIndex].type; 
        }
    }
}