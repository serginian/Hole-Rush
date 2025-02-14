using Gameplay;
using TMPro;
using UnityEngine;

namespace UI.Screens
{
    public class GameScreen : UiWindow, IInitializable
    {
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI timerText;

        
        
        /********************** MONO BEHAVIOUR **********************/
        protected override void Awake()
        {
            base.Awake();
            CloseWithoutAnimation();
        }
        
        
        /********************** INITIALIZATION **********************/
        
        public int InitializationOrder => 1;
        public void Initialize(MissionManager manager)
        {
            manager.OnScoresUpdated += UpdateScores;
            manager.OnTimeUpdated += UpdateTime;
            manager.OnMissionStarted += OnMissionStarted;
            manager.OnMissionEnded += OnMissionEnded;
        }

        
        
        /********************** INNER LOGIC **********************/
        
        private async void OnMissionEnded()
        {
            await CloseAsync();
        }

        private async void OnMissionStarted()
        {
            await ShowAsync();
        }

        private void UpdateTime(int timer)
        {
            timerText.text = timer.ToString();
        }

        private void UpdateScores(int score)
        {
            scoreText.text = $"Scores: {score}";
        }
        
        
    } // end of class
}
