using EventsManager.LocalEventStorage.Abstractions;
using EventsManager.LocalEventStorage.Core;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;

namespace EventsManager.LocalEventStorage.Test
{
    public class SQLiteTest : BaseTest
    {
        [Theory]
        [MemberData(nameof(TestDataGenerator.GetRandomAntennaSignalRow), MemberType = typeof(TestDataGenerator))]
        public void SQLite__Insert_Rows(string serializedSignal, DateTime timestamp)
        {
            // Prepare
            CacheContext db = this.GetService<CacheContext>();

            // Pre-validate

            // Perform
            int countBefore = db.Signals.Count();
            db.Signals.Add(Signal.New(deviceId: "fooDevice", data: serializedSignal, timestamp: timestamp));
            db.SaveChanges();
            db.Dispose();

            int countAfter = db.Signals.Count();
            string lastData = db.Signals.Last().Data;
            db.Dispose();

            // Post-Validate
            Assert.Equal(countBefore + 1, countAfter); // signal inserted correctly
            Assert.Equal(serializedSignal, lastData);
        }

        [Fact]
        public void SQLite__Truncate_Table()
        {
            // Prepare
            SignalRepository signalRepo = new SignalRepository(this.GetService<CacheContext>());

            int count = signalRepo.QueryAll.Count();
            if (count == 0)
                signalRepo.Add(Signal.New(deviceId: "fooDevice", data: "foo", timestamp: DateTime.Now));

            // Pre-validate
            Assert.NotEmpty(signalRepo.QueryAll);

            // Perform
            signalRepo.Truncate();

            // Post-validate
            Assert.Empty(signalRepo.QueryAll);
            //adapter.Disconnect();
        }

        [Fact]
        public void SQLite__Insert_Rows__InSingleTransaction_Stress()
        {
            // Prepare
            SignalRepository signalRepo = new SignalRepository(this.GetService<CacheContext>());
            int countBefore = signalRepo.QueryAll.Count();

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            int testSize = 100000;

            // Pre-validate

            // Perform
            List<Signal> signals = new List<Signal>();
            List<object[]> rows = new List<object[]>();
            for (int i = 0; i < testSize; i++)
                signals.Add(Signal.New(deviceId: "fooDevice", data: Guid.NewGuid().ToString(), timestamp: DateTime.Now));

            Stopwatch sw = new Stopwatch();
            sw.Start();
            signalRepo.Add(signals);
            sw.Stop();


            // Post-validate
            int countAfter = signalRepo.QueryAll.Count();
            Assert.Equal(countBefore + testSize, countAfter); // signal inserted correctly
            double averageInsertSpeed = testSize / sw.Elapsed.TotalSeconds;
            Assert.True(averageInsertSpeed > 5000);
        }

        [Fact]
        public void SQLite_Delete_Rows_Created_Before_Date()
        {
            // Prepare
            SignalRepository signalRepo = new SignalRepository(this.GetService<CacheContext>());
            int count = signalRepo.QueryAll.Count();

            // Pre-validate
            signalRepo.Truncate();
            Assert.Empty(signalRepo.QueryAll);
            DateTime border = DateTime.Now.AddMinutes(-1);
            string topGuid = Guid.NewGuid().ToString();
            signalRepo.Add(new Signal[] {
                Signal.New(deviceId: "fooDevice", data: Guid.NewGuid().ToString(), timestamp: DateTime.Now.AddMinutes(-3)),
                Signal.New(deviceId: "fooDevice", data: Guid.NewGuid().ToString(), timestamp: DateTime.Now.AddMinutes(-2)),
                Signal.New(deviceId: "fooDevice", data: topGuid, timestamp: border)
            });

            Assert.Equal(3, signalRepo.QueryAll.Count());

            // Perform
            List<Signal> toDelete = signalRepo.QueryAll.Where(x => x.Timestamp < border).ToList();
            signalRepo.Delete(toDelete); // Truncate();

            // Post-validate
            Assert.Equal(1, signalRepo.QueryAll.Count());
            Assert.Equal(topGuid, signalRepo.QueryAll.First().Data);
        }

        /// <summary>
        /// Checks that the database is restored if necessary
        /// </summary>
        [Fact]
        public void SQLite_DatabaseFile_Restoring()
        {
            // Prepare
            string tempDbPath = System.IO.Path.GetFileName(System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".db"));
            CacheSettings settings = new CacheSettings
            {
                DbFilepath = tempDbPath,
                CreateDbScriptPath = "cache.sql"
            };

            // Pre-validate
            Assert.False(System.IO.File.Exists(settings.DbFilepath)); // Database file must be saved
            Assert.True(System.IO.File.Exists(settings.CreateDbScriptPath));

            // Perform
            CacheContext db = new CacheContext(settings);
            int count = db.Signals.Count();

            // Post-validate
            System.IO.File.Delete(tempDbPath);
        }
    }
}