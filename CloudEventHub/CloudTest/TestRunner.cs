namespace EventsGateway.Test
{
    using System;
    using System.Configuration;
    using Newtonsoft.Json;
    using EventsGateway.Common;
    using EventsGateway.Gateway;

    //--//

    public class TestRunner
    {

        private static void TestMockData( ILogger logger )
        {
            /////////////////////////////////////////////////////////////////////////////////////////////
            // Test core service 
            //
            CoreTest mockDataTest = new CoreTest( logger );
            mockDataTest.Run( );
            Console.WriteLine( String.Format( "Core Test completed" ) );
        }

        private static void TestRealData( ILogger logger )
        {
            /////////////////////////////////////////////////////////////////////////////////////////////
            // Test Socket
            //
            RealDataTest realDataTest = new RealDataTest( logger );
            realDataTest.Run( );
            Console.WriteLine( String.Format( "Socket Test completed" ) );
        }
         
        static void Main( string[] args )
        {
            // we do not need a tunable logger, but this is a nice way to test it...
            ILogger logger = NLogEventLogger.Instance; //TunableLogger.FromLogger(
                //    SafeLogger.FromLogger( TestLogger.Instance )
                 //   );

            //TunableLogger.LoggingLevel loggingLevel = TunableLogger.LevelFromString( ConfigurationManager.AppSettings.Get( "LoggingLevel" ) );

            //logger.Level = ( loggingLevel != TunableLogger.LoggingLevel.Undefined ) ? loggingLevel : TunableLogger.LoggingLevel.Errors;

            if(args.Length == 0)
            {
                //if started without arguments
                TestMockData( logger );
            }

            foreach( string t in args )
            {
                switch( t.Substring( 0, 1 ).Replace( "/", "-" ) + t.Substring( 1 ).ToLowerInvariant( ) )
                {
                    case "-MockData":
                        TestMockData( logger );
                        break;
                    //case "-WebService":
                    //    TestWebService( logger );
                    //    break;
                    //case "-Socket":
                    //    TestSocket( logger );
                    //    break;
                    //case "-AllTimeBounded":
                    //    TestMockData( logger );
                    //    TestWebService( logger );
                    //    TestSocket( logger );
                    //    break;
                    case "-RealData":
                        TestRealData( logger );
                        break;
                }
            }

            // wait for logging tasks to complete
            Console.WriteLine( "Press enter to exit" );
            Console.ReadLine( );
        }
    }
}
