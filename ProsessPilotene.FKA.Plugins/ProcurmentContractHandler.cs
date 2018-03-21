using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

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
                CreateContractResponsibles(postEntity, contractTeams, service);
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

        private void CreateContractResponsibles(Entity postEntity, List<Entity> contractTeams, IOrganizationService service)
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

        public string HandleCountryScore(Entity contract, IOrganizationService service, bool tracking)
        {
            try
            {
                if (contract.GetAttributeValue<EntityReference>("pp_accountid") == null)
                {
                    if (tracking)
                        return "Plugin HandleCountryScore does not contain relation to Supplier for contract " + contract.GetAttributeValue<string>("name");
                    return null;
                }

                //Get Parent Account
                Guid accountId = contract.GetAttributeValue<EntityReference>("pp_accountid").Id;
                Entity account = service.Retrieve("account", accountId, new ColumnSet(true));
                string country = GetAccountCountry(account);
                if (country == "")
                {
                    if (tracking)
                        return "Plugin HandleCountryScore cant find supplier country in GetAccountCountry(" + accountId + ")";
                    return null;
                }

                //Using Parent Country find Score
                Guid scorePerCountryId = GetScorePerCountryId(country, service);
                if (scorePerCountryId == Guid.Empty)
                {
                    if (tracking)
                        return "Plugin HandleCountryScore cant find score in GetScorePerCountry(" + country + ", service). Please check that " + country + " is added to pp_scorepercountry";
                    return null;
                }
                
                Entity scourePerCountry = service.Retrieve("pp_scorepercountry", scorePerCountryId, new ColumnSet(true));
                decimal score = scourePerCountry.GetAttributeValue<decimal>("pp_fairtradescore");        

                //Update Contract
                Entity newContract = new Entity(contract.LogicalName)
                {
                    Id = contract.Id,
                    ["pp_score2"] = score,
                    ["pp_countryscoresid"] = new EntityReference(scourePerCountry.LogicalName, scorePerCountryId)
                };
                service.Update(newContract);
                {
                    if (tracking)
                        return "Plugin HandleCountryScore successfully updated with score = " + score + " for " + country;
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private string GetAccountCountry(Entity account)
        {
            try
            {
                string country = "";
                if (account.Attributes.Contains("address2_country"))
                    country = account.GetAttributeValue<string>("address2_country");
                else if (account.Attributes.Contains("address1_country"))
                    country = account.GetAttributeValue<string>("address1_country");
                return country;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private Guid GetScorePerCountryId(string country, IOrganizationService service)
        {
            try
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = "pp_scorepercountry";
                query.ColumnSet = new ColumnSet();
                query.ColumnSet.Columns.Add("pp_scorepercountryid");
                query.Criteria.AddCondition("pp_country", ConditionOperator.Equal, country);
                EntityCollection scoreOfCountries = service.RetrieveMultiple(query);
                if (scoreOfCountries.Entities.Count == 0)
                    return Guid.Empty;
                Entity scoreOfCountry = scoreOfCountries.Entities[0];
                return scoreOfCountry.GetAttributeValue<Guid>("pp_scorepercountryid");
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
