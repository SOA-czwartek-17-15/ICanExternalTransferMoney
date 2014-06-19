using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;
using Contracts;
using Newtonsoft.Json;
using System.ServiceModel;

namespace ICanExternalTransferMoney
{
    class ServiceRepoZeroMQClient
    {
        private ZmqContext context;
        private ZmqSocket client;
        private string address;
        public ServiceRepoZeroMQClient(string _address)
        {
            address = _address;
        }

        public bool Alive()
        {
            try
            {
                context = ZmqContext.Create();
                client = context.CreateSocket(SocketType.REQ);
                client.Connect(address);
                JSONMessage message = new JSONMessage();
                message.Service = "ICanExternalTransferMoney";
                message.Function = "Alive";
                message.Parameters = new string[] { "ICanExternalTransferMoney" };
                client.Send(JsonConvert.SerializeObject(message), Encoding.Unicode);
                string response = client.Receive(Encoding.Unicode);
                //client.Disconnect(address);
                return true;
            }
            catch(Exception ex) { return false; }
        }

        public bool Unregister()
        {
            try
            {
                context = ZmqContext.Create();
                client = context.CreateSocket(SocketType.REQ);
                client.Connect(address);
                JSONMessage message = new JSONMessage();
                message.Service = "ICanExternalTransferMoney";
                message.Function = "Unregister";
                message.Parameters = new string[] { "ICanExternalTransferMoney" };
                client.Send(JsonConvert.SerializeObject(message), Encoding.Unicode);
                string response = client.Receive(Encoding.Unicode);
                //client.Disconnect(address);
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public bool Register(string serviceAddress)
        {
            try
            {
                context = ZmqContext.Create();
                client = context.CreateSocket(SocketType.REQ);
                client.Connect(address);
                JSONMessage message = new JSONMessage();
                message.Service = "ICanExternalTransferMoney";
                message.Function = "Register";
                message.Parameters = new string[] { "ICanExternalTransferMoney", serviceAddress };
                client.Send(JsonConvert.SerializeObject(message), Encoding.Unicode);
                string response = client.Receive(Encoding.Unicode);
                //client.Disconnect(
                if (response != null) return true;
                else return false;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return false; }

        }

        public string GetServiceLocation(string serviceName)
        {
            try
            {
                context = ZmqContext.Create();
                client = context.CreateSocket(SocketType.REQ);
                client.Connect(address);
                JSONMessage message = new JSONMessage();
                message.Service = "ICanExternalTransferMoney";
                message.Function = "GetServiceLocation";
                message.Parameters = new string[] { serviceName };
                client.Send(JsonConvert.SerializeObject(message), Encoding.Unicode);
                string response = client.Receive(Encoding.Unicode);
                //client.Disconnect(address);

                JSONMessage resp = JsonConvert.DeserializeObject<JSONMessage>(response);
                return resp.ReponseString;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return null; }
        }
    }
}
