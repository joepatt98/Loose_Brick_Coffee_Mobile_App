using Square;
using Square.Exceptions;
using Square.Models;
using System;
using System.Diagnostics;
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

        async void OnButtonClicked(object sender, EventArgs args)
        {
            if (Email.Text == null || Password.Text == null)
            {
                await DisplayAlert("Error", "Invalid Login, try again", "OK");
                return;
            }

            // Generates a new key (idempotency) each time the button is clicked.
            string key = Guid.NewGuid().ToString();

            // This is the Access Token for the Square Account being used to communicate
            // with the APIs in the application.
            string access_token = " "; // " EAAAEE4ZFnem1dGc-nNoec6nSD-IlO7F696yHDzNlv3gA3kU6ZYHZcijNe1I931X";

            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken(access_token)
                .Build();

            var emailAddress = new CustomerTextFilter.Builder()
              .Exact(Email.Text)
              .Build();

            var efilter = new CustomerFilter.Builder()
              .EmailAddress(emailAddress)
              .Build();

            var equery = new CustomerQuery.Builder()
              .Filter(efilter)
              .Build();

            var ebody = new SearchCustomersRequest.Builder()
              .Query(equery)
              .Build();

            var password = new CustomerTextFilter.Builder()
              .Exact(Password.Text)
              .Build();

            var pfilter = new CustomerFilter.Builder()
              .ReferenceId(password)
              .Build();

            var pquery = new CustomerQuery.Builder()
              .Filter(pfilter)
              .Build();

            var pbody = new SearchCustomersRequest.Builder()
              .Query(pquery)
              .Build();

            try
            {
                SearchCustomersResponse eresult = await client.CustomersApi.SearchCustomersAsync(body: ebody);
                SearchCustomersResponse presult = await client.CustomersApi.SearchCustomersAsync(body: pbody);

                if (eresult == null || presult == null)
                {
                    await DisplayAlert("Error", "We couldn't find that email and password combination. \nPlease try again!", "OK");
                }

                else
                {
                    var MainPage = new MainPage();
                    await Navigation.PushModalAsync(MainPage);
                }

            }

            catch (ApiException e)
            {
                Debug.WriteLine($"Exception: {e}");

                await DisplayAlert("Error", "We couldn't find that email and password combination. \nPlease try again!", "OK");
            }

        }

    }

}