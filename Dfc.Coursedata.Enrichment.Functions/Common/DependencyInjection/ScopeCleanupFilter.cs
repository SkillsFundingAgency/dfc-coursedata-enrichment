﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;

namespace Dfc.Coursedata.Enrichment.Functions.Common.DependencyInjection
{
    public class ScopeCleanupFilter : IFunctionInvocationFilter, IFunctionExceptionFilter
    {
        public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
        {
            RemoveScope(exceptionContext.FunctionInstanceId);
            return Task.CompletedTask;
        }

        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            RemoveScope(executedContext.FunctionInstanceId);
            return Task.CompletedTask;
        }

        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        private void RemoveScope(Guid id)
        {
            if (ServiceScopeDictionary.TryRemove(id, out var scope))
            {
                scope.Dispose();
            }
        }
    }
}