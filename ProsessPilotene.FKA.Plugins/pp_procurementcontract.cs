using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ProsessPilotene.FKA.Plugins
{
    public class pp_procurementcontract : IPlugin
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
                if (entity.LogicalName != "pp_procurementcontract")
                    return;
                
                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    switch (context.MessageName.ToLower())
                    {
                        case "update":
                        {
                            //skip trace to increase performace for high volume updates                     
                            if (context.Depth >= 1)
                                new ProcurmentContractHandler().HandleCountryScore(postEntity, service, false);
                            break;
                        }
                        case "create":
                        {
                            tracingService.Trace("ProsessPilotene.FKA.Plugins.pp_procurementcontract - OnCreate triggered. V.2018.03.17");
                            ProcurmentContractHandler handler =  new ProcurmentContractHandler();
                            handler.HandleTeamRoles(postEntity, service);
                            tracingService.Trace(handler.HandleCountryScore(postEntity, service, true));
                            break;
                        }
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in the plug-in: pp_procurementcontract. " + ex.Message, ex);
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
