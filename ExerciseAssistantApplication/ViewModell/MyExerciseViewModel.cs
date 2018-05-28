using ExerciseAssistantApplication.Common;
using ExerciseAssistantApplication.Modell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseAssistantApplication.ViewModell
{
    public class MyExerciseViewModel : ViewModelBase
    {
        /// <summary>
        /// The item of exercise from database
        /// </summary>
        private List<string> exerciseList;
        /// <summary>
        /// The exercise name from database
        /// </summary>
        private string exerciseName;
        /// <summary>
        /// The date from database
        /// </summary>
        private string date;
        /// <summary>
        /// The result from database
        /// </summary>
        private string resultJoint;
        /// <summary>
        /// The result from database
        /// </summary>
        private string resultAngle;
        /// <summary>
        /// Command for Back button
        /// </summary>
        public RelayCommand BackCommand { get; set; }
         

        public MyExerciseViewModel(string userName)
        {
            this.BackCommand = new RelayCommand(Back, Cancel);
            List<Exercise> exercisesForUser = new List<Exercise>();
            using (MyDbContext db = new MyDbContext())
            {
                exercisesForUser = db.Exercises.Where(e => e.Username.Equals(userName)).ToList();
            }

            for (int i = 0; i < exercisesForUser.Count; ++i)
            {
                exerciseName += exercisesForUser[i].ExerciseName + "\n";

                date += exercisesForUser[i].Date + "\n";

                resultJoint += exercisesForUser[i].ResultJoint + "\n";

                resultAngle += exercisesForUser[i].ResultAngle + "\n";
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
            return true;
        }
        /// <summary>
        /// Getter and setter for exercise name
        /// </summary>
        public string ExerciseName
        {
            get { return this.exerciseName; }
            set
            {
                this.exerciseName = value;
                this.OnPropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
            }
        }
        /// <summary>
        /// Getter and setter for item's date
        /// </summary>
        public string Date
        {
            get { return this.date; }
            set
            {
                this.date = value;
                this.OnPropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
            }
        }
        /// <summary>
        /// Getter and setter for item's result
        /// </summary>
        public string ResultJoint
        {
            get { return this.resultJoint; }
            set
            {
                this.resultJoint = value;
                this.OnPropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
            }
        }
        /// <summary>
        /// Getter and setter for item's result
        /// </summary>
        public string ResultAngle
        {
            get { return this.resultAngle; }
            set
            {
                this.resultAngle = value;
                this.OnPropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
            }
        }
    }
}
