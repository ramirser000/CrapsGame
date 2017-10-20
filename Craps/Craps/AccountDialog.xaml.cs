using System;
using System.Windows;


namespace Craps
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowAccountDialog : Window
    {
        private WindowCraps mainWindow;

        public WindowAccountDialog(WindowCraps mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            mainWindow.enable(false);
        }

        private void windowAccountDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            if ((bool)checkBoxBets.IsChecked)
            {
                int account = 0;
                if (Int32.TryParse(textBoxAccount.Text, out account))
                {
                    mainWindow.setIsPlacingBet(true);
                    mainWindow.setAccount(account);
                    mainWindow.setButtons(false);                 
                    e.Cancel = closeWindow();
                }
                else
                {
                    MessageBox.Show("You must enter a valid full dollar amount!", "Invalid Value", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                }
            }else
            {
                mainWindow.setIsPlacingBet(false);
                mainWindow.setButtons(true);
                e.Cancel = closeWindow();             
            }

        }

        private bool closeWindow()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure about your options?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                mainWindow.enable(true);
                return false;
            }
            else
            {
                mainWindow.enable(false);
                return true;
            }
            
        }

        private void checkBoxBets_Click(object sender, RoutedEventArgs e)
        {                     
            if((bool)checkBoxBets.IsChecked)
            {
                textBoxAccount.IsEnabled = true;
            }else
            {
                textBoxAccount.IsEnabled = false;
            }
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
