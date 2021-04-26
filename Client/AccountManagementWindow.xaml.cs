using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for AccountManagementWindow.xaml
    /// </summary>
    public partial class AccountManagementWindow : Window
    {
        ClientDataController dataController;    //Object of the Data Handler class
        MainWindow mainWindow;  //Reference for the MainWindow class
        string requestResult;   //Stores the status of a function after it's executed
        uint account, balance = 0;
        string dollars = "";

        private static readonly Regex checkAmountsRegex = new Regex("[^0-9]+"); //Regex expression which used to prevent users from entering letters inside the amount Text boxes

        public AccountManagementWindow(Window mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow as MainWindow; //Initialize MainWindow reference object

            dataController = ClientDataController.GetClientDataController();    //Initialize Client System Data Handler class object
        }

        //Redirects users to the MainWindow interface if they click on the "Go To Main Window" button
        private void goToMainWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mainWindow.Activate();
        }

        //Activate the CreateAccount function in the system, when the user clicks on the Create Account Button
        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            //Gets a user confirmation
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Are you agreeing with the Bank's Terms and Conditions?", "Creating a Bank Account", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                MessageBox.Show("You cancelled the Bank Account creation process!", "Alert!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            uint accountId;
            requestResult = dataController.CreateAccount(out accountId);
            if (requestResult.Equals("success"))
            {
                displayUserAccountsComboBox.SelectedIndex = 0;
                MessageBox.Show("A new account has been created!\nAccount ID : " + accountId, "Success!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                DisplayAccounts();  //Loads and displays current users all Bank Accounts in the System
            }
            else
            {
                MessageBox.Show(requestResult, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //This function will activate when the user completed the login process
        //Load Bank Account data according to the current user
        public void LoadAccounts()
        {
            displayUserAccountsComboBox.Items.Clear();
            accountManagementUserGreetingLabel.Content = "!Howdy " + dataController.GetUserFname();
            DisplayAccounts();  //Loads and displays current users all Bank Accounts in the System
        }

        //Loads and displays current users all Bank Accounts in the System
        private void DisplayAccounts()
        {
            List<uint> accounts = new List<uint>();
            requestResult = dataController.GetAccountIdsByUser(out accounts);
            if (requestResult.Equals("success"))
            {
                if (accounts.Count != 0)
                {
                    displayUserAccountsComboBox.Items.Clear();
                    displayUserAccountsComboBox.Items.Insert(0, "Please select an account");
                    displayUserAccountsComboBox.SelectedIndex = 0;

                    int counter = 1;
                    foreach (uint account in accounts)
                    {
                        displayUserAccountsComboBox.Items.Insert(counter, account);
                        counter++;
                    }
                }
            }
            else
            {
                MessageBox.Show(requestResult, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Letters and Number validation function, by using the above mentioned Regex
        //Letters are not allowed
        //Only numbers are allowed
        private static bool ISTextAllowed(string text)
        {
            return !checkAmountsRegex.IsMatch(text);
        }

        //Validate the user input when he or she enters data in the Text Box assined to get amounts for withdrawal and deposit functions
        private void IsValidInputDollar(object sender, KeyEventArgs e)
        {
            string value = depositAmountDollar.Text;
            if (!ISTextAllowed(value))
            {
                depositAmountDollar.Text = dollars;
            }
            else
            {
                dollars = value;
            }
        }

        //This function will activate when user clicks on the Withdraw button
        private void withdrawButton_Click(object sender, RoutedEventArgs e)
        {
            string amount = withdrawAmountDollar.Text;
            if (amount.Length == 0)
            {
                MessageBox.Show("Please enter an amount to Withdraw!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (displayUserAccountsComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a Bank Account to withdraw!", "Error!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            uint withdrawAmount = UInt32.Parse(amount);
            requestResult = dataController.Withdraw(account, withdrawAmount, out balance);
            if (requestResult.Equals("success"))    //If the Withdraw is success
            {
                MessageBox.Show("Successfully Withdrawed $" + amount + " from your account", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                displayAccountBalanceLabel.Content = "Account Balance: $" + balance;
                withdrawAmountDollar.Clear();
            }
            else
            {
                MessageBox.Show(requestResult, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //This function will be executed when the user changes the value in the account selection Combo Box
        private void displayUserAccountsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint check;
            requestResult = dataController.GetUserId(out check);
            if (requestResult.Equals("success"))
            {
                //Check whether the user is logged or not
                if (check != 0) //User logged
                {
                    if (displayUserAccountsComboBox.SelectedIndex == 0)
                    {
                        return;
                    }

                    if (displayUserAccountsComboBox.Items.IsEmpty)
                        return;

                    account = (uint)displayUserAccountsComboBox.SelectedItem;
                    requestResult = dataController.SetAccountId(account);
                    if (requestResult.Equals("success"))
                    {
                        requestResult = dataController.GetBalance(out balance);
                        if (requestResult.Equals("success"))
                        {
                            displayAccountNumberLabel.Content = "Account Number: " + account;   //Display Bank Account id in the information section
                            displayAccountBalanceLabel.Content = "Account Balance: $" + balance;   //Display Bank Account balance in the information section
                            mainWindow.SetAccountData();
                        }
                        else
                        {
                            MessageBox.Show(requestResult, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show(requestResult, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        //Activate when the user clicks on the Close button
        private void CloseAction(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            mainWindow.Activate();
        }

        //When the user clicks the Deposit Button, this function will invoke
        private void depositButton_Click(object sender, RoutedEventArgs e)
        {
            string amount = depositAmountDollar.Text;
            if (amount.Length == 0)
            {
                MessageBox.Show("Please enter an amount to Deposit!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (displayUserAccountsComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a Bank Account to Deposit!", "Error!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            uint depositAmount = UInt32.Parse(amount);
            requestResult = dataController.Deposit(depositAmount, out balance);
            if (requestResult.Equals("success"))
            {
                MessageBox.Show("Successfully Added $" + amount + " to your account", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                displayAccountBalanceLabel.Content = "Account Balance: $" + balance;
                depositAmountDollar.Clear();
            }
            else
            {
                MessageBox.Show(requestResult, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Logout function of this Window
        public void Logout()
        {
            accountManagementUserGreetingLabel.Content = "";
            displayUserAccountsComboBox.Items.Clear();
            displayAccountBalanceLabel.Content = "";
            displayAccountNumberLabel.Content = "";
            dataController.SetAccountId(0);
        }
    }
}
