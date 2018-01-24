using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProsessPilotene.FKA.Plugins
{
    public class OpportunityClose : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
             (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // The InputParameters collection contains all the data passed in the message request.
            Entity entity = (Entity)context.InputParameters["OpportunityClose"];

            try
            {
                var competitorRef = entity.GetAttributeValue<EntityReference>("competitorid");
                var description = entity.GetAttributeValue<string>("description");
                
                if (string.IsNullOrWhiteSpace(description) || competitorRef == null)
                    throw new InvalidPluginExecutionException("Du må angi en konkurrent og en beskrivelse for å få lukket salgsmuligheten.\n\n");
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}

