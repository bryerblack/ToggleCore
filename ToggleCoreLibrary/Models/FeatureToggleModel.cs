using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleCoreLibrary.Models
{
    [Table("featuretoggles")]
    [PrimaryKey(nameof(ToggleId))]
    public class FeatureToggleModel
    {
        public string ToggleId;
        public bool Toggle { get; set; }
        public FeatureToggleType? FeatureToggleType { get; set; }
        public DateOnly? CreationDate { get; set; }

        [NotMapped]
        public Dictionary<string, List<string>> AdditionalRules { get; set; }
        public string? AdditionalRulesJson { get; set; }

        public FeatureToggleModel() { }

        public FeatureToggleModel(string toggleId, bool toggle, FeatureToggleType? featureToggleType,
            DateOnly? creationDate, Dictionary<string, List<string>>? additionalRules = null)
        {
            ToggleId = toggleId;
            Toggle = toggle;
            FeatureToggleType = featureToggleType;
            CreationDate = creationDate;
            AdditionalRules = additionalRules ?? new Dictionary<string, List<string>> { };
        }
    }

    public enum FeatureToggleType
    {
        RELEASE,
        EXPERIMENT,
        PERMISSIONING,
        OPS
    }
}
