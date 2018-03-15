namespace EventsGateway.Gateway
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "GatewayService")]
    public interface IGatewayService
    {
        [OperationContract]
        int Enqueue(string deviceId, string jsonData);
    }
}