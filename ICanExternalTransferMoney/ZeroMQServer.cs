using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZeroMQ;
using Contracts;

namespace ICanExternalTransferMoney
{
    class ZeroMQServer
    {
        private ZmqContext context;
        private ZmqSocket socket;
        private bool stop = false;
        private Contracts.ICanExternalTransferMoney transfer;

        public ZeroMQServer(string address, Contracts.ICanExternalTransferMoney _transfer)
        {
            context = ZmqContext.Create();
            socket = context.CreateSocket(SocketType.REP);
            socket.Bind(address);
            Console.WriteLine("0MQServer on!");
            transfer = _transfer;
        }

        public void Receive()
        {
            while (!stop)
            {
                //Thread.Sleep(3000);
                var message = socket.Receive(Encoding.Unicode, new TimeSpan(100));
                if (message == null)
                {
                    continue;
                }
                Console.WriteLine(message);

                var response = ProcessMessage(message);

                socket.Send(response, Encoding.Unicode);
            }
        }

        private string ProcessMessage(string message)
        {
            JSONMessage m = JsonConvert.DeserializeObject<JSONMessage>(message);
            switch (m.Function)
            {
                //Guid ReceiveExternalMoney(string from, Guid to, double howMany)
                case "ReceiveExternalMoney":
                    string from = m.Parameters[0];
                    Guid to = JsonConvert.DeserializeObject<Guid>(m.Parameters[1]);
                    double howMany = JsonConvert.DeserializeObject<double>(m.Parameters[2]);
                    m.ReponseString = JsonConvert.SerializeObject(transfer.ReceiveExternalMoney(from,to,howMany));
                    break;
                //Guid SendExternalMoney(Guid from, string to, double howMany)
                case "SendExternalMoney":
                    Guid fromsend = JsonConvert.DeserializeObject<Guid>(m.Parameters[0]);
                    string tosend = m.Parameters[1];
                    double howManysend = JsonConvert.DeserializeObject<double>(m.Parameters[2]);
                    m.ReponseString = JsonConvert.SerializeObject(transfer.SendExternalMoney(fromsend, tosend, howManysend));
                    break;
                default:
                    Console.WriteLine("Unknown: " + m.Service + " - " + m.Function);
                    break;
            }

            return JsonConvert.SerializeObject(m);
        }

        public void Start() { stop = false; }
        public void Stop() { stop = true; }
    }
}
