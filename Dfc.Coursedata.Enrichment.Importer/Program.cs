using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.Coursedata.Enrichment.Common.Interfaces;
using Dfc.Coursedata.Enrichment.Importer.Data;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.Coursedata.Enrichment.Importer.Entities.CSV;
using Dfc.Coursedata.Enrichment.Importer.Gremlin;
using Dfc.Coursedata.Enrichment.Importer.Interfaces;
using Dfc.Coursedata.Enrichment.Services;
using Dfc.Coursedata.Enrichment.Services.Enums;
using Dfc.Coursedata.Enrichment.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dfc.Coursedata.Enrichment.Importer
{
    internal class Program
    {
        public static IConfiguration Configuration { get; set; }
        private static bool _insertProviders = false;
        private static bool _insertLarsData = false;
        private static bool _insertILRData = false;
        private static bool _loadWholeModel = true;


        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            //var pcd = new ProviderCourseData();
            //var gremlInsert = new GremlinInsert();

            var gremlInsert = serviceProvider.GetService<IGremlinInsert>();
            var pcd = serviceProvider.GetService<IProviderCourseData>();
            
            if (_insertProviders)
            {
                var p = pcd.GetProviderData().Result;
                gremlInsert.InsertProviders(p);
            }

            if (_insertLarsData)
            {
                
                IResult<ILarsSearchResult> larsData = null;
                Task.Run(async () =>
                {
                    larsData = await GetLarsData(serviceProvider);
                }).GetAwaiter().GetResult();

                if (larsData != null)
                {
                    gremlInsert.InsertLars(larsData);
                }
            }

            if (_insertILRData)
            {
                var ilrData = pcd.GetILRData();

                var enumerable = ilrData as ILR[] ?? ilrData.ToArray();

                gremlInsert.InsertIlrData(enumerable.ToList());

                
                //if (enumerable.ToList().Count > 0)
                //{
                //    gremlInsert.InsertEdges(enumerable);
                //}
            }

            if (_loadWholeModel)
            {
                var provider = pcd.GetProvider();
                var courseDetails = pcd.GetCourseDetails();
                var venues = pcd.GetVenues();
                //var qualifications = pcd.GetQualifications();
                var opportunities = pcd.GetOpportunities();

                gremlInsert.InsertCsvProvider(provider);
                gremlInsert.InsertCsvCourseDetails(provider.UKPRN, courseDetails);  // creates course node and coursedetail node, creates edges provider/course, course/qualification
                gremlInsert.InsertCsvVenues(venues);                                // load venues and then opportunities to link venue to course
                gremlInsert.InsertCsvOpportunities(opportunities);

                //gremlInsert.InsertCsvCourseDetailsOnly(provider.UKPRN, courseDetails);
            }

            // Exit program
            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadLine();
        }


        private static async Task<IResult<ILarsSearchResult>> GetLarsData(ServiceProvider serviceProvider)
        {
            var searchCriteria = new LarsSearchCriteria("business", 10, 20, null, new[]
            {
                LarsSearchFacet.AwardOrgCode,
                LarsSearchFacet.NotionalNVQLevelv2
            });

            var serv = serviceProvider.GetService<ILarsSearchService>();
            var result = await serv.SearchAsync(searchCriteria);

            return result;
        }


        public static ServiceProvider ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddTransient(provider => new HttpClient());
            services.Configure<LarsSearchSettings>(Configuration.GetSection("LarsSearchSettings"));
            services.AddScoped<ILarsSearchService, LarsSearchService>();

            services.Configure<GremlinCosmosDbSettings>(Configuration.GetSection("GremlinCosmosDbSettings"));
            services.AddScoped<IGremlinBase, GremlinBase>();
            services.AddScoped<IGremlinInsert, GremlinInsert>();

            services.Configure<UkrlpSettings>(Configuration.GetSection("UkrlpSettings"));
            services.AddScoped<IProviderCourseData, ProviderCourseData>();

            return services.BuildServiceProvider();
        }
    }
}
