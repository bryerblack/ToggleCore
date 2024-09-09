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
            Console.WriteLine("A METHOD HAS BEEN INTERCEPTED");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(".");
                Thread.Sleep(1000);
            }

            var method = context.TargetMethod;
            var customAttribute = method.CustomAttributes;
            var toggleId = customAttribute.FirstOrDefault()?.ConstructorArguments.First().Value;

            if (toggleId is string x)
            {
                var model = _featureToggleDBMapper.Map(x);
                var additionalRules = true;
                if (!model.AdditionalRules.IsNullOrEmpty())
                    additionalRules = CheckAdditionalRules(model.AdditionalRules);
                if (model.Toggle && additionalRules)
                {
                    context.Proceed();
                }
                else
                    Console.WriteLine("METHOD OFF - CANCELING DETECTION");
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
