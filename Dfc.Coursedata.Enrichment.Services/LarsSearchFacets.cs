using System;
using System.Collections.Generic;
using System.Text;
using Dfc.Coursedata.Enrichment.Common;
using Dfc.Coursedata.Enrichment.Services.Interfaces;

namespace Dfc.Coursedata.Enrichment.Services
{
    public class LarsSearchFacets : ValueObject<LarsSearchFacets>, ILarsSearchFacets
    {
        public string NotionalNVQLevelv2ODataType { get; }
        public IEnumerable<SearchFacet> NotionalNVQLevelv2 { get; }
        public string AwardOrgCodeODataType { get; }
        public IEnumerable<SearchFacet> AwardOrgCode { get; }

        public LarsSearchFacets(
            string notionalNVQLevelv2ODataType,
            IEnumerable<SearchFacet> notionalNVQLevelv2,
            string awardOrgCodeODataType,
            IEnumerable<SearchFacet> awardOrgCode)
        {
            Throw.IfNullOrWhiteSpace(notionalNVQLevelv2ODataType, nameof(notionalNVQLevelv2ODataType));
            Throw.IfNullOrEmpty(notionalNVQLevelv2, nameof(notionalNVQLevelv2));
            Throw.IfNullOrWhiteSpace(awardOrgCodeODataType, nameof(awardOrgCodeODataType));
            Throw.IfNullOrEmpty(awardOrgCode, nameof(awardOrgCode));

            NotionalNVQLevelv2ODataType = notionalNVQLevelv2ODataType;
            NotionalNVQLevelv2 = notionalNVQLevelv2;
            AwardOrgCodeODataType = awardOrgCodeODataType;
            AwardOrgCode = awardOrgCode;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return NotionalNVQLevelv2ODataType;
            yield return NotionalNVQLevelv2;
            yield return AwardOrgCodeODataType;
            yield return AwardOrgCode;
        }
    }
}
