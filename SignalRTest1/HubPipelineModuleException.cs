using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRTest1
{
    public class HubPipelineModuleException : HubPipelineModule
    {
        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            var caller = invokerContext.Hub.Clients.Caller;
            caller.onGlobalException(exceptionContext.Error.Message);
            base.OnIncomingError(exceptionContext, invokerContext);
        }
    }
}