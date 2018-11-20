using System;
using Microsoft.Azure.WebJobs.Description;

namespace Dfc.Coursedata.Enrichment.Functions.Common.DependencyInjection
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}