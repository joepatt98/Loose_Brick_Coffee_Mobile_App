using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace LooseBrick
{
    public partial class NewUserPage : ContentPage
    {
        public NewUserPage()
        {

            var vm = new LoginViewModel();
            this.BindingContext = vm;

            vm.DisplayInvalidLoginPrompt += () => DisplayAlert("Error", "Invalid Login, try again", "OK");

            var MainPage = new MainPage();
            vm.DisplayMainPage += () => Navigation.PushModalAsync(MainPage);

            InitializeComponent();

            Email.Completed += (object sender, EventArgs e) =>
            {

                Password.Focus();

            };

            Password.Completed += (object sender, EventArgs e) =>
            {

                vm.LoginCommand.Execute(null);

            };
        }
    }
}
