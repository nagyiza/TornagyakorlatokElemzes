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
    public class LogInViewModel : ViewModelBase
    {
        private string email_box;
        public string upassword;
        public RelayCommand OkCommand { get; set; }
        public RelayCommand BackCommand { get; set; }

        public LogInViewModel()
        {
            this.OkCommand = new RelayCommand(Ok, Cancel);
            this.BackCommand = new RelayCommand(Back, Cancel);
        }

        public void Ok()
        {
            try
            {
                int isAdmin = AuthenticateEmail(Email_box, uPassword);
                if (isAdmin == 1)// if login with success and is Admin
                {
                    ViewService.CloseDialog(this);
                    MenuViewModel mvm = new MenuViewModel(true);
                    ViewService.ShowDialog(mvm);
                }
                else
                {
                    if (isAdmin == 2)// if login with success and is User
                    { 
                        ViewService.CloseDialog(this);
                        MenuViewModel mvm = new MenuViewModel(false);
                        ViewService.ShowDialog(mvm);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {

                MessageBox.Show("ERROR: Invalid email or invalid password!" );
                //ez nem kell csak amig nem megy addig itt marad:
                ViewService.CloseDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: Invalid email or invalid password!");

            }
        }
        public User cUser { get; set; }
        
        //return: 0 - invalid user, 1 - admin, 2 - user
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


        //public string CalculateHash(string clearTextPassword, string salt)
        //{
        //    byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
        //    HashAlgorithm algorithm = new SHA256Managed();
        //    byte[] hash = algorithm.ComputeHash(saltedHashBytes);
        //    return Convert.ToBase64String(hash);
        //}


        public void Back()
        {
            ViewService.CloseDialog(this);
        }
        private bool Cancel()
        {
            //if (string.IsNullOrWhiteSpace(this.Email) || string.IsNullOrWhiteSpace(this.uPassword))
            //    return false;
            //else
                return true;
        }

        
        public string uPassword
        {
            get { return upassword; }
            set
            {
                upassword = value;
                this.OnPropertyChanged("uPassword");
            }
        }
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
