using BarRaider.SdTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMacro.Actions
{
    public class MacroSettingsBase
    {
        [JsonProperty(PropertyName = "inputText")]
        public string InputText { get; set; }

        [JsonProperty(PropertyName = "delay")]
        public int Delay { get; set; }

        [JsonProperty(PropertyName = "enterMode")]
        public bool EnterMode { get; set; }

        [JsonProperty(PropertyName = "forcedMacro")]
        public bool ForcedMacro { get; set; }

        [JsonProperty(PropertyName = "keydownDelay")]
        public bool KeydownDelay { get; set; }

        [JsonProperty(PropertyName = "ignoreNewline")]
        public bool IgnoreNewline { get; set; }

        [JsonProperty(PropertyName = "loadFromFiles")]
        public bool LoadFromFiles { get; set; }

        [FilenameProperty]
        [JsonProperty(PropertyName = "primaryInputFile")]
        public string PrimaryInputFile { get; set; }
    }
}
