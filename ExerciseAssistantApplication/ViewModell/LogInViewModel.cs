using ExerciseAssistantApplication.Common;
using ExerciseAssistantApplication.Modell;
using ExerciseAssistantApplication.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExerciseAssistantApplication.ViewModell
{
    /// <summary>
    /// ViewModel for log in screen
    /// </summary>
    public class LogInViewModel : ViewModelBase
    {
        /// <summary>
        /// Edit box foe email
        /// </summary>
        private string email_box;
        /// <summary>
        /// Edit box for password
        /// </summary>
        public string upassword;
        /// <summary>
        /// Command for OK button
        /// </summary>
        public RelayCommand OkCommand { get; set; }
        /// <summary>
        /// Command for Back button
        /// </summary>
        public RelayCommand BackCommand { get; set; }
        /// <summary>
        /// The constructor
        /// </summary>
        public LogInViewModel()
        {
            this.OkCommand = new RelayCommand(Ok, Cancel);
            this.BackCommand = new RelayCommand(Back, Cancel);
        }
        /// <summary>
        /// Event for OK button
        /// </summary>
        public void Ok()
        {
            try
            {
                //check the user, and if is an admin, show the admin's menu
                int isAdmin = AuthenticateEmail(Email_box, uPassword);
                string[] emailSplit = Email_box.Split('@');
                if (isAdmin == 1)// if login with success and is Admin
                {
                    ViewService.CloseDialog(this);
                    MenuViewModel mvm = new MenuViewModel(true, emailSplit[0]);
                    ViewService.ShowDialog(mvm);
                }
                else
                {
                    if (isAdmin == 2)// if login with success and is User
                    { 
                        ViewService.CloseDialog(this);
                        MenuViewModel mvm = new MenuViewModel(false, emailSplit[0]);
                        ViewService.ShowDialog(mvm);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {

                MessageBox.Show("ERROR: Invalid email or invalid password!" );
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: Invalid email or invalid password!");

            }
        }
        /// <summary>
        /// Variable for user
        /// </summary>
        public User cUser { get; set; }

        /// <summary>
        /// Autentification with email and password
        /// Using the local database
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="password">The user's password</param>
        /// <returns> 0 - invalid user, 1 - admin, 2 - user </returns>
        public int AuthenticateEmail(string email, string password)
        {
            using (MyDbContext db = new MyDbContext())
            {
                //string hashPwd = CalculateHash(password, email);
                cUser = db.Users.FirstOrDefault(u => u.Email.Equals(email) && u.Password.Equals(uPassword));
                if (cUser == null)
                    throw new UnauthorizedAccessException("invalid email or password");
                else
                {
                    if (cUser.IsAdmin)
                    {
                        return 1; // admin
                    }
                    else
                    {
                        return 2; // user
                    }
                }

            }
        }

        /// <summary>
        /// Event for Back button
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
            //if (string.IsNullOrWhiteSpace(this.Email) || string.IsNullOrWhiteSpace(this.uPassword))
            //    return false;
            //else
                return true;
        }

        /// <summary>
        /// Getter and setter for password
        /// </summary>
        public string uPassword
        {
            get { return upassword; }
            set
            {
                upassword = value;
                this.OnPropertyChanged("uPassword");
            }
        }
        /// <summary>
        /// Getter and setter for email
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
    }
}
