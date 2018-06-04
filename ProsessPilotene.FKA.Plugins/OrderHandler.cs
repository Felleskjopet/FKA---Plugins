using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ProsessPilotene.FKA.Plugins
{
    internal class OrderHandler
    {
        public void ClosingRequirements(Entity postEntity, IOrganizationService service)
        {
            try
            {
                if (postEntity.GetAttributeValue<OptionSetValue>("pp_contractordercode").Value != 2 && !postEntity.Contains("pp_suppliercode"))
                {
                    return;
                    
                }
                else
                {
                    if (postEntity.GetAttributeValue<bool>("pp_approvedbysalesmanagercode").Equals(true))
                    {
                        return;
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException(
                            "Orderen må godkjennes av salgssjef før den kan lukkes.\n\n");
                    }
                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}

