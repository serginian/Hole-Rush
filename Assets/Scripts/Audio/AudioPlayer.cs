using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public sealed class AudioPlayer : MonoBehaviour
    {
        private static AudioPlayer instance;

        [Header("Settings")] 
        [SerializeField] private int streamsCount = 5;
        [SerializeField] private float smoothTime = 1f;

        [Header("Mixer Groups")]
        [SerializeField] private AudioMixerGroup gameplayGroup;
        [SerializeField] private AudioMixerGroup uiGroup;
        [SerializeField] private AudioMixerGroup musicGroup;

        public static bool IsAvailable => instance != null;

        private static readonly Dictionary<AudioGroup, AudioMixerGroup> groupMappings =
            new Dictionary<AudioGroup, AudioMixerGroup>();

        private static readonly Dictionary<string, AudioClip> audioMappings = new Dictionary<string, AudioClip>();
        private static AudioSource[] _sources;
        private static readonly AudioSource[] _themeSources = new AudioSource[2];
        private static int _curThemeStream = -1;
        private static int _curSourceIndex = 0;
        private static string currentTheme = string.Empty;


        
        /******************************* MONO BEHAVIOUR *******************************/

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        private void Initialize()
        {
            groupMappings.Clear();
            groupMappings.Add(AudioGroup.Gameplay, gameplayGroup);
            groupMappings.Add(AudioGroup.UI, uiGroup);
            groupMappings.Add(AudioGroup.Music, musicGroup);

            _sources = new AudioSource[streamsCount];
            for (int i = 0; i < streamsCount; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.loop = false;
                source.playOnAwake = false;
                _sources[i] = source;
            }

            _themeSources[0] = gameObject.AddComponent<AudioSource>();
            _themeSources[0].loop = true;
            _themeSources[0].playOnAwake = false;
            _themeSources[1] = gameObject.AddComponent<AudioSource>();
            _themeSources[1].loop = true;
            _themeSources[1].playOnAwake = false;
        }


        /******************************* PUBLIC INTERFACE *******************************/
        public static void LoadAudioClip(string name, AudioClip clip)
        {
            audioMappings[name] = clip;
        }

        public static void UnloadAudioClip(string name)
        {
            if (!audioMappings.ContainsKey(name)) return;
            var obj = audioMappings[name];
            audioMappings.Remove(name);
            Resources.UnloadAsset(obj);
        }

        public static void UnloadAll()
        {
            audioMappings.Clear();
            Resources.UnloadUnusedAssets();
        }

        public static void PlaySound(AudioClip clip, AudioGroup group, float volume = 1f)
        {
            var audio = _sources[_curSourceIndex];
            audio.Stop();
            audio.volume = volume;
            audio.clip = clip;
            audio.outputAudioMixerGroup = groupMappings[group];
            audio.Play();
            _curSourceIndex++;
            if (_curSourceIndex == _sources.Length)
                _curSourceIndex = 0;
        }

        public static void PlaySound(string clip, AudioGroup group, float volume = 1f)
        {
            if (audioMappings.ContainsKey(clip))
                PlaySound(audioMappings[clip], group, volume);
            else
                Debug.LogError("ERROR: Audio manager has no requested sound: " + clip);
        }

        public static void PlayTheme(string clip)
        {
            if (currentTheme == clip)
                return;

            if (!audioMappings.ContainsKey(clip))
            {
                StopTheme();
                return;
            }

            if (_curThemeStream >= 0)
                StopTheme();

            _curThemeStream = (_curThemeStream + 1) % _themeSources.Length;

            _themeSources[_curThemeStream].clip = audioMappings[clip];
            _themeSources[_curThemeStream].volume = 0f;
            _themeSources[_curThemeStream].outputAudioMixerGroup = groupMappings[AudioGroup.Music];
            _themeSources[_curThemeStream].Play();
            _themeSources[_curThemeStream].DOFade(1f, instance.smoothTime);

            currentTheme = clip;
        }

        public static void StopTheme()
        {
            if (_curThemeStream < 0)
                return;

            var curStream = _themeSources[_curThemeStream];
            curStream.DOFade(0f, instance.smoothTime).onComplete = () => curStream.Stop();
            //_curThemeStream = -1;
        }

        public static void MuteTheme()
        {
            MuteTheme(instance.smoothTime);
        }

        public static void UnmuteTheme()
        {
            UnmuteTheme(instance.smoothTime);
        }

        public static void MuteTheme(float smoothing)
        {
            var curStream = _themeSources[_curThemeStream];
            curStream.DOFade(0f, smoothing);
        }

        public static void UnmuteTheme(float smoothing)
        {
            var curStream = _themeSources[_curThemeStream];
            curStream.DOFade(1f, smoothing);
        }
        
    } // end of class
}