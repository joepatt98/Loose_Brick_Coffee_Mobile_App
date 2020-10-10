using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LooseBrick.Droid
{
    // actual activity of splash page
    [Activity(Theme = "@style/Theme.Splash",
        MainLauncher = true, // sets it as starter launch
        NoHistory = true
       )]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // start main activity
            StartActivity(typeof(MainActivity));
        }
    }
}