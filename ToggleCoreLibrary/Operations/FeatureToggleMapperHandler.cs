using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleCoreLibrary.Operations
{
    public static class FeatureToggleMapperHandler
    {
        private static FeatureToggleMapper mapper = new FeatureToggleDbMapper();

        public static FeatureToggleMapper GetMapper() { return mapper;}

        public static void SetMapper(FeatureToggleMapper featureToggleMapper)
        {
            mapper = featureToggleMapper;
        }
    }
}
