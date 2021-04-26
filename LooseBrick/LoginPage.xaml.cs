<<<<<<< Updated upstream
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
=======
﻿using Xamarin.Forms;
>>>>>>> Stashed changes
using Xamarin.Forms.Xaml;
using Square;
using Square.Models;
using Square.Exceptions;
using System;
using System.Diagnostics;
<<<<<<< Updated upstream
using System.Linq;
=======
>>>>>>> Stashed changes

namespace LooseBrick
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public static bool LoggedIn { get; set; }

        public LoginPage()
        {

            var vm = new LoginViewModel();
            BindingContext = vm;

            vm.DisplayInvalidLoginPrompt += () => DisplayAlert("Error", "Invalid Login, Try again", "OK");

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

                //vm.LoginCommand.Execute(null);

            };

        }

        async void OnButtonClicked(object sender, EventArgs args)
        {
            if (Email.Text == "" || Password.Text == "")
            {
                await DisplayAlert("Error", "Invalid Login, Try Again", "OK");
                return;
            }

            // Generates a new key (idempotency) each time the button is clicked.
            string key = Guid.NewGuid().ToString();

            // This is the Access Token for the Square Account being used to communicate
            // with the APIs in the application.
            string access_token = "EAAAEEGUegliN33KdnaRMfEGKbSzgz723KNZ3IzrMA6cIZ1CdPJ-rS3Li7PJhyAD";
            // "EAAAEE4ZFnem1dGc-nNoec6nSD-IlO7F696yHDzNlv3gA3kU6ZYHZcijNe1I931X";
            // Joe's Test Token "EAAAEEGUegliN33KdnaRMfEGKbSzgz723KNZ3IzrMA6cIZ1CdPJ-rS3Li7PJhyAD"

            SquareClient client = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken(access_token)
                .Build();

            var emailAddress = new CustomerTextFilter.Builder()
              .Exact(Email.Text)
              .Build();

            var referenceId = new CustomerTextFilter.Builder()
              .Exact(Password.Text)
              .Build();

            var filter = new CustomerFilter.Builder()
              .EmailAddress(emailAddress)
              .ReferenceId(referenceId)
              .Build();

            var query = new CustomerQuery.Builder()
              .Filter(filter)
              .Build();

            var body = new SearchCustomersRequest.Builder()
              .Query(query)
              .Build();

            try
            {
                SearchCustomersResponse result = await client.CustomersApi.SearchCustomersAsync(body: body);

                if (result.Customers == null || result.Customers.Count == 0) // 
                {
                    LoggedIn = false;
                    await DisplayAlert("Error", "We couldn't find that email and password combination. Please try again!", "OK");
                }

                else
                {
                    LoggedIn = true;
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