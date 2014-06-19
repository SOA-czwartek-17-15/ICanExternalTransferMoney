using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;
using Newtonsoft.Json;

namespace ZeroMQMock
{
    class Program
    {
        private ZmqContext context;
        private ZmqSocket socket;
        private bool stop = false;

        public Program(string address)
        {
            context = ZmqContext.Create();
            socket = context.CreateSocket(SocketType.REP);
            socket.Bind(address);
            Console.WriteLine("0MQServer on! - MOCK");
        }

        public void Run()
        {
            while (true)
            {
                //Thread.Sleep(3000);
                var message = socket.Receive(Encoding.Unicode, new TimeSpan(100));
                if (message == null)
                {
                    continue;
                }
                Console.WriteLine(JsonConvert.DeserializeObject<JSONMessage>(message).Function);

                var response = new JSONMessage();
                response.ReponseString = "tcp://127.0.0.1:5571";

                socket.Send(JsonConvert.SerializeObject(response), Encoding.Unicode);
            }
        }

        static void Main(string[] args)
        {
            Program program = new Program("tcp://127.0.0.1:5570");
            Thread th = new Thread(program.Run);
            th.Start();
        }
    }
}
