using System.Collections;
using System.Linq;
using Audio;
using Settings;
using UI;
using UI.Screens;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Gameplay
{
    public class MissionManager : MonoBehaviour
    {
        [Header("Settings")]
        public float holePadding = 0.15f;
        public Vector3 gameAreaPosition = Vector3.zero;
        public Vector3 gameAreaSize = Vector3.zero;
        
        [Header("Resources")] 
        [SerializeField] private GameObject holePrefab;

        public event UnityAction OnMissionStarted;
        public event UnityAction OnMissionEnded;
        public event UnityAction<int> OnTimeUpdated;
        public event UnityAction<int> OnScoresUpdated;

        public static MissionManager Instance { get; private set; }
        public static PlayerController Player { get; private set; }
        public static int Scores => Instance?._scores ?? 0;
        public static bool IsMissionStarted => Instance?._isMissionActive ?? false;

        private HoleManager _holeManager;
        private float _remainingTime;
        private int _scores;
        private bool _isMissionActive;
        private Coroutine _missionCoroutine;

        private const string ROUND_DURATION = "Round Duration";
        private const string HOLES_COUNT = "Holes Count";
        private const string BUFF_TIME = "Buff time";
        private const string DEBUFF_TIME = "Debuff time";
        private const string SCORES_PER_HIT = "Scores per hit";
        private const string GAME_THEME = "Game";

        
        
        /********************** MONO BEHAVIOUR **********************/
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            _holeManager = new HoleManager(this, holePrefab, holePadding, transform.position + gameAreaPosition, gameAreaSize);
            Player = FindAnyObjectByType<PlayerController>();
            Assert.IsNotNull(Player, "PlayerController not found in the scene!");

            yield return null;
            InitializeAll(); // IoC principle
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + gameAreaPosition, gameAreaSize);
        }

        
        
        /******************** PUBLIC  INTERFACE ********************/
        
        public void StartMission()
        {
            ResetMissionState();
            ResetUiInformation();
            SpawnHoles();
            StartMissionTimer(EndMission);
            OnMissionStarted?.Invoke();
        }

        public async void EndMission()
        {
            if (_missionCoroutine != null)
                StopCoroutine(_missionCoroutine);
            
            _remainingTime = 0;
            _isMissionActive = false;
            _holeManager.ClearHoles();
            OnTimeUpdated?.Invoke((int)_remainingTime);
            OnMissionEnded?.Invoke();

            // Factory pattern
            (await UiMaster.GetOrCreateWindow<ResultsScreen>("Main")).ShowAsync(Scores);
        }

        
        
        /********************** INNER LOGIC **********************/
        
        private void ResetMissionState()
        {
            _remainingTime = GlobalVariables.GetFloat(ROUND_DURATION);
            _scores = 0;
            _isMissionActive = true;
            AudioPlayer.PlayTheme(GAME_THEME);
        }

        private void ResetUiInformation()
        {
            OnScoresUpdated?.Invoke(Scores);
            OnTimeUpdated?.Invoke((int)_remainingTime);
        }

        private void StartMissionTimer(UnityAction onTimeEnd)
        {
            _missionCoroutine = StartCoroutine(StartCountdown(onTimeEnd));
        }

        private void SpawnHoles()
        {
            int holesCount = GlobalVariables.GetInt(HOLES_COUNT);
            _holeManager.SpawnHoles(holesCount);
        }

        private IEnumerator StartCountdown(UnityAction onTimeEnd)
        {
            var tick = new WaitForSeconds(1f);
            while (_remainingTime > 0)
            {
                yield return tick;
                _remainingTime--;
                OnTimeUpdated?.Invoke((int)_remainingTime);
            }
            
            onTimeEnd?.Invoke();
        }

        public void AddScore(bool goodHole)
        {
            int score = goodHole ? GlobalVariables.GetInt(SCORES_PER_HIT) : 0;
            _scores += score;
            _scores = Mathf.Clamp(_scores, 0, _scores);
            OnScoresUpdated?.Invoke(_scores);

            float time = GlobalVariables.GetFloat(goodHole ? BUFF_TIME : DEBUFF_TIME);
            AddTime(time);
        }

        private void AddTime(float time)
        {
            _remainingTime += time;
            _remainingTime = Mathf.Max(0, _remainingTime);
            OnTimeUpdated?.Invoke((int)_remainingTime);
        }
        
        private void InitializeAll()
        {
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OfType<IInitializable>()
                .OrderBy(t => t.InitializationOrder)
                .ToList()
                .ForEach(t => t.Initialize(this));
        }
        
        
    }// end of class
}