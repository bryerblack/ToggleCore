using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToggleCoreLibrary.Models;

namespace ToggleCoreLibrary.Operations
{
    public abstract class FeatureToggleMapper
    {
        public abstract FeatureToggleModel Map(string featureToggleId);
    }
}
