using ContractsAsync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace ICanExternalTransferMoney
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class CanExternalTransferMoneyAsync : ICanExternalTransferMoneyAsync
    {
        private ZmqContext context;
        private ZmqSocket socket;

        public CanExternalTransferMoneyAsync()
        {
            context = ZmqContext.Create();
            socket = context.CreateSocket(SocketType.PUB);
            socket.Bind("tcp://127.0.0.1:5000");
            Console.WriteLine("0MQClient on!");
        }

        public void ReceiveExternalMoney(string from, Guid to, double howMany, Guid ident, string serviceName) 
        {
            socket.Send("Receive: " + from + " - " + howMany + ident + " - " + serviceName, Encoding.UTF8);
        }
        public void SendExternalMoney(Guid from, string to, double howMany, Guid ident, string serviceName) 
        {
            socket.Send("Send: " + to + " - " + howMany + ident + " - " + serviceName, Encoding.UTF8);
        }
        public void CheckOperationStatus(Guid ident, string serviceName) 
        {
            socket.Send("Check: " + ident + " - " + serviceName, Encoding.UTF8);
        }

        public void CloseSocket() { socket.Close(); }


    }
}
