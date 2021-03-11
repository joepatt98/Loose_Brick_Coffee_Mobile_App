using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace LooseBrick
{

    public class LoginViewModel : INotifyPropertyChanged
    {

        public Action DisplayInvalidLoginPrompt;
        public Action DisplayMainPage;
        public Action DisplayNewUserPage;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string firstname;

        public string FirstName
        {

            get { return firstname; }

            set
            {

                firstname = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FirstName"));

            }

        }

        private string email;

        public string Email
        {

            get { return email; }

            set
            {

                email = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Email"));

            }

        }

        private string password;

        public string Password
        {

            get { return password; }
            set
            {
                password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }

        }

        public ICommand LoginCommand { protected set; get; }
        public ICommand PreCreateCommand { protected set; get; }

        public LoginViewModel()
        {

            LoginCommand = new Command(OnSubmit);
            PreCreateCommand = new Command(OnPreCreate);

        }

        public void OnSubmit()
        {

            if (email != "email" || password != "password")
            {

                DisplayInvalidLoginPrompt();

            }

            else
            {
                DisplayMainPage();
            }

        }

        public void OnPreCreate()
        {

            DisplayNewUserPage();

        }

    }
}