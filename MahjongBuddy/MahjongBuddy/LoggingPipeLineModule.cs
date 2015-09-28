using log4net;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MahjongBuddy
{
    public class LoggingPipeLineModule : HubPipelineModule 
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LoggingPipeLineModule));

        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            _logger.Debug("=> Invoking " + context.MethodDescriptor.Name + " on hub " + context.MethodDescriptor.Hub.Name);
            return base.OnBeforeIncoming(context);
        }
        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            _logger.Debug("<= Invoking " + context.Invocation.Method + " on client hub " + context.Invocation.Hub);
            return base.OnBeforeOutgoing(context);
        } 
    }
}