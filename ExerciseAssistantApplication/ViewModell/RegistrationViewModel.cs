using ExerciseAssistantApplication.Common;
using ExerciseAssistantApplication.Modell;

namespace ExerciseAssistantApplication.ViewModell
{
    /// <summary>
    /// ViewModel for registration
    /// </summary>
    public class RegistrationViewModel : ViewModelBase
    {
        /// <summary>
        /// Edit box for username
        /// </summary>
        private string username_box;
        /// <summary>
        /// Edit box for email
        /// </summary>
        private string email_box;
        /// <summary>
        /// Edit box for password
        /// </summary>
        private string password_box;
        /// <summary>
        /// Edit box for password (again)
        /// </summary>
        private string passwordagain_box;
        /// <summary>
        /// Command for Ok button
        /// </summary>
        public RelayCommand OkCommand { get; set; }
        /// <summary>
        /// Command for Back button
        /// </summary>
        public RelayCommand BackCommand { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistrationViewModel()
        {
            this.OkCommand = new RelayCommand(OkClick, Cancel);
            this.BackCommand = new RelayCommand(Back, Cancel);
        }
        /// <summary>
        /// Event for OK button
        /// </summary>
        public void OkClick()
        {

            if (username_box != null && email_box != null && checkEmail(email_box) && upassword != null && (upassword == upassword2))
            {
                string[] emailSplit = Email_box.Split('@');
                ViewService.CloseDialog(this);
                MenuViewModel mvm = new MenuViewModel(false, emailSplit[0]);
                ViewService.ShowDialog(mvm);
                MyDbContext db = new MyDbContext();

                db.Users.Add(new User { Username = username_box, Password = upassword, Email = email_box, IsAdmin = false, IsActive = true });
                db.SaveChanges();
            }


        }
        /// <summary>
        /// Check email format
        /// </summary>
        /// <param name="email"></param>
        /// <returns> true - if the email is correct, false - if the email is not correct</returns>
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
        /// <summary>
        /// Event for back button
        /// </summary>
        public void Back()
        {
            ViewService.CloseDialog(this);

        }
        /// <summary>
        /// Event for cancel
        /// </summary>
        /// <returns></returns>
        private bool Cancel()
        {

            return true;
        }

        //---variables for binding
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
        //------
        /// <summary>
        /// Getter and setter the email
        /// </summary>
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
        /// <summary>
        /// Getter and setter the username
        /// </summary>
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
        /// <summary>
        /// Getter and setter the password
        /// </summary>
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
        /// <summary>
        /// Getter and setter the password
        /// </summary>
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
