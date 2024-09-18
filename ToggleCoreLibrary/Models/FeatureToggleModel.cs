using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleCoreLibrary.Models
{
    [Table("FeatureToggle")]
    [PrimaryKey(nameof(ToggleId))]
    public class FeatureToggleModel
    {
        public string ToggleId;
        public bool Toggle { get; set; }
        public DateOnly? CreationDate { get; set; }
        public DateOnly? ExpirationDate { get; set; }

        [NotMapped]
        public Dictionary<string, List<string>> AdditionalRules { get; set; }
        public string? AdditionalRulesJson { get; set; }

        public FeatureToggleModel() { }

        public FeatureToggleModel(string toggleId, bool toggle, DateOnly? creationDate, 
            DateOnly? expirationDate, Dictionary<string, List<string>>? additionalRules = null)
        {
            ToggleId = toggleId;
            Toggle = toggle;
            CreationDate = creationDate;
            ExpirationDate = expirationDate;
            AdditionalRules = additionalRules ?? new Dictionary<string, List<string>> { };
        }
    }
}
