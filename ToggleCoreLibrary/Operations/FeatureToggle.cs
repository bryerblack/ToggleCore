using ArxOne.MrAdvice.Advice;
using Microsoft.IdentityModel.Tokens;

namespace ToggleCoreLibrary.Operations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
    public class FeatureToggle : Attribute, IMethodAdvice
    {
        public string FeatureToggleId { get; set; }
        public FeatureToggleDbMapper _featureToggleDBMapper = new FeatureToggleDbMapper();
        public DynamicRulesConfigMapper _rulesMapper = new DynamicRulesConfigMapper();

        public FeatureToggle(string featureToggleId)
        {
            FeatureToggleId = featureToggleId;
        }

        public void Advise(MethodAdviceContext context)
        {
            
            var method = context.TargetMethod;
            var customAttribute = method.CustomAttributes;
            var toggleId = customAttribute.FirstOrDefault()?.ConstructorArguments.First().Value;

            if (toggleId is string x)
            {
                var model = _featureToggleDBMapper.Map(x);
                var expired = false;
                var additionalRules = true;
                
                // check aditional rules
                if (!model.AdditionalRules.IsNullOrEmpty())
                    additionalRules = CheckAdditionalRules(model.AdditionalRules);
                
                // check if feature toggle is expired
                if (model.ExpirationDate != null)
                {
                    if (model.ExpirationDate <= DateOnly.FromDateTime(DateTime.Now))
                        expired = true;
                }
                
                // implements feature toggle
                if ((model.Toggle && additionalRules) || expired)
                {
                    context.Proceed();
                }
            }
        }

        private bool CheckAdditionalRules(Dictionary<string, List<string>> additionalRules)
        {
            bool rules = false;
            foreach (var rule in additionalRules)
            {
                if (rule.Value.Contains(_rulesMapper.Map(rule.Key)))
                {
                    rules = true;
                }
            }
            return rules;
        }
    }
}
