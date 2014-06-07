using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace ICanExternalTransferMoney
{
    class ZeroMQServer
    {
        private ZmqContext context;
        private ZmqSocket socket;

        public void Receive()
        {
            context = ZmqContext.Create();
            socket = context.CreateSocket(SocketType.SUB);
            socket.SubscribeAll();
            socket.Connect("tcp://127.0.0.1:5000");
            Console.WriteLine("0MQServer on!");
            while (true)
            {
                Thread.Sleep(3000);
                var rcvdMsg = socket.Receive(Encoding.UTF8);
                Console.WriteLine(rcvdMsg);

                //Console.WriteLine("Sending : " + replyMsg + Environment.NewLine);
                //socket.Send(replyMsg, Encoding.UTF8);
            }
        }
    }
}
