using ExerciseAssistantApplication.Common;
using ExerciseAssistantApplication.Modell;

namespace ExerciseAssistantApplication.ViewModell
{
    public class RegistrationViewModel : ViewModelBase
    {
        private string username_box;
        private string email_box;
        private string password_box;
        private string passwordagain_box;

        public RelayCommand OkCommand { get; set; }
        public RelayCommand BackCommand { get; set; }


        public RegistrationViewModel()
        {
            this.OkCommand = new RelayCommand(OkClick, Cancel);
            this.BackCommand = new RelayCommand(Back, Cancel);
        }

        public void OkClick()
        {

            if (username_box != null && email_box != null && checkEmail(email_box) && upassword != null && (upassword == upassword2))
            {
                ViewService.CloseDialog(this);
                MenuViewModel mvm = new MenuViewModel(false);
                ViewService.ShowDialog(mvm);
                MyDbContext db = new MyDbContext();

                db.Users.Add(new User { Username = username_box, Password = upassword, Email = email_box, IsAdmin = false, IsActive = true });
                db.SaveChanges();
            }


        }
        public bool checkEmail(string email)
        {
            string[] words = email.Split('@');
            if (words.Length == 2)
            {
                string[] words2 = words[1].Split('.');
                if (words2.Length >= 2)
                {
                    if (words2[words2.Length - 1] == "com" || words2[words2.Length - 1] == "hu" || words2[words2.Length - 1] == "ro")
                    {
                        return true;
                    }
                    else return false;

                }
                else return false;

            }
            else return false;

        }
        public void Back()
        {
            ViewService.CloseDialog(this);

        }
        private bool Cancel()
        {

            return true;
        }

        //variables for binding

        public string upassword;
        public string uPassword
        {
            get { return upassword; }
            set
            {
                upassword = value;
                this.OnPropertyChanged("uPassword");
            }
        }
        public string upassword2;
        public string uPassword2
        {
            get { return upassword2; }
            set
            {
                upassword2 = value;
                this.OnPropertyChanged("uPassword2");
            }
        }
        
        //getting text box content
        public string Email_box
        {
            get { return this.email_box; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this.email_box, value))
                {
                    this.email_box = value;
                    this.OnPropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }

        public string Username_box
        {
            get { return this.username_box; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this.username_box, value))
                {
                    this.username_box = value;
                    this.OnPropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        public string Password_box
        {
            get { return this.password_box; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this.password_box, value))
                {
                    this.password_box = value;
                    this.OnPropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        public string PasswordAgain_box
        {
            get { return this.passwordagain_box; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this.passwordagain_box, value))
                {
                    this.passwordagain_box = value;
                    this.OnPropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
    }
}
