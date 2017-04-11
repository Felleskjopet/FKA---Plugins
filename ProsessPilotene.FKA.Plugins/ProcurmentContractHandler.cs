using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace ProsessPilotene.FKA.Plugins
{
    public class ProcurmentContractHandler
    {
        public void HandleTeamRoles(Entity postEntity, IOrganizationService service)
        {
            try
            {
                var procOpportunity = postEntity.GetAttributeValue<EntityReference>("pp_procurementprocessid");

                if (procOpportunity == null)
                    return;

                var contractTeams = GetContractTeams(procOpportunity, service);
                CreatContractResponsibles(postEntity, contractTeams, service);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
      
        private List<Entity> GetContractTeams(EntityReference procOpportunity, IOrganizationService service)
        {
            try
            {
                var query = new QueryExpression("pp_opportunityteam");
                query.ColumnSet = new ColumnSet("pp_userid", "pp_role", "pp_categoryteam");
                query.Criteria.AddCondition("pp_opportunityid", ConditionOperator.Equal, procOpportunity.Id);
                var results = service.RetrieveMultiple(query);

                return results.Entities.ToList();

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private void CreatContractResponsibles(Entity postEntity, List<Entity> contractTeams, IOrganizationService service)
        {
            try
            {
                foreach (var contractTeam in contractTeams)
                {
                    var entity = new Entity("pp_contractteam");
                    entity["pp_contractid"] = postEntity.ToEntityReference();
                    entity["pp_userid"] = contractTeam.GetAttributeValue<EntityReference>("pp_userid");
                    entity["pp_role"] = contractTeam.GetAttributeValue<OptionSetValue>("pp_role");
                    entity["pp_categoryteam"] = contractTeam.GetAttributeValue<bool>("pp_categoryteam");

                    service.Create(entity);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

    }
}
