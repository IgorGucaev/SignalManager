namespace EventsGateway.Gateway
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "GatewayService")]
    public interface IGatewayService : IService
    {
        [OperationContract]
        int Enqueue(string jsonData);
    }
}