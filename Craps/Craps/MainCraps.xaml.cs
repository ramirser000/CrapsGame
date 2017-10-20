using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Craps
{
    //Command definitions.
    namespace Commands
    {
        public class CrapsCommands
        {
            public static RoutedUICommand startCommand = new RoutedUICommand("Start command", "startCommand", typeof(CrapsCommands));
            public static RoutedUICommand restartCommand = new RoutedUICommand("Restart command", "restartCommand", typeof(CrapsCommands));
            public static RoutedUICommand exitCommand = new RoutedUICommand("Exit command", "exitCommand", typeof(CrapsCommands));
            public static RoutedUICommand helpCommand = new RoutedUICommand("Help command", "helpCommand", typeof(CrapsCommands));
            public static RoutedUICommand rulesCommand = new RoutedUICommand("Rules command", "rulesCommand", typeof(CrapsCommands));
            public static RoutedUICommand shortCutsCommand = new RoutedUICommand("Shortcuts command", "shortCutsCommand", typeof(CrapsCommands));
            public static RoutedUICommand rollCommand = new RoutedUICommand("Roll command", "rollCommand", typeof(CrapsCommands));
            public static RoutedUICommand playAgainCommand = new RoutedUICommand("Play again command", "playAgainCommand", typeof(CrapsCommands));
        }
    }

    //Main Window
    public partial class WindowCraps : Window
    {
        private bool inGame;
        private bool inPoint;
        private bool placingBet;
        private bool endGame;

        private int playerWinTotal;
        private int houseWinTotal;
        private int point;
        private int accountBalance;
        private int bet;


        public WindowCraps()
        {
            InitializeComponent();
        }

        //Command function implementations.
        private void start()
        {
            if (!inGame)
            {
                labelAccount.Visibility = Visibility.Hidden;
                textBoxAccount.Visibility = Visibility.Hidden;

                labelBet.Visibility = Visibility.Hidden;
                textBoxBet.Visibility = Visibility.Hidden;
                placingBet = false;

                askUserIfUsingAccount();
                inGame = true;
                inPoint = false;
                playerWinTotal = 0;
                houseWinTotal = 0;

                labelPhase.Foreground = new SolidColorBrush(Colors.Green);
                labelPhase.Content = "Start Phase";
                labelPhase.Visibility = Visibility.Visible;

                resetValues(true);

                buttonRollDice.IsEnabled = true;
                menuItemStart.IsEnabled = false;
            }

        }
        private void restart()
        {
            inGame = false;
            start();
        }
        private void exit()
        {
            Close();
        }
        private void help()
        {
            int arch = (IntPtr.Size == 8) ? 64 : 32;
            string netVersion = Environment.Version.ToString();

            string str = "Name: Sergio Ramirez\n" +
                         "Version: Craps 1.0 - " + arch + " bit\n" +
                         ".Net version: " + netVersion + "\n";

            MessageBox.Show(str, "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void rules()
        {
            string str = "\u2022 A player rolls two dice where each die has six faces in the usual way(values 1 through 6).\n\n" +
                         "\u2022 After the dice have come to rest the sum of the two upward faces is calculated.\n\n" +
                         "\u2022 The first roll\n" +
                         "- If the sum is 7 or 11 on the first throw the roller / player wins.\n" +
                         "- If the sum is 2, 3 or 12 the roller/ player loses, that is the house wins.\n" +
                         "- If the sum is 4, 5, 6, 8, 9, or 10, that sum becomes the roller / player's \"point\"\n\n" +
                         "\u2022 Continue given the player's point\n\n" +
                         "- Now the player must roll the \"point\" total before rolling a 7 in order to win.\n" +
                         "- If they roll a 7 before rolling the point value they got on the first roll the roller / player loses(the 'house' wins). ";

            MessageBox.Show(str, "Rules", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void shortCuts()
        {
            string str = "Start game \t Ctrl+S\n" +
                         "Restart game \t Ctrl+R\n" +
                         "Exit game \t\t Ctrl+E\n" +
                         "About \t\t Ctrl+H\n" +
                         "Game rules \t Ctrl+L\n" +
                         "Shortcuts \t\t Ctrl+K\n" +
                         "Roll dice \t\t Ctrl+D\n" +
                         "Play again \t Ctrl+A";

            MessageBox.Show(str, "Keyboard Shortcuts", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        private void roll()
        {
            bool badBet = false;
            if (placingBet)
            {
                if (Int32.TryParse(textBoxBet.Text, out bet))
                {
                    if (bet <= 0 || bet > accountBalance)
                    {
                        invalidBet();
                        badBet = true;
                    }
                }
                else
                {
                    invalidBet();
                    badBet = true;
                }

            }

            if(placingBet && !badBet || !placingBet)
            {
                labelWinnerLooser.Visibility = Visibility.Hidden;
                buttonPlayAgain.IsEnabled = false;

                Random randum = new Random();
                int dieOne = randum.Next(1, 7);
                int dieTwo = randum.Next(1, 7);
                int dieTotal = dieOne + dieTwo;

                textBoxDieOne.Text = dieOne.ToString();
                textBoxDieTwo.Text = dieTwo.ToString();
                textBoxDieTotal.Text = dieTotal.ToString();

                if (inPoint)
                {
                    if (dieTotal == 7)
                    {
                        incrementHouseScore();
                        checkBalance('d');

                    }
                    else if (dieTotal == point)
                    {
                        incrementPlayerScore();
                        checkBalance('i');
                    }

                }
                else
                {
                    if (dieTotal == 7 || dieTotal == 11)
                    {
                        incrementPlayerScore();
                        checkBalance('i');

                    }
                    else if (dieTotal == 2 || dieTotal == 3 || dieTotal == 12)
                    {
                        incrementHouseScore();
                        checkBalance('d');

                    }
                    else
                    {
                        labelPhase.Foreground = new SolidColorBrush(Colors.Orange);
                        labelPhase.Content = "Point Phase";
                        textBoxPoint.Text = dieTotal.ToString();
                        point = dieTotal;
                        inPoint = true;
                    }

                }

                buttonRollDice.IsEnabled = (placingBet && !inPoint) ? false : inPoint;
            }
            


        }
        private void playAgain()
        {
            resetValues(false);

        }

        //Helper methods.
        private void resetValues(bool resetScore)
        {
            buttonRollDice.IsEnabled = !placingBet;
            textBoxDieOne.Text = "";
            textBoxDieTwo.Text = "";
            textBoxDieTotal.Text = "";
            textBoxPoint.Text = "";
            textBoxBet.Text = "";
            labelWinnerLooser.Visibility = Visibility.Hidden;
            labelPhase.Visibility = Visibility.Visible;
            buttonPlayAgain.IsEnabled = false;


            if (resetScore)
            {
                playerWinTotal = 0;
                houseWinTotal = 0;
                textBoxPlayerWinTotal.Text = playerWinTotal.ToString();
                textBoxHouseWinTotal.Text = playerWinTotal.ToString();
            }
        }
        private void incrementPlayerScore()
        {
            playerWinTotal++;
            inPoint = false;
            labelWinnerLooser.Content = "Winner!";
            labelWinnerLooser.Visibility = Visibility.Visible;
            textBoxPlayerWinTotal.Text = playerWinTotal.ToString();
            labelPhase.Foreground = new SolidColorBrush(Colors.Green);
            labelPhase.Content = "Start Phase";
            buttonPlayAgain.IsEnabled = true;
            labelPhase.Visibility = Visibility.Hidden;
        }
        private void incrementHouseScore()
        {
            houseWinTotal++;
            inPoint = false;
            labelWinnerLooser.Content = "Looser!";
            labelWinnerLooser.Visibility = Visibility.Visible;
            textBoxHouseWinTotal.Text = houseWinTotal.ToString();
            labelPhase.Foreground = new SolidColorBrush(Colors.Green);
            labelPhase.Content = "Start Phase";
            buttonPlayAgain.IsEnabled = true;
            labelPhase.Visibility = Visibility.Hidden;
        }
        private void invalidBet()
        {
            MessageBox.Show("Invalid bet value!\n You must enter a valid full dollar bet amount " +
                        "greater than 0 and withing your account balance.", "Invalid Bet", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void askUserIfUsingAccount()
        {
            WindowAccountDialog w = new WindowAccountDialog(this);
            w.Show();

        }
        private void checkBalance(char c)
        {
            if (placingBet)
            {
                if (c == 'i')
                {
                    accountBalance += bet;
                    textBoxAccount.Text = accountBalance.ToString();

                }
                else if (c == 'd')
                {
                    accountBalance -= bet;
                    textBoxAccount.Text = accountBalance.ToString();
                }

                if (accountBalance == 0)
                {
                    MessageBoxResult result = MessageBox.Show("Game Over!\n Start a new session?", "Gave Over", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        restart();
                    }
                    else
                    {
                        endGame = true;
                        App.Current.Shutdown();
                    }
                }
            }

        }
        public void setAccount(int account)
        {
            accountBalance = account;
            textBoxAccount.Text = accountBalance.ToString();
        }
        public void setIsPlacingBet(bool betting)
        {
            if ((placingBet = betting))
            {
                labelAccount.Visibility = Visibility.Visible;
                textBoxAccount.Visibility = Visibility.Visible;

                labelBet.Visibility = Visibility.Visible;
                textBoxBet.Visibility = Visibility.Visible;
            }

        }
        public void enable(bool enable)
        {
            IsEnabled = enable;
        }
        public void setButtons(bool b)
        {
            buttonRollDice.IsEnabled = b;
        }

        
        //Event Handlers
        private void windowCraps_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!endGame)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    App.Current.Shutdown();
                }
            }
            
        }

        private void textBoxBet_KeyUp(object sender, KeyEventArgs e)
        {
            buttonRollDice.IsEnabled = true;
        }

        //Execute command functions.
        private void startCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            start();
        }
        private void restartCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            restart();
        }
        private void exitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            exit();
        }
        private void helpCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            help();
        }
        private void rulesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            rules();
        }
        private void rollCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            roll();
        }
        private void playAgainCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            playAgain();
        }
        private void shortCutsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            shortCuts();
        }
    }
    
}


