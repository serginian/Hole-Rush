using System;
using UnityEngine;

namespace Audio
{
    public class AudioProfile : MonoBehaviour
    {
        public bool unloadOnStart;
        public AudioMap[] audioLibrary;
        public bool playThemeOnStart = false;
        public string themeName;

        private void Start()
        {
            if (!AudioPlayer.IsAvailable)
                throw new ApplicationException("Audio player was not created before trying to use it");

            if (unloadOnStart)
                AudioPlayer.UnloadAll();

            foreach (var a in audioLibrary)
                AudioPlayer.LoadAudioClip(a.name, a.clip);

            if (playThemeOnStart)
                AudioPlayer.PlayTheme(themeName);
        }
    }
}