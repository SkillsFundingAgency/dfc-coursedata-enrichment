using System;
using Dfc.Coursedata.Enrichment.Importer.Data;
using Dfc.Coursedata.Enrichment.Importer.Gremlin;

namespace Dfc.CourseData.Importer
{
    class Program
    {
        private static bool _insertProviders = false;
        private static bool _insertLarsData = true; // qualifications

        // TODO: before committing to github, move sensitive data to localsettings.json

        static void Main(string[] args)
        {
            ProviderCourseData pcd = new ProviderCourseData();
            var p = pcd.GetProviderData().Result;


            if (_insertProviders)
            {
                // insert into cosmos graph db
                var gremlInsert = new Insert();
                gremlInsert.InsertProviders(p);
            }

            if (_insertLarsData)
            {

            }

            // Exit program
            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadLine();
        }

        
    }
}
