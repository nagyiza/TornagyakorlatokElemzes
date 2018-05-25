using System;
using ExerciseAssistantApplication.Common;
using System.Windows;
using System.Diagnostics;
using System.IO;
using ExerciseAssistantApplication.Modell;
using SkeletonCompare;

namespace ExerciseAssistantApplication.ViewModell
{
    /// <summary>
    /// ViewModel for menu
    /// </summary>
    public class MenuViewModel : ViewModelBase
    {
        /// <summary>
        /// Username from email
        /// </summary>
        private string userName;
        /// <summary>
        /// Process for unity
        /// </summary>
        private Process process;
        /// <summary>
        /// Path of the data from unity
        /// </summary>
        private static String path = "..\\..\\..\\UnityData\\";

        /// <summary>
        /// Visibility for user 
        /// If is an admin, visible all buttons, or if is a user hidden admin's buttons
        /// </summary>
        private string visibility;
        /// <summary>
        /// Command for start button
        /// </summary>
        public RelayCommand Start { get; set; }
        /// <summary>
        /// Command for MyExercise button
        /// </summary>
        public RelayCommand MyExercise { get; set; }
        /// <summary>
        ///  Command for NewExercise button
        /// </summary>
        public RelayCommand NewExercise { get; set; }
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="isAdmin"></param>
        public MenuViewModel(bool isAdmin, string userName)
        {
            this.userName = userName;
            this.Start = new RelayCommand(StartClick, StartCancel);
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
        /// <summary>
        /// Getter and setter for visibility
        /// </summary>
        public string Visibility
        {
            get { return visibility; }
            set { visibility = value; }
        }
        /// <summary>
        /// Event for start button
        /// </summary>
        public void StartClick()
        {
            try
            {
                //Start the unity game
                process = new Process();
                process.StartInfo.FileName = "Skeleton3D.exe";

                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(myProcess_HasExited);
                process.Start();

                process.WaitForInputIdle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to UnityGame.exe.");
            }
        }

        private void myProcess_HasExited(object sender, EventArgs e)
        {
            MyDbContext db = new MyDbContext();
            char[] separators = { ' ' };
            StreamReader fileReader;
            if (File.Exists(path + "UnityData.txt"))
            {
                fileReader = new StreamReader(path + "UnityData.txt");
            }
            else
            {
                return;
            }

            string line;
            //read the skeleton data
            while ((line = fileReader.ReadLine()) != null)
            {
                // split the data 
                string[] words = line.Split(separators);
                string[] nameSplit = words[4].Split('.');
                CompareMain.Compare(nameSplit[0], false);// false - without the teaching
                string result = CompareMain.Result;

                db.Exercises.Add(new Exercise { Username = userName, ExerciseName = words[0], Date = words[1] + " " + words[2] + " " + words[3], Result = result });
                db.SaveChanges();
            }
            fileReader.Close();
            File.Delete(path + "UnityData.txt");
        }

        /// <summary>
        /// Event for cancel the start
        /// </summary>
        /// <returns></returns>
        public bool StartCancel()
        {
            return true;
        }
        /// <summary>
        /// Event for myexercise button
        /// </summary>
        public void MyExerciseClick()
        {
            MyExerciseViewModel mevm = new MyExerciseViewModel(userName);
            ViewService.ShowDialog(mevm);
        }
        /// <summary>
        /// Event for cancelk the myexercise 
        /// </summary>
        /// <returns></returns>
        public bool MyExerciseCancel()
        {
            return true;
        }
        /// <summary>
        /// Event for new exercise button
        /// </summary>
        public void NewExerciseClick()
        {
            //NewExerciseViewModel nevm = new NewExerciseViewModel();
            //ViewService.ShowDialog(nevm);
            ReferenceDataCollection.MainWindow m = new ReferenceDataCollection.MainWindow();
            m.Show();
        }
        /// <summary>
        /// Event for cancel the new exercise
        /// </summary>
        /// <returns></returns>
        public bool NewExerciseCancel()
        {
            return true;
        }

    }
}
