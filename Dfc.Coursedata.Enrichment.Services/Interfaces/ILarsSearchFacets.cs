﻿using System.Collections.Generic;

namespace Dfc.Coursedata.Enrichment.Services.Interfaces
{
    public interface ILarsSearchFacets
    {
        string NotionalNVQLevelv2ODataType { get; }
        string AwardOrgCodeODataType { get; }
        IEnumerable<SearchFacet> NotionalNVQLevelv2 { get; }
        IEnumerable<SearchFacet> AwardOrgCode { get; }
    }
}
