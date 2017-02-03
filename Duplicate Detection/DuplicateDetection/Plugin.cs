using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateDetection
{
    public class Plugin : IPlugin
    {
        void IPlugin.Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                var Name = ((OptionSetValue)entity.Attributes["new_name"]).Value;

                #region  Retrieve All Account Record

                QueryExpression Account = new QueryExpression { EntityName = entity.LogicalName, ColumnSet = new ColumnSet("new_name") };
                Account.Criteria.AddCondition("new_name", ConditionOperator.Equal, Name);
                EntityCollection RetrieveAccount = service.RetrieveMultiple(Account);

                //If the retrieved Account Count Greater Than 1 , following Error Message Throw

                if (RetrieveAccount.Entities.Count > 1)
                {
                    throw new InvalidPluginExecutionException("Following Record with Same Vital Exists");

                }
                else if (RetrieveAccount.Entities.Count == 0)
                {
                    return;
                }
                #endregion
            }
        }
    }
}
