using BizTier;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

namespace Client
{
    //Data Handler class of the Client System
    public class ClientDataController
    {
        //Singleton object of this class
        private static ClientDataController dataController = null;

        //Save the Business Tier Server connection status
        private string serverStatus;

        //Save the Connection result with the Business Tier Server
        bool serverConnectionResult;

        //Business Tier object
        IBankBiz bank;

        //User Windows of the Current Session
        private UserManagementWindow userManagement = null;
        private AccountManagementWindow accountManagement = null;
        private TransactionsManagementWindow transactionsManagement = null;

        //Current User data
        private bool isUserSelected = false;
        private bool isAccountSelected = false;
        private uint userId;
        private uint accountId;
        private string userFname;

        private ClientDataController()
        {
            //Connect with the server
            serverConnectionResult = ConnectServer(out serverStatus);
        }

        /*
         * Function - Connect with the Business Tier Server
         * Parameters - status - to get the status of the Server connection
         * Return type - Bool value, indicating success or faults
         */
        private bool ConnectServer(out string status)
        {
            try
            {
                ChannelFactory<IBankBiz> channelFactory;
                NetTcpBinding tcpBinding = new NetTcpBinding();
                string url = "net.tcp://localhost:50002/BankClient";
                channelFactory = new ChannelFactory<IBankBiz>(tcpBinding, url);
                bank = channelFactory.CreateChannel();
                List<uint> users = new List<uint>();
                uint checkUser = bank.GetUser();
                status = "success";
                return true;
            }
            catch (Exception e)
            {
                status = "Cannot connect with the Server!\nError : " + e.Message;
                return false;
            }
        }

        /*
         * Function - Process all transactions and save data
         * Parameters - None
         * Return type - Bool value, indicating success or faults
         */
        public bool CommitData()
        {
            try
            {
                return bank.CommitData();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /*
         * Function - Get the status of the Business Tier Server connection
         * Parameters - None
         * Return type - Bool value, indicating success or faults
         */
        public void GetServerConnectionStatus(out bool isConnectionSuccessful, out string serverStatusMessage)
        {
            isConnectionSuccessful = serverConnectionResult;
            serverStatusMessage = serverStatus;
        }

        /*
         * Function - Get the Singlton object of this class
         * Parameters - None
         * Return type - Singleton object of ClientDataController class
         */
        public static ClientDataController GetClientDataController()
        {
            if (dataController == null)
                dataController = new ClientDataController();

            return dataController;
        }

        /*
         * Function - Get object of the UserManagementWindow class
         * Parameters - object of the startup MainWindow class
         * Return type - Object of the UserManagementWindow class
         */
        public UserManagementWindow GetUserManagement(Window mainWindow)
        {
            if (userManagement == null)
                userManagement = new UserManagementWindow(mainWindow);

            return userManagement;
        }

        /*
         * Function - Get object of the AccountManagementWindow class
         * Parameters - object of the startup MainWindow class
         * Return type - Object of the AccountManagementWindow class
         */
        public AccountManagementWindow GetAccountManagement(Window mainWindow)
        {
            if (accountManagement == null)
                accountManagement = new AccountManagementWindow(mainWindow);

            return accountManagement;
        }

        /*
         * Function - Get object of the TransactionsManagementWindow class
         * Parameters - object of the startup MainWindow class
         * Return type - Object of the TransactionsManagementWindow class
         */
        public TransactionsManagementWindow GetTransactionManagement(Window mainWindow)
        {
            if (transactionsManagement == null)
                transactionsManagement = new TransactionsManagementWindow(mainWindow);

            return transactionsManagement;
        }

        /*
         * Function - Login function of the system
         * Parameters - fname - to pass the first name of the user
         *              id - to pass the user id of the user
         *              message - to get the message of the login function status
         * Return type - Bool value, indicating success or faults
         */
        public bool Login(string fname, uint id, out string message)
        {
            try
            {
                return bank.Login(fname, id, out message);
            }
            catch (Exception)
            {
                message = "";
                return false;
            }
        }

        /*
         * Function - Search Transaction method
         * Parameters - fname - to pass the first name of the user
         *              id - to pass the user id of the user
         *              message - to get the message of the login function status
         * Return type - Bool value, indicating success or faults
         */
        public string SearchTransactions(uint searchQuery, out List<uint> transactions)
        {
            try
            {
                bool result = bank.SearchTransactions(searchQuery, out transactions, out string status);
                return status;
            }
            catch (Exception)
            {
                transactions = null;
                return "server method execution failed";
            }
        }

        /*
         * Function - Set/save the current user id
         * Parameters - userID - to pass the id of the current user
         * Return type - string, containing the status of the function
         */
        public string SetUserId(uint userId)
        {
            try
            {
                if (userId == 0)
                {
                    this.userId = userId;
                    bank.SetUser(userId);
                    this.isUserSelected = false;
                    return "success";
                }

                this.userId = userId;
                bank.SetUser(userId);
                this.isUserSelected = true;
                return "success";
            }
            catch (Exception e)
            {
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
         * Function - Get current user id
         * Parameters - user - get the user id
         * Return type - string, containing the status of the function
         */
        public string GetUserId(out uint user)
        {
            try
            {
                this.userId = bank.GetUser();
                user = this.userId;
                return "success";
            }
            catch (Exception e)
            {
                user = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
         * Function - Set the first name of the current user
         * Parameters - fname - to pass the first name of the user
         * Return type - None
         */
        public void SetUserFname(string fname)
        {
            this.userFname = fname;
        }

        /*
         * Function - Get the first name of the current user
         * Parameters - None
         * Return type - string, to get the first name of the user
         */
        public string GetUserFname()
        {
            return userFname;
        }

        /*
         * Function - Get a Bank Account id and validate and if success save the Bank Account id
         * Parameters - accountId - pass the Bank Account id
         * Return type - string, containing the status of the function
         */
        public string SetAccountId(uint accountId)
        {
            try
            {
                if (accountId == 0)
                {
                    this.isAccountSelected = false;
                    return "success";
                }

                List<uint> userAccounts = new List<uint>();
                bool result = bank.GetAccountIdsByUser(out userAccounts);
                if (result)
                {
                    foreach (uint account in userAccounts)
                    {
                        if (accountId == account)
                        {
                            this.accountId = accountId;
                            this.isAccountSelected = true;
                            return "success";
                        }
                    }

                    return "invalid account";
                }
                else
                {
                    return "server method execution failed";
                }

            }
            catch (Exception e)
            {
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
         * Function - Find whether a user is logged or not
         * Parameters - None
         * Return type - bool value, containing true or false
         */
        public bool IsUserSelected()
        {
            return isUserSelected;
        }

        /*
         *Function - Find whether a Bank Account is selected or not
         * Parameters - None
         * Return type - bool value, containing true or false
         */
        public bool IsAccountSelected()
        {
            return isAccountSelected;
        }

        /*
         *Function - Get the saved Bank Account Id
         * Parameters - None
         * Return type - Bank Account id
         */
        public uint GetAccountId()
        {
            return accountId;
        }

        /*
         *Function - Creating a Bank Account for the current user
         *              accountID - to return/get the newly created Bank Account id
         * Return type - string, containing the status of the function
         */
        public string CreateAccount(out uint newAccountId)
        {
            try
            {
                bool result = bank.CreateAccount(out newAccountId);
                if (result)
                    return "success";
                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                newAccountId = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        *Function - Creating a new User Account in the system
        * Parameters - fname - to pass the first name of the user
        *              lname - to pass the last name of the user
        *              userID - to get the newly created User Account's id
        * Return type - string, containing the status of the function
        */
        public string CreateUser(string fname, string lname, out uint newUserId)
        {
            try
            {
                bool result = bank.CreateUser(fname, lname, out newUserId);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                newUserId = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        *Function - Deposit a given amount in the selected Bank Account
        *              amount - pass the amount that need to deposited in the Bank Account
        *              balance - to get the new balance of the given Bank Account
        * Return type - string, containing the status of the function
        */
        public string Deposit(uint amount, out uint balance)
        {
            try
            {
                bool result = bank.Deposit(accountId, amount, out balance);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                balance = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        * Function - Get all Bank Account Ids of the current user
        * Parameters - accounts - to get a list containing user's all bank account ids.
        * Return type - string, containing the status of the function
        */
        public string GetAccountIdsByUser(out List<uint> accounts)
        {
            try
            {
                bool result = bank.GetAccountIdsByUser(out accounts);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                accounts = null;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        * Function - Get balance of the current Bank Account
        * Parameters - balance - to get the balance of the Bank Account
        * Return type - string, containing the status of the function
        */
        public string GetBalance(out uint balance)
        {
            try
            {
                bool result = bank.GetBalance(accountId, out balance);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                balance = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        *Function - Get the owners id of a given Bank Account
        * Parameters - searchAccountId - to pass the account id of the Bank Account, to get the user's Id
        *              accountUserId - to get the user id of the given Bank Account
        * Return type - string, containing the status of the function
        */
        public string GetAccountOwner(uint searchAccountId, out uint accountUserId)
        {
            try
            {
                bool result = bank.GetOwner(searchAccountId, out accountUserId);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                accountUserId = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        * Function - Get Transaction data of a given transaction
        * Parameters - transactionID - to pass the transaction id
        *              senderAccount - to get the Bank Account id, which was used to transfer money
        *              receiverAccount - to get the Bank Account id, which received the transaction money
        *              amount - to get the amount of the transaction
        * Return type - string, containing the status of the function
        */
        public string GetTransactionData(uint transactionID, out uint senderAccount, out uint receiverAccount, out uint amount)
        {
            try
            {
                string result = bank.GetTransactionData(transactionID, out senderAccount, out receiverAccount, out amount);
                if (result.Equals("success"))
                    return "success";
                else if (result.Equals("invalid transaction"))
                {
                    senderAccount = 0;
                    receiverAccount = 0;
                    amount = 0;
                    return "Invalid Transaction";
                }
                else
                {
                    senderAccount = 0;
                    receiverAccount = 0;
                    amount = 0;
                    return result;
                }
            }
            catch (Exception e)
            {
                senderAccount = 0;
                receiverAccount = 0;
                amount = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        * Function - Get Transactions of the current user
        * Parameters - transactions - a list containing all transactions of the current user
        * Return type - string, containing the status of the function
        */
        public string GetUserTransactions(out List<uint> transactions)
        {
            try
            {
                bool result = bank.GetUserTransactions(out transactions);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                transactions = null;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        *Function - Get Transactions of the current Bank Account
        * Parameters - transactions - a list containing all transactions of the current Bank Account
        * Return type - string, containing the status of the function
        */
        public string GetAccountTransaction(out List<uint> transactions)
        {
            try
            {
                bool result = bank.GetAccountTransactions(accountId, out transactions);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                transactions = null;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
         * Function - Get first name and the last name of the current user
         * Parameters - fname - to get the first name of the current user
         *              lname - to get the last name of the current user
         * Return type - string, containing the status of the function
         */
        public string GetUserName(out string fname, out string lname)
        {
            try
            {
                bool result = bank.GetUserName(out fname, out lname);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                fname = "";
                lname = "";
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        * Function - Create a new transaction
        * Parameters - transactionID - to get the newly created transaction id
        *              senderAccount - to pass the sender Bank Account id of the transaction
        *              receiverAccount - to pass the account id of the receiver Bank Account
        *              amount - to pass the amount of the transaction
        * Return type - string, containing the status of the function
        */
        public string NewTransaction(out uint transactionID, uint senderAccount, uint receiverAccount, uint amount)
        {
            try
            {
                string status;
                bool result = bank.NewTransaction(out transactionID, senderAccount, receiverAccount, amount, out status);
                if (result)
                    return status;

                else
                {
                    return "server method execution failed";
                }
            }
            catch (Exception e)
            {
                transactionID = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
        *Function - Set first name and last name of the current user
        * Parameters - fname - to pass the first name of the user
        *              lname - to pass the last name of the user
        * Return type - string, containing the status of the function
        */
        public string SetUserName(string fname, string lname)
        {
            try
            {
                bool result = bank.SetUserName(fname, lname);
                if (result)
                    return "success";

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }

        /*
       * Function - Withdrawal function of the currently selected Bank Account (Get a given amount  from the current Bank Aaccount)
       * Parameters - accountID - to pass the id of the Bank Account, to withdraw money
       *              amount - to pass the amount, that need to be withdrawn from the Bank Account
       *              balance - to get the balance of the Bank Account, after performing the Withdraw function
       * Return type - string, containing the status of the function
       */
        public string Withdraw(uint accountID, uint amount, out uint balance)
        {
            try
            {
                string status;
                bool result = bank.Withdraw(accountID, amount, out balance, out status);
                if (result)
                    return status;

                else
                    return "server method execution failed";
            }
            catch (Exception e)
            {
                balance = 0;
                return "Cannot connect with the server!\nError : " + e.Message;
            }
        }
    }
}
