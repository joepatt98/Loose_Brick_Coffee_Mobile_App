using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
