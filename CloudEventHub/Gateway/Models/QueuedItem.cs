namespace EventsGateway.Gateway
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using EventsGateway.Common;
    using Newtonsoft.Json.Converters;

    //--//

    [DataContract]
    public class QueuedItem
    {
        [DataMember( Name = "serializedData" )]
        public string JsonData { get; set; }
    }

    public static class DataTransforms
    {
        public static QueuedItem QueuedItemFromSensorDataContract( SensorDataContract sensorData, ILogger logger = null )
        {
            if( sensorData == null )
            {
                return null;
            }

            QueuedItem result = null;
            try
            {
                result = new QueuedItem
                {
                    JsonData = JsonConvert.SerializeObject( sensorData )
                };
            }
            catch( Exception ex )
            {
                if( logger != null )
                {
                    logger.LogError( "Error on serialize item: " + ex.Message );
                }
            }

            return result;
        }

        public static SensorDataContract SensorDataContractFromString( string data, ILogger logger = null )
        {
            SensorDataContract result;
            try
            {
                result =
                    JsonConvert.DeserializeObject<SensorDataContract>( data, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" } );
            }
            catch( Exception ex )
            {
                result = null;
                //TODO: maybe better to add some metrics instead
                if( logger != null )
                {
                    logger.LogError( "Error on deserialize item: " + ex.Message );
                }
            }

            return result;
        }
        
        public static SensorDataContract SensorDataContractFromQueuedItem( QueuedItem data, ILogger logger = null )
        {
            if( data == null )
            {
                return null;
            }

            SensorDataContract result = SensorDataContractFromString( data.JsonData );
            return result;
        }

        public static SensorDataContract AddTimeCreated( SensorDataContract data )
        {
            if( data == null )
            {
                return null;
            }

            SensorDataContract result = data;
            if( result.TimeCreated == default( DateTime ) )
            {
                var creationTime = DateTime.UtcNow;
                result.TimeCreated = creationTime;
            }

            return result;
        }

        public static SensorDataContract AddIPToLocation( SensorDataContract data, string gatewayIPAddressString )
        {
            if( data == null )
            {
                return null;
            }

            SensorDataContract result = data;
            if( result.Location == null )
            {
                result.Location = "Unknown" + '\n';
            }
            else
            {
                result.Location = result.Location + '\n';
            }

            if( gatewayIPAddressString != null )
            {
                result.Location += gatewayIPAddressString;
            }

            return result;
        }
    }
}
