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
