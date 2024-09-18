using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToggleCoreLibrary.Contexts;
using ToggleCoreLibrary.Models;

namespace ToggleCoreLibrary.Operations
{
    public class FeatureToggleDbMapper : FeatureToggleMapper
    {
        public ApplicationDbContext Context { get; set; }
        public static NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("FeatureToggleConfig");
        public readonly DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseSqlServer(@$"Server={section["server"]};Database={section["database"]};ConnectRetryCount=0;Integrated Security={section["integratedSecurity"]};TrustServerCertificate={section["trustServerCertificate"]}")
           .Options;

        public override FeatureToggleModel Map(string featureToggleId)
        {
            Context = new ApplicationDbContext(contextOptions);

            var featureToggle = Context.featureToggleModels.FirstOrDefault(x => x.ToggleId.Equals(featureToggleId));
            if (featureToggle == null)
            {
                return new FeatureToggleModel()
                {
                    ToggleId = featureToggleId,
                    Toggle = false,

                };
            }

            Dictionary<string, List<string>>? ruleString = null;
            if (featureToggle.AdditionalRulesJson != null)
            {
                ruleString = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(featureToggle.AdditionalRulesJson);
            }
            return new FeatureToggleModel(
                featureToggle.ToggleId,
                featureToggle.Toggle,
                featureToggle.CreationDate,
                featureToggle.ExpirationDate,
                ruleString);
        }
    }
}
