using System.Collections;
using System.Threading.Tasks;
using Audio;
using TMPro;
using UnityEngine;

namespace UI.Screens
{
    public class ResultsScreen : UiWindow
    {
        [SerializeField] private TextMeshProUGUI  scoreText;
        
        private const string RESULTS_THEME = "Results";
        
        public Task ShowAsync(int scores)
        {
            scoreText.text = scores.ToString();
            return ShowAsync();
        }
        
        public IEnumerator Show(int scores)
        {
            scoreText.text = scores.ToString();
            return Show();
        }

        public override Task ShowAsync()
        {
            AudioPlayer.PlayTheme(RESULTS_THEME);
            return base.ShowAsync();
        }

        public override IEnumerator Show()
        {
            AudioPlayer.PlayTheme(RESULTS_THEME);
            return base.Show();
        }
    } // end of class
}
