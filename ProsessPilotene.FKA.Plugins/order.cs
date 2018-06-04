﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ProsessPilotene.FKA.Plugins
{
    public class order : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
               (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
        context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];
                Entity postEntity = context.PostEntityImages["PostImage"];

                // Verify that the target entity represents an account.
                // If not, this plug-in was not registered correctly.
                if (entity.LogicalName != "order")
                    return;

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                
            
                    try
                    {
                        if (context.MessageName.ToLower() == "create")
                            new AccountManagerHandler().HandleRoles(postEntity, service);

                    if (context.MessageName.ToLower() == "update")
                        new AccountManagerHandler().HandleRoles(postEntity, service);
                }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in the plug-in: order.cs" + ex.Message, ex);
                    }

                    catch (Exception ex)
                    {
                        tracingService.Trace("Plugin error: {0}", ex.ToString());
                        throw;
                    }
            }
        }
    }
}