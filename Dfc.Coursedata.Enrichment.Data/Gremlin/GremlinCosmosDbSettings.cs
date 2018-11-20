﻿using Dfc.Coursedata.Enrichment.Data.Interfaces;

namespace Dfc.Coursedata.Enrichment.Data.Gremlin
{
    public class GremlinCosmosDbSettings : IGremlinCosmosDbSettings
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string AuthKey { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}
