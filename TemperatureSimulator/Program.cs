using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EngineSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var influxDbClient = new InfluxDbClient("http://localhost:8086/", "", "", InfluxDbVersion.Latest);
            var databaseCreateResult = await influxDbClient.Database.CreateDatabaseAsync("MyEngineData");

            Console.WriteLine("Engine started...");

            var cylinders = new List<Task>();

            for (int i = 1; i < 4; i++)
            {
                var task = SimulateData(influxDbClient, i, 100);
                cylinders.Add(task);
            }

            await Task.WhenAll(cylinders);

            Console.WriteLine("Engine Stopped...");

        }

        private static async Task SimulateData(InfluxDbClient db, int cylinder, int count = 100)
        {
            for (int i = 0; i < count; i++)
            {
                var temp = new Random().Next(1000, 1300);

                var tempData = new Point()
                {
                    Name = "temps", // serie/measurement/table to write into
                    Tags = new Dictionary<string, object>()
                      {
                        { "cylinder", cylinder }
                      },
                    Fields = new Dictionary<string, object>()
                      {
                        { "value", temp }
                      },
                    Timestamp = DateTime.UtcNow // optional (can be set to any DateTime moment)
                };

                await db.Client.WriteAsync(tempData, "MyEngineData");

            }
        }
    }
}
