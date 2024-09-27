using System.Configuration;

namespace ToggleCoreLibrary.Operations
{
    public class DynamicRulesConfigMapper : DynamicRulesMapper
    {
        public override string Map(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
