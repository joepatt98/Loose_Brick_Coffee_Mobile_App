using System;

using Xamarin.Forms;

namespace LooseBrick
{

    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed() => true;

        public async void AddToCart(object sender, EventArgs args)
        {
            (sender as Button).Text = "Added";
        }
    }
}
