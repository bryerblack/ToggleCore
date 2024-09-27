using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleCoreLibrary.Operations
{
    public class DynamicRulesHandler
    {
        private static DynamicRulesMapper mapper = null;

        public DynamicRulesHandler() { }

        public static DynamicRulesMapper GetMapper() 
        {
            if (mapper == null)
            {
                mapper = new DynamicRulesConfigMapper();
            }
            return mapper; 
        }

        public static void SetMapper(DynamicRulesMapper dynamicRulesMapper)
        {
            mapper = dynamicRulesMapper;
        }
    }
}
