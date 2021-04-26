using System;
using System.ServiceModel;

namespace DataTier
{
    class DataMain
    {
        static void Main(string[] args)
        {
            ServiceHost host;
            try
            {
                //Creating a Singleton object to pass to the Business Tier Server
                IBankDataImpl bankData = IBankDataImpl.GetBankDatImpl();

                //Creating the Server connection
                NetTcpBinding tcpBinding = new NetTcpBinding();
                host = new ServiceHost(bankData);
                string url = "net.tcp://localhost:50001/Bank";
                host.AddServiceEndpoint(typeof(IBankData), tcpBinding, url);
                host.Open();

                Console.WriteLine("Server started successfully!\nPress \"Enter\" to exit....");
                Console.ReadLine();
                host.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot connect to the server...\nError : " + e.Message);
            }
        }
    }
}
