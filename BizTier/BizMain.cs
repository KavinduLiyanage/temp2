using System;
using System.ServiceModel;

namespace BizTier
{
    class BizMain
    {
        static void Main(string[] args)
        {
            ServiceHost host;
            try
            {
                //Creating the Business Tier Server connection
                NetTcpBinding tcpBinding = new NetTcpBinding();
                host = new ServiceHost(typeof(IBankBizImpl));
                string url = "net.tcp://localhost:50002/BankClient";
                host.AddServiceEndpoint(typeof(IBankBiz), tcpBinding, url);
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
