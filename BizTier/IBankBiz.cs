using System.Collections.Generic;
using System.ServiceModel;

namespace BizTier
{
    [ServiceContract]
    public interface IBankBiz
    {
        [OperationContract]
        bool Login(string fname, uint id, out string message);

        [OperationContract]
        bool SearchTransactions(uint searchQuery, out List<uint> transactions, out string searchResultStatus);

        [OperationContract]
        bool CommitData();

        [OperationContract]
        void SetUser(uint userID);

        [OperationContract]
        uint GetUser();

        [OperationContract]
        bool GetAccountIdsByUser(out List<uint> accounts);

        [OperationContract]
        bool CreateAccount(out uint accountID);

        [OperationContract]
        bool Deposit(uint accountID, uint amount, out uint balance);

        [OperationContract]
        bool Withdraw(uint accountID, uint amount, out uint balance, out string status);

        [OperationContract]
        bool GetBalance(uint accountID, out uint balance);

        [OperationContract]
        bool GetOwner(uint accountID, out uint userID);

        [OperationContract]
        bool CreateUser(string fname, string lname, out uint userID);

        [OperationContract]
        bool GetUserName(out string fname, out string lname);

        [OperationContract]
        bool SetUserName(string fname, string lname);

        [OperationContract]
        bool GetUserTransactions(out List<uint> transactions);

        [OperationContract]
        bool GetAccountTransactions(uint accountID, out List<uint> transactions);

        [OperationContract]
        bool NewTransaction(out uint transactionID, uint senderAccount, uint receiverAccount, uint amount, out string status);

        [OperationContract]
        string GetTransactionData(uint transactionID, out uint senderAccount, out uint receiverAccount, out uint amount);
    }
}
