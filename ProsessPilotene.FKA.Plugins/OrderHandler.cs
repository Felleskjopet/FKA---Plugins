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
            // A method for TTR to check if an sales person can close the order
            try
            {
                // Check if the order has an order type "Order" or that there is an "supplier" for the order
                if (postEntity.GetAttributeValue<OptionSetValue>("pp_contractordercode").Value != 2 || postEntity.GetAttributeValue<OptionSetValue>("pp_suppliercode") == null)
                    return;

                // Check if the order has been marked as "Approved by Sales Manager"
                if (postEntity.GetAttributeValue<bool>("pp_approvedbysalesmanagercode").Equals(true))
                    return;

                // Throw errormessage to the user
                throw new InvalidPluginExecutionException(
                    "Orderen må godkjennes av salgssjef før den kan lukkes.\n\n");

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}

