
namespace EventsGateway.Gateway
{
    using System;
    using System.Text;
    using Newtonsoft.Json;
    using Common;
    using Common.Threading;
    using Microsoft.Azure.Devices.Client;

    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client.Exceptions;
    using AzureDevices = Microsoft.Azure.Devices;

    //--//

    public class IotHubSender<T> : IMessageSender<T> // Ex MessageSender
    {
        protected const string DeviceName = "myDevice";
        private string deviceKey;
        private DeviceClient deviceClient;

        private static readonly string _logMesagePrefix = "MessageSender error. ";

        //--//

        private readonly string _defaultSubject;
        private readonly string _defaultDeviceId;
        private readonly string _defaultDeviceDisplayName;

        public ILogger Logger
        {
            private get;
            set;
        }

        public IotHubSender(string gatewayIotHubConnectionString, ILogger logger)
        {
            Logger = SafeLogger.FromLogger(logger);

#if DEBUG_LOG
            Logger.LogInfo( "Connecting to IotHub" );
#endif

            AzureDevices.RegistryManager registryManager = AzureDevices.RegistryManager.CreateFromConnectionString(gatewayIotHubConnectionString);
            AddDeviceAsync(registryManager).Wait();

            deviceClient = DeviceClient.CreateFromConnectionString("HostName=technobee-infrastructure-testbed-01-iot-hub.azure-devices.net;DeviceId=myDevice;SharedAccessKey=kZTuwNbRvnmW5nz6XBvORD3GDo+K5bZdWCKlQBXACjA=");
            // Create(gatewayIotHubConnectionString, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceName, deviceKey));
            deviceClient.OpenAsync();
        }

        private async Task AddDeviceAsync(AzureDevices.RegistryManager registryManager)
        {
            AzureDevices.Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new AzureDevices.Device(DeviceName));
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.Azure.Devices.Client.Exceptions.DeviceAlreadyExistsException 
                    || ex is Microsoft.Azure.Devices.Common.Exceptions.DeviceAlreadyExistsException)
                {
                    device = await registryManager.GetDeviceAsync(DeviceName);
                    
                }
                else
                    throw;
            }

            deviceKey = device.Authentication.SymmetricKey.PrimaryKey;
        }

        public TaskWrapper SendMessage(T data)
        {
            TaskWrapper result = null;

            try
            {
                if (data == null)
                {
                    return default(TaskWrapper);
                }

                string jsonData = JsonConvert.SerializeObject(data);

                result = PrepareAndSend(jsonData);
            }
            catch (Exception ex)
            {
                Logger.LogError(_logMesagePrefix + ex.Message);
            }

            return result;
        }

        public TaskWrapper SendSerialized(string jsonData)
        {
            TaskWrapper result = null;

            try
            {
                if (String.IsNullOrEmpty(jsonData))
                {
                    return default(TaskWrapper);
                }

                result = PrepareAndSend(jsonData);
            }
            catch (Exception ex)
            {
                Logger.LogError(_logMesagePrefix + ex.Message);
            }

            return result;
        }

        public void Close()
        {
            deviceClient = null;
        }

        private TaskWrapper PrepareAndSend(string jsonData)
        {
            var msg = PrepareMessage(jsonData);

            var sh = new SafeAction<Message>(m => deviceClient.SendEventAsync(msg), Logger);

            return TaskWrapper.Run(() => sh.SafeInvoke(msg));
        }

        protected Message PrepareMessage(string serializedData, string subject = default(string), string deviceId = default(string), string deviceDisplayName = default(string))
        {
            if (subject == default(string))
                subject = _defaultSubject;

            if (deviceId == default(string))
                deviceId = _defaultDeviceId;

            if (deviceDisplayName == default(string))
                deviceDisplayName = _defaultDeviceDisplayName;

            var creationTime = DateTime.UtcNow;

            Message message = null;

            if (!String.IsNullOrEmpty(serializedData))
            {
                message = new Message(Encoding.UTF8.GetBytes(serializedData));
                message.Properties.Add("Subject", subject);
                message.Properties.Add("CreationTime", JsonConvert.SerializeObject(creationTime));
            }

            return message;
        }
    }
}