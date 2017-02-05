using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace ProsessPilotene.FKA.Plugins
{
    internal class AccountManagerHandler
    {
        public void HandleRoles(Entity postEntity, IOrganizationService service)
        {
            try
            {
                var role = postEntity.GetAttributeValue<OptionSetValue>("pp_role");
                var account = postEntity.GetAttributeValue<EntityReference>("pp_accountid");

                var query = new QueryExpression("pp_accountmanager");
                query.ColumnSet = new ColumnSet("pp_role", "pp_accountid");
                query.Criteria.AddCondition("pp_role", ConditionOperator.Equal, role.Value);
                query.Criteria.AddCondition("pp_accountid", ConditionOperator.Equal, account.Id);
                var resuts = service.RetrieveMultiple(query);

                if (resuts.Entities.Count > 1)
                    throw new InvalidPluginExecutionException("Denne rollen er allerede i bruk på denne kunden.");
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
