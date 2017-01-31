using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;

namespace AutoNumberGeneration
{
    public class AutoNumber : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            int autoNumber = 0;
            string prefix = string.Empty;
            string suffix = string.Empty;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                Entity entity = (Entity)context.InputParameters["Target"];
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                if (context.MessageName == "Create")
                {
                    #region On Create
                    QueryExpression query = new QueryExpression("new_autonumber");
                    FilterExpression childFilter = query.Criteria.AddFilter(LogicalOperator.And);
                    childFilter.AddCondition("new_typename", ConditionOperator.Equal, entity.LogicalName);
                    query.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("new_typename", "new_autonumberid", "new_prefix", "new_number", "new_suffix", "new_entityattributename");
                    #endregion
             
                    EntityCollection entitycollection = service.RetrieveMultiple(query);
                    if (entitycollection.Entities.Count > 0)
                    {
                        Entity _dynamicsEntity = (Entity)entitycollection.Entities[0];
                        if (_dynamicsEntity.Attributes.Contains("new_number"))
                        {
                            autoNumber =Convert.ToInt32( _dynamicsEntity.Attributes["new_number"]) + 1;
                        }

                        #region Prefix Handling

                        if (_dynamicsEntity.Attributes.Contains("new_prefix"))
                            {
                                prefix = _dynamicsEntity.Attributes["new_prefix"].ToString();
                            }
                           

                         #endregion

                        #region Updating Autonumber

                        if (_dynamicsEntity.Attributes.Contains("new_entityattributename"))
                        {
                            string _attributeName = _dynamicsEntity.Attributes["new_entityattributename"].ToString();

                            string _formatedAutoNumber = prefix + autoNumber.ToString().PadLeft(4, '0');

                            _dynamicsEntity.Attributes["new_prefix"] = prefix;

                            _dynamicsEntity.Attributes["new_number"] = autoNumber;

                           // if (entity.Attributes.Contains(_attributeName))
                            //{
                                entity.Attributes[_attributeName] = _formatedAutoNumber;
                            //}

                            service.Update(_dynamicsEntity);
                        }

                        #endregion
                    }
                }
            }
          }
        }
    }
