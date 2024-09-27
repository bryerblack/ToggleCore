using ToggleCoreLibrary.Models;

namespace ToggleCoreLibrary.Operations
{
    public abstract class FeatureToggleMapper
    {
        public abstract FeatureToggleModel Map(string featureToggleId);
    }
}
