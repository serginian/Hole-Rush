using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public struct AudioMap
    {
        public string name;
        public AudioClip clip;
        public bool isTheme;
    }
}