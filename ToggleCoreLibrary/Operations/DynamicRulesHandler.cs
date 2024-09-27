using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleCoreLibrary.Operations
{
    public static class DynamicRulesHandler
    {
        private static DynamicRulesMapper mapper = new DynamicRulesConfigMapper();

        public static DynamicRulesMapper GetMapper() { return mapper; }

        public static void SetMapper(DynamicRulesMapper dynamicRulesMapper)
        {
            mapper = dynamicRulesMapper;
        }
    }
}
