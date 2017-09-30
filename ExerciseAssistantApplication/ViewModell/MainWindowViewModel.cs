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
    public class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand SignInCommand { get; set; }
        public RelayCommand RegistrationCommand { get; set; }

        public MainWindowViewModel()
        {
            this.SignInCommand = new RelayCommand(SignInClick, SignInCancel);
            this.RegistrationCommand = new RelayCommand(Registration, RegistrationCancel);

            this.NewExercise = new RelayCommand(NewExerciseClick, NewExerciseCancel);
        }

        public void SignInClick()
        {
            //hide before startup
            LogInViewModel blpvm = new LogInViewModel();
            ViewService.ShowDialog(blpvm);
        }
        private bool SignInCancel()
        {
            return true;
        }
        private void Registration()
        {
            RegistrationViewModel rvm = new RegistrationViewModel();
            ViewService.ShowDialog(rvm);
        }
        private bool RegistrationCancel()
        {
            return true;
        }

        public RelayCommand NewExercise { get; set; }
        public void NewExerciseClick()
        {
            //NewExerciseViewModel nevm = new NewExerciseViewModel();
            //ViewService.ShowDialog(nevm);

            ReferenceDataCollection.MainWindow m = new ReferenceDataCollection.MainWindow();
            m.Show();
        }
        public bool NewExerciseCancel()
        {
            return true;
        }
    }
}
