using ExerciseAssistantApplication.Common;
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
        /// Command for Back button
        /// </summary>
        public RelayCommand BackCommand { get; set; }

        public MyExerciseViewModel()
        {
            this.BackCommand = new RelayCommand(Back, Cancel);
            
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

    }
}
