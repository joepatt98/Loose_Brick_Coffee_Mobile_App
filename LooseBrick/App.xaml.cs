using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LooseBrick
{
    public partial class App : Application
    {
        public static bool Loggedin { get; set; }

        public App()
        {
            InitializeComponent();

            if (Loggedin == false)
            {

                MainPage = new NavigationPage(new LoginPage());

            }

            else
            {

                MainPage = new NavigationPage(new MainPage());

            }

        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {

        }

        protected override void OnResume()
        {

        }

    }
}
