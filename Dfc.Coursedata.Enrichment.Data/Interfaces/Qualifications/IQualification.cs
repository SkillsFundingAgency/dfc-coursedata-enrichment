﻿namespace Dfc.Coursedata.Enrichment.Data.Interfaces.Qualifications
{
    public interface IQualification
    {
        string NotionalNVQLevelv2 { get; }
        string AwardOrgCode { get; }
        string TotalQualificationTime { get; }
        string UnitType { get; }
        string AwardOrgName { get; }
        string LearnAimRef { get; }
        string LearnAimRefTitle { get; }
        string LearnDirectClassSystemCode1 { get; }
        string LearnDirectClassSystemCode2 { get; }
        string SectorSubjectAreaTier1 { get; }
        string SectorSubjectAreaTier2 { get; }
        string GuidedLearningHours { get; }

    }
}
