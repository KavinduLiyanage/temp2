using DataTier;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace BizTier
{
    //Defining the Service Behavior with default Per Client pattern
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    internal class IBankBizImpl : IBankBiz
    {
        //Data tier object
        IBankData dataTier;

        //To store the user id
        private static uint user = 0;

        //To keep the track of the Transaction amount of a particular Bank Account
        //Key of the Dictionary object - Bank Account Id
        //Value of the Dictionary object - Total Transaction Amount
        Dictionary<uint, uint> transactionAmount = new Dictionary<uint, uint>();

        IBankBizImpl()
        {
            try
            {
                //Connect with the Server in the Data Tier
                ChannelFactory<IBankData> channelFactory;
                NetTcpBinding tcpBinding = new NetTcpBinding();
                string url = "net.tcp://localhost:50001/Bank";
                channelFactory = new ChannelFactory<IBankData>(tcpBinding, url);
                dataTier = channelFactory.CreateChannel();
                Console.WriteLine("BizServer connected to the server!");
            }
            catch (Exception e)
            {
                Console.WriteLine("BizServer cannot connect with the data server!\nError : " + e.Message);
            }
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
                List<uint> loginUsers = new List<uint>();
                bool result = dataTier.GetUsers(out loginUsers);
                if (result)
                {
                    if (loginUsers.Count == 0)
                    {
                        message = "No user accounts in the System! Please create a User Account first!";
                        return true;
                    }
                    else
                    {
                        foreach (uint loginUser in loginUsers)
                        {
                            if (id == loginUser)
                            {
                                string firstName, lastName;
                                result = dataTier.GetUserName(loginUser, out firstName, out lastName);
                                if (result)
                                {
                                    if (fname.Equals(firstName))
                                    {
                                        message = "Login Success";
                                        return true;
                                    }
                                    else
                                    {
                                        message = "Invalid User Name!";
                                        return true;
                                    }
                                }
                                else
                                {
                                    message = "Cannot execute Server method! Please check your connection";
                                    return true;
                                }
                            }
                        }

                        message = "Invalid login id!";
                        return true;
                    }
                }
                else
                {
                    message = "Cannot execute Server method! Please check your connection";
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute Login() function\nError : " + e.Message);
                message = "Cannot execute Login() function\nError : " + e.Message;
                return false;
            }
        }

        /*
         * Function - Search a transaction/transactions for a given query
         * Parameters - searchQuery - transaction id or sender or receiver Bank Account id or amount to search
         *              transactions - a list containing transactions as search result
         *              searchResultStatus - status of the search function, whether its success or not
         * Return type - Bool value, indicating success or faults
         */
        public bool SearchTransactions(uint searchQuery, out List<uint> transactions, out string searchResultStatus)
        {
            try
            {
                List<uint> allTransactions = new List<uint>();
                bool result = dataTier.GetTransactions(out allTransactions);
                if (result)
                {
                    if (allTransactions.Count > 0)
                    {
                        List<uint> searchResult1 = new List<uint>();
                        uint sender, receiver, amount;
                        foreach (uint transaction in allTransactions)
                        {
                            result = dataTier.GetTransactionData(transaction, out sender, out receiver, out amount);
                            if (searchQuery == sender || searchQuery == receiver || searchQuery == amount || searchQuery == transaction)
                            {
                                searchResult1.Add(transaction);
                            }
                        }

                        if (searchResult1.Count > 0)
                        {
                            List<uint> searchResult2 = new List<uint>();
                            List<uint> userTransactions = new List<uint>();
                            result = this.GetUserTransactions(out userTransactions);
                            if (userTransactions.Count > 0)
                            {
                                foreach (uint userTrans in userTransactions)
                                {
                                    foreach (uint searchCheck in searchResult1)
                                    {
                                        if (searchCheck == userTrans)
                                        {
                                            searchResult2.Add(searchCheck);
                                        }
                                    }
                                }

                                transactions = searchResult2;
                                searchResultStatus = "success";
                                return true;
                            }
                            else
                            {
                                //The user is going to view another user's transaction data, so hide the search result
                                transactions = null;
                                searchResultStatus = "Invalid user input!";
                                return true;
                            }
                        }

                        else
                        {
                            transactions = null;
                            searchResultStatus = "No search result for your query!";
                            return true;
                        }
                    }
                    else
                    {
                        transactions = null;
                        searchResultStatus = "There are no Pending Transactions in the System";
                        return true;
                    }
                }
                else
                {
                    transactions = null;
                    searchResultStatus = "Cannot execute SearchTransactions() function";
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute SearchTransactions() function\nError : " + e.Message);
                transactions = null;
                searchResultStatus = "Cannot execute SearchTransactions() function\nError : " + e.Message;
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
                return dataTier.CommitData();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute CommitData() function\nError : " + e.Message);
                return false;
            }
        }

        /*
         * Function - Creating a Bank Account for the current user
         * Parameters - accountID - to return/get the newly created Bank Account id
         * Return type - Bool value, indicating success or faults
         */
        public bool CreateAccount(out uint accountID)
        {
            try
            {
                return dataTier.CreateAccount(user, out accountID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute CreateAccount() function\nError : " + e.Message);
                accountID = 0;
                return false;
            }
        }

        /*
        * Function - Creating a new User Account in the system
        * Parameters - fname - to pass the first name of the user
        *              lname - to pass the last name of the user
        *              userID - to get the newly created User Account's id
        * Return type - Bool value, indicating success or faults
        */
        public bool CreateUser(string fname, string lname, out uint userID)
        {
            try
            {
                return dataTier.CreateUser(fname, lname, out userID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute CreateUser() function\nError : " + e.Message);
                userID = 0;
                return false;
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
            try
            {
                return dataTier.Deposit(accountID, amount, out balance);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute Deposit() function\nError : " + e.Message);
                balance = 0;
                return false;
            }
        }

        /*
        * Function - Get all Bank Account Ids of a given user
        * Parameters - accounts - to get a list containing user's all bank account ids.
        * Return type - Bool value, indicating success or faults
        */
        public bool GetAccountIdsByUser(out List<uint> accounts)
        {
            try
            {
                return dataTier.GetAccountIdsByUser(user, out accounts);
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
                return dataTier.GetBalance(accountID, out balance);
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
        *              userID - to get the user id of the given Bank Account
        * Return type - Bool value, indicating success or faults
        */
        public bool GetOwner(uint accountID, out uint userID)
        {
            try
            {
                return dataTier.GetOwner(accountID, out userID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetOwner() function\nError : " + e.Message);
                userID = 0;
                return false;
            }
        }

        /*
        *Function - Get Transaction data of a given transaction of the current user
        * Parameters - transactionID - to pass the transaction id
        *              senderAccount - to get the Bank Account id, which was used to transfer money
        *              receiverAccount - to get the Bank Account id, which received the transaction money
        *              amount - to get the amount of the transaction
        * Return type - Bool value, indicating success or faults
        */
        public string GetTransactionData(uint transactionID, out uint senderAccount, out uint receiverAccount, out uint amount)
        {
            try
            {
                bool result = dataTier.GetTransactionData(transactionID, out senderAccount, out receiverAccount, out amount);
                if (result)
                    return "success";
                else
                    return "GetTransactionData() method in the database has been failed to execute!";
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetTransactionData() function\nError : " + e.Message);
                senderAccount = 0;
                receiverAccount = 0;
                amount = 0;
                return "Cannot execute GetTransactionData() function\nError : " + e.Message;
            }
        }

        /*
        * Function - Get Transactions of the current user
        * Parameters - transactionsParam - a list containing all transactions of the current user
        * Return type - Bool value, indicating success or faults
        */
        public bool GetUserTransactions(out List<uint> transactionsParam)
        {
            try
            {
                List<uint> userAccounts = new List<uint>();
                bool result = this.GetAccountIdsByUser(out userAccounts);
                if (!result)
                {
                    transactionsParam = null;
                    return false;
                }

                List<uint> trans = new List<uint>();
                foreach (uint account in userAccounts.ToArray())
                {
                    List<uint> accountTransactions = new List<uint>();
                    result = this.GetAccountTransactions(account, out accountTransactions);
                    if (result)
                    {
                        if (accountTransactions.Count > 0)
                        {
                            foreach (uint transaction in accountTransactions)
                            {
                                trans.Add(transaction);
                            }
                        }
                    }
                }
                transactionsParam = trans;
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetUserTransactions() function\nError : " + e.Message);
                transactionsParam = null;
                return false;
            }
        }

        /*
        * Function - Get Transactions of a particular account
        * Parameters - accountID - to pass the Bank Account Id to find the all transactions of that Bank Account
        *               transactions - a list containing all transactions of the given Bank Account
        * Return type - Bool value, indicating success or faults
        */
        public bool GetAccountTransactions(uint accountID, out List<uint> transactions)
        {
            try
            {
                List<uint> allTransactions = new List<uint>();
                bool result = dataTier.GetTransactions(out allTransactions);
                if (result)
                {
                    uint sender, receiver, amt;
                    List<uint> trans = new List<uint>();
                    foreach (uint transaction in allTransactions)
                    {
                        result = dataTier.GetTransactionData(transaction, out sender, out receiver, out amt);
                        if (!result)
                        {
                            transactions = null;
                            return false;
                        }

                        if (accountID == sender || accountID == receiver)
                            trans.Add(transaction);
                    }
                    transactions = trans;
                    return true;
                }
                else
                {
                    transactions = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetAccountTransactions() function\nError : " + e.Message);
                transactions = null;
                return false;
            }
        }

        /*
         * Function - Get current user id
         * Parameters - None
         * Return type - uint value, containing current user's Id
         */
        public uint GetUser()
        {
            return user;
        }

        /*
         * Function - Get first name and the last name of the current user
         * Parameters - fname - to get the first name of the current user
         *              lname - to get the last name of the current user
         * Return type - Bool value, indicating success or faults
         */
        public bool GetUserName(out string fname, out string lname)
        {
            try
            {
                return dataTier.GetUserName(user, out fname, out lname);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute GetUserName() function\nError : " + e.Message);
                fname = "";
                lname = "";
                return false;
            }
        }

        /*
        * Function - Create a new transaction
        * Parameters - transactionID - to get the newly created transaction id
        *              senderAccount - to pass the sender Bank Account id of the transaction
        *              receiverAccount - to pass the account id of the receiver Bank Account
        *              amount - to pass the amount of the transaction
        *              status - to get the status of the new transaction (whether it succeeded or not)
        * Return type - Bool value, indicating success or faults
        */
        public bool NewTransaction(out uint transactionID, uint senderAccount, uint receiverAccount, uint amount, out string status)
        {
            try
            {
                //Receiver account validation
                if (!this.IsAccountAvailable(receiverAccount, out string message))
                {
                    status = message;
                    transactionID = 0;
                    return true;
                }

                //Receiver account validation
                else if (!message.Equals("success"))
                {
                    status = message;
                    transactionID = 0;
                    return true;
                }

                uint balance;
                bool result = dataTier.GetBalance(senderAccount, out balance);
                if (result)
                {
                    //Account balance validation
                    if (amount >= balance)
                    {
                        status = "Not enough amount in your account to transfer!";
                        transactionID = 0;
                        return true;
                    }

                    else if (transactionAmount.ContainsKey(senderAccount))
                    {
                        if (amount >= (balance - transactionAmount[senderAccount]))
                        {
                            status = "Not enough amount in your account to transfer!";
                            transactionID = 0;
                            return true;
                        }

                        else
                        {
                            status = "success";
                            transactionAmount[senderAccount] = transactionAmount[senderAccount] + amount;
                            return dataTier.NewTransaction(out transactionID, senderAccount, receiverAccount, amount);
                        }
                    }

                    else
                    {
                        transactionAmount.Add(senderAccount, amount);
                        status = "success";
                        return dataTier.NewTransaction(out transactionID, senderAccount, receiverAccount, amount);
                    }
                }
                else
                {
                    status = "GetBalance method failed";
                    transactionID = 0;
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute NewTransaction() function\nError : " + e.Message);
                transactionID = 0;
                status = "Cannot connect with the server\nCannot execute NewTransaction() function\nError : " + e.Message;
                return false;
            }
        }

        /*
        * Function - Check whether a given account is exists in the System
        * Parameters - searchAccount - to pass the account id, that we ned to search
        *              message - to get the status of the account (whther it exists or not)
        * Return type - Bool value, indicating success or faults
        */
        private bool IsAccountAvailable(uint searchAccount, out string message)
        {
            try
            {
                List<uint> users = new List<uint>();
                List<uint> accounts = new List<uint>();
                bool result = dataTier.GetUsers(out users);
                if (result)
                {
                    foreach (uint user in users)
                    {
                        result = dataTier.GetAccountIdsByUser(user, out accounts);
                        if (result)
                        {
                            foreach (uint account in accounts)
                            {
                                if (searchAccount == account)
                                {
                                    message = "success";
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            message = "Cannot execute method";
                            Console.WriteLine("Cannot execute GetAccountIdsByUser() function in the IsAccountAvailable() function");
                            return true;
                        }
                    }
                    message = "Invalid Transfer Account Number!";
                    return true;
                }
                else
                {
                    message = "Cannot execute method";
                    Console.WriteLine("Cannot execute GetUsers() function in the IsAccountAvailable() function");
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute NewTransaction() function\nError : " + e.Message);
                message = "Cannot execute NewTransaction() function\nError : " + e.Message;
                return false;
            }
        }

        /*
         * Function - Set/save the current user id
         * Parameters - userID - to pass the id of the current user
         * Return type - None
         */
        public void SetUser(uint userID)
        {
            user = userID;
        }

        /*
        * Function - Set first name and last name of a given user
        * Parameters - fname - to pass the first name of the user
        *              lname - to pass the last name of the user
        * Return type - Bool value, indicating success or faults
        */
        public bool SetUserName(string fname, string lname)
        {
            try
            {
                return dataTier.SetUserName(user, fname, lname);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute SetUserName() function\nError : " + e.Message);
                return false;
            }
        }

        /*
       * Function - Withdrawal function of a given Bank Account (Get an amount of a given account)
       * Parameters - accountID - to pass the id of the Bank Account, to withdraw money
       *              amount - to pass the amount, that need to be withdrawn from the Bank Account
       *              balance - to get the balance of the Bank Account, after performing the Withdraw function
       * Return type - Bool value, indicating success or faults
       */
        public bool Withdraw(uint accountID, uint amount, out uint balance, out string status)
        {
            try
            {
                bool result = dataTier.GetBalance(accountID, out balance);
                if (result)
                {
                    if (amount >= balance)
                    {
                        status = "Not enough amount in your account to withdraw";
                        return true;
                    }

                    status = "success";
                    return dataTier.Withdraw(accountID, amount, out balance);
                }
                else
                {
                    status = "GetBalance method failed";
                    balance = 0;
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot execute Withdraw() function\nError : " + e.Message);
                balance = 0;
                status = "Cannot connect with the server!\nCannot execute Withdraw() function\nError : " + e.Message;
                return false;
            }
        }
    }
}
