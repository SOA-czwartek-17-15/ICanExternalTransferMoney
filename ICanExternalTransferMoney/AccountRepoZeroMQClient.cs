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
    public class AccountRepoZeroMQClient
    {
        private ZmqContext context;
        private ZmqSocket client;
        private string address;

        public AccountRepoZeroMQClient(string _address)
        {
            address = _address;
        }

        public Account GetAccountById(Guid to)
        {
            try
            {
                context = ZmqContext.Create();
                client = context.CreateSocket(SocketType.REQ);
                client.Connect(address);
                JSONMessage message = new JSONMessage();
                message.Service = "ICanExternalTransferMoney";
                message.Function = "GetAccountById";
                message.Parameters = new string[] { JsonConvert.SerializeObject(to) };
                client.Send(JsonConvert.SerializeObject(message), Encoding.Unicode);
                string response = client.Receive(Encoding.Unicode);
                //client.Disconnect(address);

                JSONMessage resp = JsonConvert.DeserializeObject<JSONMessage>(response);
                return JsonConvert.DeserializeObject<Account>(resp.ReponseString);
            }
            catch (Exception ex) { throw new EndpointNotFoundException(); }
        }

        public bool ChangeAccountBalance(Guid to, long p)
        {
            try
            {
                context = ZmqContext.Create();
                client = context.CreateSocket(SocketType.REQ);
                client.Connect(address);
                JSONMessage message = new JSONMessage();
                message.Service = "ICanExternalTransferMoney";
                message.Function = "ChangeAccountBalance";
                message.Parameters = new string[] { JsonConvert.SerializeObject(to), JsonConvert.SerializeObject(p) };
                client.Send(JsonConvert.SerializeObject(message), Encoding.Unicode);
                string response = client.Receive(Encoding.Unicode);
                //client.Disconnect(address);

                JSONMessage resp = JsonConvert.DeserializeObject<JSONMessage>(response);
                return JsonConvert.DeserializeObject<bool>(resp.ReponseString);
            }
            catch (Exception ex) { throw new EndpointNotFoundException(); }
        }
    }
}
