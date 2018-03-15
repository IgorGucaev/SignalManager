
namespace EventsGateway.Gateway
{
    using System;
    using System.Text;
    using Newtonsoft.Json;
    using Common;
    using Common.Threading;
    using Microsoft.Azure.Devices.Client;

    using System.Threading.Tasks;
    using AzureDevices = Microsoft.Azure.Devices;
    using EventsManager.CloudEventHub.Gateway.Utils.MessageSender;
    using System.Collections.Generic;

    public class IotHubSender<T> : IMessageSender<T> // Ex MessageSender
    {
        private DeviceClientExtCollection deviceClients;

        private static readonly string _logMesagePrefix = "MessageSender error. ";

        private readonly string _defaultSubject;

        public ILogger Logger
        {
            private get;
            set;
        }

        public IotHubSender(IotHubSenderSettings senderSettings, ILogger logger)
            //string gatewayIotHubConnectionString, ILogger logger)
        {
            Logger = SafeLogger.FromLogger(logger);

#if DEBUG_LOG
            Logger.LogInfo( "Connecting to IotHub" );
#endif

            /* AzureDevices.RegistryManager registryManager = AzureDevices.RegistryManager.CreateFromConnectionString(senderSettings.IotHubConnectionString);
            AddDeviceAsync(registryManager).Wait(); */

            deviceClients = new DeviceClientExtCollection();

            foreach (DeviceBinding binding in senderSettings.Bindings)
            {
                var deviceClient = DeviceClient.CreateFromConnectionString(binding.GatewayHostName);
                
                deviceClient.OpenAsync();

                deviceClients.Append(binding.DeviceId, deviceClient);
            }
        }

        //private async Task AddDeviceAsync(AzureDevices.RegistryManager registryManager)
        //{
        //    AzureDevices.Device device;
        //    try
        //    {
        //        device = await registryManager.AddDeviceAsync(new AzureDevices.Device(DeviceName));
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is Microsoft.Azure.Devices.Client.Exceptions.DeviceAlreadyExistsException 
        //            || ex is Microsoft.Azure.Devices.Common.Exceptions.DeviceAlreadyExistsException)
        //        {
        //            device = await registryManager.GetDeviceAsync(DeviceName);
                    
        //        }
        //        else
        //            throw;
        //    }

        //    deviceKey = device.Authentication.SymmetricKey.PrimaryKey;
        //}

        public TaskWrapper SendMessage(string deviceId, T data)
        {
            TaskWrapper result = null;

            try
            {
                if (data == null)
                    return default(TaskWrapper);

                string jsonData = JsonConvert.SerializeObject(data);

                result = PrepareAndSend(deviceId, jsonData);
            }
            catch (Exception ex)
            {
                Logger.LogError(_logMesagePrefix + ex.Message);
            }

            return result;
        }

        public TaskWrapper SendSerialized(string deviceId, string jsonData)
        {
            TaskWrapper result = null;

            try
            {
                if (String.IsNullOrEmpty(jsonData))
                {
                    return default(TaskWrapper);
                }

                result = PrepareAndSend(deviceId, jsonData);
            }
            catch (Exception ex)
            {
                Logger.LogError(_logMesagePrefix + ex.Message);
            }

            return result;
        }

        public void Close()
        {
            deviceClients.Close();
        }

        private TaskWrapper PrepareAndSend(string deviceId, string jsonData)
        {
            var msg = PrepareMessage(jsonData);

            var sh = new SafeAction<Message>(m => deviceClients[deviceId].SendEventAsync(msg), Logger);

            return TaskWrapper.Run(() => sh.SafeInvoke(msg));
        }

        protected Message PrepareMessage(string serializedData, string subject = default(string))
        {
            if (subject == default(string))
                subject = _defaultSubject;

            Message message = null;

            if (!String.IsNullOrEmpty(serializedData))
            {
                message = new Message(Encoding.UTF8.GetBytes(serializedData));
                message.Properties.Add("Subject", subject);
                message.Properties.Add("CreationTime", JsonConvert.SerializeObject(DateTime.UtcNow));
            }

            return message;
        }
    }
}