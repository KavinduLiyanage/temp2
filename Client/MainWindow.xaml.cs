using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientDataController dataController;     //Data Handler class's object
        private UserManagementWindow userManagement;    //User Management Window object
        private AccountManagementWindow accountManagement;  //Account Management Window object
        private TransactionsManagementWindow transactionsManagement;    //Transaction Management Window object
        private uint accountId; //save the selected Bank Account id
        bool isConnectionSuccessful;    //check the connection with the Business Server is sucessful
        string serverStatusMessage; //save the Business Server connection status

        public MainWindow()
        {
            InitializeComponent();

            //Get object of the data handler class
            dataController = ClientDataController.GetClientDataController();

            //check the connection with the Business Tier
            dataController.GetServerConnectionStatus(out isConnectionSuccessful, out serverStatusMessage);

            //If the connection is unsuccessful, close the application
            if (!isConnectionSuccessful)
            {
                System.Windows.MessageBox.Show(serverStatusMessage + "\nPlease restart your application!", "Cannot Connect with Server!", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Application.Current.Shutdown();
                return;
            }

            //Load Windows of the 3 interfaces (User, Account, and Transaction)
            userManagement = dataController.GetUserManagement(this) as UserManagementWindow;
            accountManagement = dataController.GetAccountManagement(this) as AccountManagementWindow;
            transactionsManagement = dataController.GetTransactionManagement(this) as TransactionsManagementWindow;
        }

        //Display the User Management Window
        private void userManagementOpen(object sender, MouseButtonEventArgs e)
        {
            userManagement.Show();
        }

        //Application closing action
        //Used to process transaction and save data in the system
        private void CloseAction(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isConnectionSuccessful)
            {
                e.Cancel = false;
            }
            else
            {
                DialogResult result = (DialogResult)System.Windows.MessageBox.Show("Are you sure want to exit?", "Application Close", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    if (dataController.CommitData())
                        e.Cancel = false;

                    else
                    {
                        System.Windows.MessageBox.Show("Cannot save your data! Application cannot close!\nPlease check Server connection or try again shortly!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                    }
                }

                else
                {
                    e.Cancel = true;
                }
            }
        }

        //Display the Account Management Window
        private void accountManagementOpen(object sender, MouseButtonEventArgs e)
        {
            if (dataController.IsUserSelected())
            {
                accountManagement.Show();
            }
            else
            {
                System.Windows.MessageBox.Show("Please Login to Access the Account Management Interface!", "No Login Session!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        //This function will activate when the login is success
        public void SetUser(string fname)
        {
            userGreetingLabel.Content = "!Howdy " + fname;  //Display the user greeting according to the current user
            loginSignupButton.Visibility = Visibility.Hidden;   //Hide the Login/Signup Button
            logoutButton.Visibility = Visibility.Visible;   //Display the Logout Button
            accountManagement.LoadAccounts();
            transactionsManagement.SetUserId();
        }

        //logout function
        public void Logout()
        {
            userGreetingLabel.Content = "Howdy user! Please login"; //Change the user greeting label
            accountLabel.Content = "!No account selected";  //Change the account id displaying label
            logoutButton.Visibility = Visibility.Hidden;    //Hide the Logout Button
            loginSignupButton.Visibility = Visibility.Visible;  //Show the Login/Signup Button
            accountManagement.Logout(); //Trigger Logout function in the Account Window
            transactionsManagement.Logout();    //Trigger Logout function in the Transaction Window
        }

        //Set account details in the Transaction Management Window and in this Window
        //This function will activate when the user selects a Bank Account
        public void SetAccountData()
        {
            accountId = dataController.GetAccountId();
            accountLabel.Content = accountId;
            transactionsManagement.SetAccountData();
        }

        //Display the Transaction Management Window
        private void TransactionManagementOpen(object sender, MouseButtonEventArgs e)
        {
            if (dataController.IsUserSelected())
            {
                if (dataController.IsAccountSelected())
                    transactionsManagement.Show();

                else
                    System.Windows.MessageBox.Show("Please Select an Bank Account to Access the Transaction Management Interface!", "No Account Selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
                System.Windows.MessageBox.Show("Please Login to Access the Transaction Management Interface!", "No Login Session!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        //Navigate to User Management Window when the user clicks on the Login/Signup button
        private void loginSignupButton_Click(object sender, RoutedEventArgs e)
        {
            userManagement.Show();
        }

        //Activate the Logout function when the user clicks on the Logout button
        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            userManagement.Logout();
            Logout();
        }
    }
}
