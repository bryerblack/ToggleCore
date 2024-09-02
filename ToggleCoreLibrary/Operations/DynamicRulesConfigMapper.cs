using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
