using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LooseBrick
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {

        public LoginPage()
        {

            var vm = new LoginViewModel();
            this.BindingContext = vm;

            vm.DisplayInvalidLoginPrompt += () => DisplayAlert("Error", "Invalid Login, try again", "OK");

            var MainPage = new MainPage();
            vm.DisplayMainPage += () => Navigation.PushModalAsync(MainPage);

            var NewUserPage = new NewUserPage();
            vm.DisplayNewUserPage += () => Navigation.PushModalAsync(NewUserPage);

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