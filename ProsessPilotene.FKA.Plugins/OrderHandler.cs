using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProsessPilotene.FKA.Plugins
{
    public class OrderHandler : IPlugin
    {
        public void ClosingRequirements(Entity postEntity, IOrganizationService service)
        {
            try
            {
                if (postEntity.GetAttributeValue<OptionSetValue>("pp_contractordercode").equals)
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

