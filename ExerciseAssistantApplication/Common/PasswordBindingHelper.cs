
namespace ExerciseAssistantApplication.Common
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Password binding helper class
    /// </summary>
    public static class PasswordBindingHelper
    {
        /// <summary>
        /// The password property
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
          DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBindingHelper), new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        /// <summary>
        /// The updating password
        /// </summary>
        private static readonly DependencyProperty UpdatingPassword =
         DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBindingHelper), new PropertyMetadata(false));

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>Password value</returns>
        public static string GetPassword(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(PasswordProperty);
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetPassword(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(PasswordProperty, value);
        }

        /// <summary>
        /// Gets the updating password.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>Password is updated value</returns>
        private static bool GetUpdatingPassword(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(UpdatingPassword);
        }

        /// <summary>
        /// Sets the updating password.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private static void SetUpdatingPassword(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(UpdatingPassword, value);
        }

        /// <summary>
        /// Called when [password property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox != null)
            {
                passwordBox.PasswordChanged -= PasswordChanged;

                if (!(bool)GetUpdatingPassword(passwordBox))
                {
                    passwordBox.Password = (string)e.NewValue;
                }

                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        /// Passwords the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox != null)
            {
                SetUpdatingPassword(passwordBox, true);
                SetPassword(passwordBox, passwordBox.Password);
                SetUpdatingPassword(passwordBox, false);
            }
        }
    }
}
