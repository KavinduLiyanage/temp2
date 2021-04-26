using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataTier
{
    //Defining the Service Behavior with the Singleton pattern
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    internal class IBankDataImpl : IBankData
    {
        //Creating a Singleton object of this class
        private static IBankDataImpl bankDataImpl = null;

        //Creating BankDB.dll related objects
        private static BankDB.BankDB bankDB = new BankDB.BankDB();
        private static BankDB.UserAccessInterface user = bankDB.GetUserAccess();
        private static BankDB.AccountAccessInterface account = bankDB.GetAccountInterface();
        private static BankDB.TransactionAccessInterface transaction = bankDB.GetTransactionInterface();

        //Creating an object for mutex locks (For prevent two processes access the content at the same time)
        private readonly object dataLock = new object();

        //Private Constructor because of this class's Singleton behavior
        private IBankDataImpl() { }

        /*
         * Function - Get the Singlton object of this class
         * Parameters - None
         * Return type - Singleton object of IBankDataImpl class
         */
        public static IBankDataImpl GetBankDatImpl()
        {
            if (bankDataImpl == null)
                bankDataImpl = new IBankDataImpl();

            return bankDataImpl;
        }

        /*
         * Function - Process all transactions and save data in the system
         * Parameters - None
         * Return type - Bool value, indicating success or faults
         */
        public bool CommitData()
        {
            lock (dataLock)
            {
                try
                {
                    bankDB.ProcessAllTransactions();
                    this.ProcessData();
                    Console.WriteLine("Data saved successfully!");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot execute CommitData() function\nError : " + e.Message);
                    return false;
                }
            }
        }

        /*
         * Function - Save data in the system
         * Parameters - None
         * Return type - Bool value, indicating success or faults
         */
        private bool ProcessData()
        {
            try
            {
                bankDB.SaveToDisk();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute ProcessData() function\nError : " + e.Message);
                return false;
            }
        }

        /*
         * Function - Creating a Bank Account for a particular user
         * Parameters - userID - to pass the id of the requested user (current user)
         *              accountID - to return/get the newly created Bank Account id
         * Return type - Bool value, indicating success or faults
         */
        public bool CreateAccount(uint userID, out uint accountID)
        {
            lock (dataLock)
            {
                try
                {
                    accountID = account.CreateAccount(userID);
                    this.ProcessData();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot execute CreateAccount() function\nError : " + e.Message);
                    accountID = 0;
                    return false;
                }
            }
        }

        /*
        * Function - Creating a User Account in the system
        * Parameters - fname - to pass the first name of the user
        *              lname - to pass the last name of the user
        *              userID - to get the newly created User Account's id
        * Return type - Bool value, indicating success or faults
        */
        public bool CreateUser(string fname, string lname, out uint userID)
        {
            lock (dataLock)
            {
                try
                {
                    uint id = user.CreateUser();
                    user.SelectUser(id);
                    user.SetUserName(fname, lname);
                    userID = id;
                    this.ProcessData();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot execute CreateUser() function\nError : " + e.Message);
                    userID = 0;
                    return false;
                }
            }
        }

        /*
        * Function - Deposit a given amount in the selected Bank Account
        * Parameters - accountID - pass the accountId of a Bank Account, that the amount should be deposited into
        *              amount - pass the amount that need to deposited in the Bank Account
        *              balance - to get the new balance of the given Bank Account
        * Return type - Bool value, indicating success or faults
        */
        public bool Deposit(uint accountID, uint amount, out uint balance)
        {
            lock (dataLock)
            {
                try
                {
                    account.SelectAccount(accountID);
                    account.Deposit(amount);
                    balance = account.GetBalance();
                    this.ProcessData();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot execute Deposit() function\nError : " + e.Message);
                    balance = 0;
                    return false;
                }
            }
        }

        /*
        * Function - Get all Bank Account Ids of a given user
        * Parameters - userID - to pass the userId, to get his/her all existing Bank Accounts in the system
        *              accounts - to get a list containing user's all bank account ids.
        * Return type - Bool value, indicating success or faults
        */
        public bool GetAccountIdsByUser(uint userID, out List<uint> accounts)
        {
            try
            {
                accounts = account.GetAccountIDsByUser(userID);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetAccountIdsByUser() function\nError : " + e.Message);
                accounts = null;
                return false;
            }
        }

        /*
        * Function - Get balance of a given account
        * Parameters - accountID - to pass the account id of the Bank Account, to get the balance
        *              balance - to get the balance of the Bank Account
        * Return type - Bool value, indicating success or faults
        */
        public bool GetBalance(uint accountID, out uint balance)
        {
            try
            {
                account.SelectAccount(accountID);
                balance = account.GetBalance();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetBalance() function\nError : " + e.Message);
                balance = 0;
                return false;
            }
        }

        /*
        * Function - Get the owners id of a given Bank Account
        * Parameters - accountID - to pass the account id of the Bank Account, to get the user's Id
        *              userID - to get the user id of the Bank Account
        * Return type - Bool value, indicating success or faults
        */
        public bool GetOwner(uint accountID, out uint userID)
        {
            try
            {
                account.SelectAccount(accountID);
                userID = account.GetOwner();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetOwner() function\nError : " + e.Message);
                userID = 0;
                return false;
            }
        }

        /*
        * Function - Get Transaction data of a given transaction
        * Parameters - transactionID - to pass the transaction id
        *              senderAccount - to get the Bank Account id, which was used to transfer money
        *              receiverAccount - to get the Bank Account id, which received the transaction money
        *              amount - to get the amount of the transaction
        * Return type - Bool value, indicating success or faults
        */
        public bool GetTransactionData(uint transactionID, out uint senderAccount, out uint receiverAccount, out uint amount)
        {
            try
            {
                transaction.SelectTransaction(transactionID);
                senderAccount = transaction.GetSendrAcct();
                receiverAccount = transaction.GetRecvrAcct();
                amount = transaction.GetAmount();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetTransactionData() function\nError : " + e.Message);
                senderAccount = 0;
                receiverAccount = 0;
                amount = 0;
                return false;
            }
        }

        /*
        * Function - To get all transactions of the current session
        * Parameters - transactions - a list to get the all transactions of the current session of the application, until it closed
        * Return type - Bool value, indicating success or faults
        */
        public bool GetTransactions(out List<uint> transactions)
        {
            try
            {
                transactions = transaction.GetTransactions();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetTransactions() function\nError : " + e.Message);
                transactions = null;
                return false;
            }
        }

        /*
        * Function - Get the first name and last name of a given user id
        * Parameters - userID - to pass the user id of the user, that we need to find first name and last name
        *              fname - to get the first name of the given user
        *              lname - to get the last name of the given user
        * Return type - Bool value, indicating success or faults
        */
        public bool GetUserName(uint userID, out string fname, out string lname)
        {
            try
            {
                user.SelectUser(userID);
                user.GetUserName(out fname, out lname);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetUserName() function\nError : " + e.Message);
                fname = null;
                lname = null;
                return false;
            }
        }

        /*
        * Function - Get ids of all users that are saved in the system
        * Parameters - users - a list containing all user ids
        * Return type - Bool value, indicating success or faults
        */
        public bool GetUsers(out List<uint> users)
        {
            try
            {
                users = user.GetUsers();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetUsers() function\nError : " + e.Message);
                users = null;
                return false;
            }
        }

        /*
        * Function - Create a new transaction
        * Parameters - transactionID - to get the newly created transaction id
        *              senderAccount - to pass the sender Bank Account id of the transaction
        *              receiverAccount - to pass the account id of the receiver Bank Account
        *              amount - to pass the amount of the transaction
        * Return type - Bool value, indicating success or faults
        */
        public bool NewTransaction(out uint transactionID, uint senderAccount, uint receiverAccount, uint amount)
        {
            lock (dataLock)
            {
                try
                {
                    transactionID = transaction.CreateTransaction();
                    transaction.SelectTransaction(transactionID);
                    transaction.SetSendr(senderAccount);
                    transaction.SetRecvr(receiverAccount);
                    transaction.SetAmount(amount);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot execute NewTransaction() function\nError : " + e.Message);
                    transactionID = 0;
                    return false;
                }
            }
        }

        /*
        * Function - Set first name and last name of a given user
        * Parameters - userID - to pass the id of the user, which we need to change the name
        *              fname - to pass the first name of the user
        *              lname - to pass the last name of the user
        * Return type - Bool value, indicating success or faults
        */
        public bool SetUserName(uint userID, string fname, string lname)
        {
            lock (dataLock)
            {
                try
                {
                    user.SelectUser(userID);
                    user.SetUserName(fname, lname);
                    this.ProcessData();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot execute SetUserName() function\nError : " + e.Message);
                    return false;
                }
            }
        }

        /*
        * Function - Withdrawal function of a given Bank Account (Get an amount of a given account)
        * Parameters - accountID - to pass the id of the Bank Account, to withdraw money
        *              amount - to pass the amount, that need to be withdrawn from the Bank Account
        *              balance - to get the balance of the Bank Account, after performing the Withdraw function
        * Return type - Bool value, indicating success or faults
        */
        public bool Withdraw(uint accountID, uint amount, out uint balance)
        {
            lock (dataLock)
            {
                try
                {
                    account.SelectAccount(accountID);
                    account.Withdraw(amount);
                    balance = account.GetBalance();
                    this.ProcessData();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot execute Withdraw() function\nError : " + e.Message);
                    balance = 0;
                    return false;
                }
            }
        }
    }
}
