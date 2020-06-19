using BarRaider.SdTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMacro.Actions
{
    public class StickyMacroSettings : MacroSettingsBase
    {
        public const int DEFAULT_AUTO_STOP_NUM = 0;
        public static StickyMacroSettings CreateDefaultSettings()
        {
            StickyMacroSettings instance = new StickyMacroSettings
            {
                InputText = String.Empty,
                Delay = 10,
                EnterMode = false,
                ForcedMacro = false,
                KeydownDelay = false,
                IgnoreNewline = false,
                EnabledImageFilename = string.Empty,
                DisabledImageFilename = string.Empty,
                LoadFromFiles = false,
                PrimaryInputFile = String.Empty,
                RunUntilEnd = false,
                AutoStopNum = DEFAULT_AUTO_STOP_NUM.ToString()
            };

            return instance;
        }

        [FilenameProperty]
        [JsonProperty(PropertyName = "enabledImage")]
        public string EnabledImageFilename { get; set; }

        [FilenameProperty]
        [JsonProperty(PropertyName = "disabledImage")]
        public string DisabledImageFilename { get; set; }

        [JsonProperty(PropertyName = "runUntilEnd")]
        public bool RunUntilEnd { get; set; }

        [JsonProperty(PropertyName = "autoStopNum")]
        public string AutoStopNum { get; set; }

    }
}
