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
    }
}