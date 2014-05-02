using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IServiceRepository
    {
        [OperationContract]
        void RegisterService(string serviceName, string serviceAddress);
        [OperationContract]
        void UnregisterService(string serviceName);
        [OperationContract]
        string GetServiceAddress(string serviceName);
        [OperationContract]
        void IsAlive(string serviceName);
    }
}