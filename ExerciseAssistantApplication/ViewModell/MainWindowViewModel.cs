using ExerciseAssistantApplication.Common;
using ExerciseAssistantApplication.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseAssistantApplication.ViewModell
{
    /// <summary>
    /// ViewModel for first screen
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Command for sign in
        /// </summary>
        public RelayCommand SignInCommand { get; set; }
        /// <summary>
        /// Command for registration
        /// </summary>
        public RelayCommand RegistrationCommand { get; set; }
        /// <summary>
        /// The constructor
        /// </summary>
        public MainWindowViewModel()
        {
            this.SignInCommand = new RelayCommand(SignInClick, SignInCancel);
            this.RegistrationCommand = new RelayCommand(Registration, RegistrationCancel);
        }
        /// <summary>
        /// Event for sign in button
        /// </summary>
        public void SignInClick()
        {
            //hide before startup
            LogInViewModel blpvm = new LogInViewModel();
            ViewService.ShowDialog(blpvm);
        }
        /// <summary>
        /// Event for cancel the sign in
        /// </summary>
        private bool SignInCancel()
        {
            return true;
        }
        /// <summary>
        /// Event for registration button
        /// </summary>
        private void Registration()
        {
            RegistrationViewModel rvm = new RegistrationViewModel();
            ViewService.ShowDialog(rvm);
        }
        /// <summary>
        /// Event for cancel the registration
        /// </summary>
        private bool RegistrationCancel()
        {
            return true;
        }
    }
}
