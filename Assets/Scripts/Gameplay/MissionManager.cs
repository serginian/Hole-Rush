using System.Collections;
using System.Collections.Generic;
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
        [Header("Settings")] public bool autoStart = false;
        public float holePadding = 0.15f;
        public Vector3 gameAreaPosition = Vector3.zero;
        public Vector3 gameAreaSize = Vector3.zero;
        [Header("Resources")] [SerializeField] private GameObject holePrefab;

        public event UnityAction OnMissionStarted;
        public event UnityAction OnMissionEnded;
        public event UnityAction<int> OnTimeUpdated;
        public event UnityAction<int> OnScoresUpdated;

        public static MissionManager Instance { get; private set; }
        public static PlayerController Player { get; private set; }
        public static int Scores => Instance?._scores ?? 0;
        public static bool IsMissionStarted => Instance?._isMissionActive ?? false;

        private readonly List<Hole> _holes = new List<Hole>();
        
        private const string ROUND_DURATION = "Round Duration";
        private const string HOLES_COUNT = "Holes Count";
        private const string BUFF_TIME = "Buff time";
        private const string DEBUFF_TIME = "Debuff time";
        private const string SCORES_PER_HIT = "Scores per hit";
        private const string GAME_THEME = "Game";
        
        private float _remainingTime;
        private int _scores;
        private bool _isMissionActive;
        private Coroutine _missionCoroutine;



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
            Player = FindAnyObjectByType<PlayerController>();
            Assert.IsNotNull(Player, "PlayerController not found in the scene!");

            yield return null;
            InitializeAll(); // initialize gameplay elements after the start in specified order

            if (autoStart)
                StartMission();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + gameAreaPosition, gameAreaSize);
        }



        /********************** PUBLIC INTERFACE **********************/
        public void StartMission()
        {
            _remainingTime = GlobalVariables.GetFloat(ROUND_DURATION);
            _scores = 0;
            _isMissionActive = true;
            AudioPlayer.PlayTheme(GAME_THEME);
            _missionCoroutine = StartCoroutine(StartCountdown());
            SpawnHoles();
            OnScoresUpdated?.Invoke(Scores);
            OnTimeUpdated?.Invoke((int) _remainingTime);
            OnMissionStarted?.Invoke();
        }

        public async void EndMission()
        {
            if (_missionCoroutine != null)
                StopCoroutine(_missionCoroutine);

            _holes.Clear();
            _remainingTime = 0;
            _isMissionActive = false;
            OnTimeUpdated?.Invoke((int) _remainingTime);
            OnMissionEnded?.Invoke();
            await (await UiMaster.GetOrCreateWindow<ResultsScreen>("Main")).ShowAsync(Scores);
        }



        /********************** INNER LOGIC **********************/
        private void InitializeAll()
        {
            // Inverse of Control
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OfType<IInitializable>().OrderBy(t => t.InitializationOrder).ToList().ForEach(t => t.Initialize(this));
        }

        private void SpawnHoles()
        {
            HoleState holeState = HoleState.Good;
            int holesCount = GlobalVariables.GetInt(HOLES_COUNT);
            for (int i = 0; i < holesCount; i++)
            {
                Vector3 position = GetRandomHolePosition();
                var holeObj = Instantiate(holePrefab, position, Quaternion.identity);
                var hole = holeObj.GetComponent<Hole>();
                hole.Initialize(this, holeState, OnHoleHit);
                holeState = holeState == HoleState.Good ? HoleState.Bad : HoleState.Good;
                _holes.Add(hole);
            }
        }

        private Vector3 GetRandomHolePosition()
        {
            // get game zone's bounds
            Vector2 boundsX = new Vector2()
            {
                x = transform.position.x + gameAreaPosition.x - gameAreaSize.x / 2f,
                y = transform.position.x + gameAreaPosition.x + gameAreaSize.x / 2f
            };
            Vector2 boundsZ = new Vector2()
            {
                x = transform.position.z + gameAreaPosition.z - gameAreaSize.z / 2f,
                y = transform.position.z + gameAreaPosition.z + gameAreaSize.z / 2f
            };

            // get a position that does not intersect with any other hole on the level
            Vector3 position;
            do
            {
                float x = Random.Range(boundsX.x, boundsX.y);
                float z = Random.Range(boundsZ.x, boundsZ.y);
                position = new Vector3(x, transform.position.y + gameAreaPosition.y, z);
            } while (_holes.Any(h => Vector3.Distance(h.transform.position, position) < holePadding));

            return position;
        }

        private void OnHoleHit(Hole hole)
        {
            bool goodHole = hole.State == HoleState.Good;
            if (goodHole)
                AddScore(GlobalVariables.GetInt(SCORES_PER_HIT));
            AddTime(GlobalVariables.GetFloat(goodHole ? BUFF_TIME : DEBUFF_TIME));

            hole.Disappear(() =>
            {
                hole.transform.position = GetRandomHolePosition();
                hole.Appear();
            });
        }

        private IEnumerator StartCountdown()
        {
            var tick = new WaitForSeconds(1f);
            
            while (_remainingTime > 0)
            {
                yield return tick;
                _remainingTime--;
                OnTimeUpdated?.Invoke((int) _remainingTime);
            }

            EndMission();
        }

        private void AddScore(int score)
        {
            _scores += score;
            _scores = Mathf.Clamp(_scores, 0, _scores);
            OnScoresUpdated?.Invoke(_scores);
        }

        private void AddTime(float time)
        {
            _remainingTime += time;
            _remainingTime = Mathf.Max(0, _remainingTime);
            OnTimeUpdated?.Invoke((int) _remainingTime);
        }

        
    } // end of class
}