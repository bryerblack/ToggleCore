﻿namespace ToggleCoreLibrary.Operations
{
    public class FeatureToggleMapperHandler
    {
        private static FeatureToggleMapper mapper = null;

        public FeatureToggleMapperHandler() { }

        public static FeatureToggleMapper GetMapper() 
        { 
            if (mapper == null)
            {
                mapper = new FeatureToggleDbMapper();
            }
            return mapper;
        }

        public static void SetMapper(FeatureToggleMapper featureToggleMapper)
        {
            mapper = featureToggleMapper;
        }
    }
}
