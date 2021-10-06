using System.Diagnostics;
using Xamarin.Forms;

namespace titanfall2_rp.Windows
{
    public class ActualApp : Application
    {
        public ActualApp()
        {
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            base.OnStart();
            Debug.WriteLine("Application started");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            Debug.WriteLine("Application sleeps");
        }

        protected override void OnResume()
        {
            base.OnResume();
            Debug.WriteLine("Application resumes");
        }
    }
}