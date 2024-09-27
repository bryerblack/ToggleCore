using ArxOne.MrAdvice.Advice;

namespace ToggleCoreLibrary.Operations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
    public class FeatureToggle : Attribute, IMethodAdvice
    {
        private string FeatureToggleId { get; set; }
        private FeatureToggleMapper _featureToggleMapper = FeatureToggleMapperHandler.GetMapper();
        private DynamicRulesMapper _rulesMapper = DynamicRulesHandler.GetMapper();

        public FeatureToggle(string featureToggleId)
        {
            FeatureToggleId = featureToggleId;
        }

        public void Advise(MethodAdviceContext context)
        {
            
            var method = context.TargetMethod;
            var customAttribute = method.CustomAttributes;
            var toggleId = customAttribute.FirstOrDefault()?.ConstructorArguments.First().Value;
            var specialArgs = method.GetParameters().Where(x => x.GetCustomAttributes(typeof(ArgumentBeholder), false).Length > 0);
            var isSpecialArgsNull = context.Arguments.Where((_, i) => specialArgs.Select(x => x.Position).Contains(i)).All(x => x == null);

            if ((!(specialArgs == null || specialArgs.Count() == 0) && isSpecialArgsNull) 
                || (specialArgs == null || specialArgs.Count() == 0))
            {
                if (toggleId is string x)
                {
                    var model = _featureToggleMapper.Map(x);
                    var expired = false;
                    var additionalRules = true;

                    // check aditional rules
                    if (!(model.AdditionalRules == null || model.AdditionalRules.Count() == 0))
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
