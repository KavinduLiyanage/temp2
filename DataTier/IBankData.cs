using System.Collections.Generic;
using System.ServiceModel;

namespace DataTier
{
    [ServiceContract]
    public interface IBankData
    {
        [OperationContract]
        bool CommitData();

        [OperationContract]
        bool GetAccountIdsByUser(uint userID, out List<uint> accounts);

        [OperationContract]
        bool CreateAccount(uint userID, out uint accountID);

        [OperationContract]
        bool Deposit(uint accountID, uint amount, out uint balance);

        [OperationContract]
        bool Withdraw(uint accountID, uint amount, out uint balance);

        [OperationContract]
        bool GetBalance(uint accountID, out uint balance);

        [OperationContract]
        bool GetOwner(uint accountID, out uint userID);

        [OperationContract]
        bool GetUsers(out List<uint> users);

        [OperationContract]
        bool CreateUser(string fname, string lname, out uint userID);

        [OperationContract]
        bool GetUserName(uint userID, out string fname, out string lname);

        [OperationContract]
        bool SetUserName(uint userID, string fname, string lname);

        [OperationContract]
        bool GetTransactions(out List<uint> transactions);

        [OperationContract]
        bool NewTransaction(out uint transactionID, uint senderAccount, uint receiverAccount, uint amount);

        [OperationContract]
        bool GetTransactionData(uint transactionID, out uint senderAccount, out uint receiverAccount, out uint amount);
    }
}
