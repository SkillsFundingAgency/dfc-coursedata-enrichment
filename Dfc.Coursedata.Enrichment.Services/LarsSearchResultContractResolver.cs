﻿using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Dfc.Coursedata.Enrichment.Services
{
    public class LarsSearchResultContractResolver : PrivateSetterContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            var names = new Dictionary<string, string>
            {
                { "ODataContext", "@odata.context" },
                { "ODataCount", "@odata.count" },
                { "SearchFacets", "@search.facets" },
                { "NotionalNVQLevelv2ODataType", "NotionalNVQLevelv2@odata.type" },
                { "AwardOrgCodeODataType", "AwardOrgCode@odata.type" },
                { "SearchScore", "@search.score" }
            };

            if (names.ContainsKey(propertyName))
            {
                names.TryGetValue(propertyName, out string name);
                return name;
            }

            return propertyName;
        }
    }
}
