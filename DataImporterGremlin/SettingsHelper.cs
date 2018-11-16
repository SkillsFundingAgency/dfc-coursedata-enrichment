using Microsoft.Extensions.Configuration;

namespace Dfc.CourseData.Importer
{
    public static class SettingsHelper
    {
        /// <summary>
        /// Built config root from settings file
        /// </summary>
        public static IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

        /// <summary>
        /// Properties wrapping up app setttings
        /// </summary>
        public static string CosmosGraphDbHostname = config.GetValue<string>("APPSETTING_CosmosGraphDbHostname");
        public static int CosmosGraphDbPort = config.GetValue<int>("APPSETTING_CosmosGraphDbPort");
        public static string CosmosGraphDbAuthKey = config.GetValue<string>("APPSETTING_CosmosGraphDbAuthKey");
        public static string CosmosGraphDbDatabase = config.GetValue<string>("APPSETTING_CosmosGraphDbDatabase");
        public static string CosmosGraphDbCollection = config.GetValue<string>("APPSETTING_CosmosGraphDbCollection");
    }
}
