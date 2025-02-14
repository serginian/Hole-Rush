 using System.Collections;
 using System.Threading.Tasks;
 using Audio;

 namespace UI.Screens
{
    public class WelcomeScreen : UiWindow
    {
        public bool showOnStart;
        
        private const string WELCOME_THEME = "Welcome";
        
        IEnumerator Start()
        {
            if (showOnStart)
                yield return Show();
        }

        public override IEnumerator Show()
        {
            AudioPlayer.PlayTheme(WELCOME_THEME);
            return base.Show();
        }

        public override Task ShowAsync()
        {
            AudioPlayer.PlayTheme(WELCOME_THEME);
            return base.ShowAsync();
        }
        
    } // end of class
}
