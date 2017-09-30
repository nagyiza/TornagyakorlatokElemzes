using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExerciseAssistantApplication.Common
{
   public  class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RaisePropertyChanged([CallerMemberName]string name = null)// ha nem irok semmit akkor veszi a property nevet
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
