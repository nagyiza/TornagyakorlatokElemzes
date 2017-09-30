using ExerciseAssistantApplication.Common;
using ExerciseAssistantApplication.Modell;
using ExerciseAssistantApplication.ViewModell;
using ExerciseAssistantApplication.View;
using System.Windows;

namespace ExerciseAssistantApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void InitializeDB()
        {
            DBInitializer db = new DBInitializer();
            db.InitializeDatabase(new MyDbContext());

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeDB();
            registerViews();
            OpenMainWindow();
        }
        private void registerViews()
        {
            ViewService.RegisterView(typeof(MainWindowViewModel), typeof(MainWindow));
            ViewService.RegisterView(typeof(LogInViewModel), typeof(LogIn));
            ViewService.RegisterView(typeof(RegistrationViewModel), typeof(Registration));
            ViewService.RegisterView(typeof(MenuViewModel), typeof(Menu));
            ViewService.RegisterView(typeof(NewExerciseViewModel), typeof(NewExercise));
            //ViewService.RegisterView(typeof(SearchViewModel), typeof(Search));
            //ViewService.RegisterView(typeof(ModificationViewModel), typeof(Modification));
            //ViewService.RegisterView(typeof(AllUserViewModel), typeof(AllUser));
        }
        private void OpenMainWindow()
        {
            MainWindowViewModel mwvm = new MainWindowViewModel();
            MainWindow mw = new MainWindow();
            ViewService.AddMainWindowToOpened(mwvm, mw);
            ViewService.ShowDialog(mwvm);
        }
    }
}
