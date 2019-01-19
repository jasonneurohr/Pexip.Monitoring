using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;
using Pexip.Lib;

namespace Pexip.Monitoring.Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            ISQLiteServices sqliteServices = new SQLiteServices();

            // Uses the location of the currently running assembly as the base path,
            // to resolve the appsettings.json location.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();

            // Get the log file path from the appsettings.json
            string logFile = configuration.GetValue<string>("logFilePath");


            var pexipParamaters = configuration.GetSection("pexipManager");
            int historyHours = configuration.GetValue<int>("historyHours") * -1;

            IManagerNode managerNode = new ManagerNode(
                pexipParamaters.GetValue<string>("username"),
                pexipParamaters.GetValue<string>("password"),
                pexipParamaters.GetValue<string>("address"));

            PexipOps pexipOps = new PexipOps(managerNode, sqliteServices);
            pexipOps.GetConferenceHistory(historyHours);
            pexipOps.GetParticipantHistory(historyHours);
        }
    }
}
