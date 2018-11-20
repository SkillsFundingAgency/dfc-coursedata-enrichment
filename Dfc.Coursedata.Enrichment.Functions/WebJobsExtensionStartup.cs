using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Dfc.Coursedata.Enrichment.Data.Gremlin;
using Dfc.Coursedata.Enrichment.Data.Interfaces;
using Dfc.Coursedata.Enrichment.Functions;
using Dfc.Coursedata.Enrichment.Functions.Common.DependencyInjection;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "A Web Jobs Extension Sample")]
namespace Dfc.Coursedata.Enrichment.Functions
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Environment.CurrentDirectory)
            //    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            //    .AddEnvironmentVariables()
            //    .Build();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables().Build();

            builder.Services.AddSingleton<IConfiguration>(configuration);
            // builder.Services.Configure<FindAndExtractSettings>(configuration.GetSection(nameof(FindAndExtractSettings)));

            builder.Services.Configure<GremlinCosmosDbSettings>(configuration.GetSection("GremlinCosmosDbSettings"));
            builder.Services.AddScoped<IGremlinBase, GremlinBase>();
            builder.Services.AddScoped<IGremlinQuery, GremlinQuery>();
        }
    }
}
