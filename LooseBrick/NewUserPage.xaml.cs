using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Square;
using Square.Models;
using Square.Exceptions;
using System;
using System.Diagnostics;

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

        async void OnButtonClicked(object sender, EventArgs args)
        {
            // Generates a new key (idempotency) each time the button is clicked.
            string key = Guid.NewGuid().ToString();

            // This is the Access Token for the Square Account being used to communicate
            // with the APIs in the application.
            string access_token = "EAAAEEGUegliN33KdnaRMfEGKbSzgz723KNZ3IzrMA6cIZ1CdPJ-rS3Li7PJhyAD";

            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken(access_token)
                .Build();

            var body = new CreateCustomerRequest.Builder()
              .GivenName(FirstName.Text)
              .FamilyName(LastName.Text)
              .EmailAddress(Email.Text)
              .PhoneNumber(Phone.Text)
              .ReferenceId(Password.Text)
              .Build();

            try
            {
                var result = await client.CustomersApi.CreateCustomerAsync(body: body);

                //Debug
                (sender as Button).Text = "Success";
            }

            catch (ApiException e)
            {
                Debug.WriteLine("Failed to make the request");
                Debug.WriteLine($"Response Code: {e.ResponseCode}");
                Debug.WriteLine($"Exception: {e}");

                //Debug
                (sender as Button).Text = "Failure";
            }

        }

    }
}
