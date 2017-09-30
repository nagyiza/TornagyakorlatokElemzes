using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ExerciseAssistantApplication.Common;
using ExerciseAssistantApplication.Modell;
using System.Windows.Controls;
using System.Windows;

namespace ExerciseAssistantApplication.ViewModell
{
    public class MenuViewModel : ViewModelBase
    {
        private string visibility;
        public RelayCommand Start { get; set; }
        public RelayCommand MyExercise { get; set; }
        public RelayCommand NewExercise { get; set; }
        public MenuViewModel(bool isAdmin)
        {
            this.Start = new RelayCommand(StartClick,StartCancel);
            this.MyExercise = new RelayCommand(MyExerciseClick, MyExerciseCancel);
            this.NewExercise = new RelayCommand(NewExerciseClick, NewExerciseCancel);
            if (isAdmin)
            {
                //NewExercise button visible
                Visibility = "Visible";
            }
            else
            {
                //NewExercise button hide
                Visibility = "Hidden";
            }

        }

        public string Visibility
        {
            get { return visibility; }
            set { visibility = value; }
        }
        public void StartClick()
        {
            //AddUserViewModel auvm = new AddUserViewModel();
            //ViewService.ShowDialog(auvm);
        }
        public bool StartCancel()
        {
            return true;
        }
        public void MyExerciseClick()
        {
            //SearchViewModel svm = new SearchViewModel();
            //ViewService.ShowDialog(svm);
        }
        public bool MyExerciseCancel()
        {
            return true;
        }
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
