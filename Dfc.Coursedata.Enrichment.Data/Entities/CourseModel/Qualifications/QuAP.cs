using System;
using System.Collections.Generic;
using Dfc.Coursedata.Enrichment.Data.Interfaces.Qualifications;

namespace Dfc.Coursedata.Enrichment.Data.Entities.CourseModel.Qualifications
{
    public class QuAP : ValueObject<QuAP>, IQuAP
    {
        public Guid ID { get; }
        public Qualification Qualification { get; }
        public Providers.Provider Provider { get; }

        public QuAP(
            Guid id,
            Qualification qualification,
            Providers.Provider provider)
        {
            Throw.IfNull(id, nameof(id));
            Throw.IfNull(qualification, nameof(qualification));
            Throw.IfNull(provider, nameof(provider));

            ID = id;
            Qualification = qualification;
            Provider = provider;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ID;
            yield return Qualification;
            yield return Provider;
        }
    }
}
