using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperMacro.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace SuperMacro.Actions
{
    [PluginActionId("com.barraider.supermacrotoggle")]
    public class ToggleSuperMacroAction : SuperMacroBase
    {
        protected class PluginSettings : MacroSettingsBase
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    InputText = String.Empty,
                    SecondaryText = String.Empty,
                    PrimaryImageFilename = String.Empty,
                    SecondaryImageFilename = string.Empty,
                    Delay = 10,
                    EnterMode = false,
                    ForcedMacro = false,
                    KeydownDelay = false,
                    IgnoreNewline = false,
                    LoadFromFiles = false,
                    PrimaryInputFile = String.Empty,
                    SecondaryInputFile = String.Empty
                };

                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "secondaryInputFile")]
            public string SecondaryInputFile { get; set; }

            [JsonProperty(PropertyName = "secondaryText")]
            public string SecondaryText { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "primaryImage")]
            public string PrimaryImageFilename { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "secondaryImage")]
            public string SecondaryImageFilename { get; set; }
        }

        protected PluginSettings Settings
        {
            get
            {
                var result = settings as PluginSettings;
                if (result == null)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, "Cannot convert MacroSettingsBase to PluginSettings");
                }
                return result;
            }
            set
            {
                settings = value;
            }
        }

        #region Private members

        string primaryFile = null;
        string secondaryFile = null;
        bool isPrimary = false;
        string secondaryMacro = String.Empty;

        #endregion

        #region Public Methods

        public ToggleSuperMacroAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                Settings = PluginSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(Settings));
            }
            else
            {
                Settings = payload.Settings.ToObject<PluginSettings>();
                HandleFilenames();
            }
            LoadMacros();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Pressed {this.GetType()}");
            if (inputRunning)
            {
                forceStop = true;
                return;
            }

            LoadMacros(); // Refresh the macros, relevant for if you're reading from a file
            forceStop = false;
            isPrimary = !isPrimary;
            string text = isPrimary ? primaryMacro : secondaryMacro;
            SendInput(text);
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Destructor called");
        }

        public async override void OnTick()
        {
            string imgBase64;
            if (isPrimary)
            {
                imgBase64 = Properties.Settings.Default.TogglePrimary;

                if (!String.IsNullOrWhiteSpace(primaryFile))
                {
                    imgBase64 = primaryFile;
                }
                await Connection.SetImageAsync(imgBase64);
            }
            else
            {
                imgBase64 = Properties.Settings.Default.ToggleSecondary;
                if (!String.IsNullOrWhiteSpace(secondaryFile))
                {
                    imgBase64 = secondaryFile;
                }
                await Connection.SetImageAsync(imgBase64);
            }
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            bool prevKeydownDelay = Settings.KeydownDelay;
            // New in StreamDeck-Tools v2.0:
            Tools.AutoPopulateSettings(Settings, payload.Settings);
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Settings loaded: {payload.Settings}");
            if (Settings.KeydownDelay && !prevKeydownDelay && Settings.Delay < CommandTools.RECOMMENDED_KEYDOWN_DELAY)
            {
                Settings.Delay = CommandTools.RECOMMENDED_KEYDOWN_DELAY;
            }
            HandleFilenames();
            LoadMacros();
        }

        #endregion

        #region Private Methods

        private void HandleFilenames()
        {
            primaryFile = Tools.FileToBase64(Settings.PrimaryImageFilename, true);
            secondaryFile = Tools.FileToBase64(Settings.SecondaryImageFilename, true);
            Connection.SetSettingsAsync(JObject.FromObject(Settings));
        }

        protected override void LoadMacros()
        {
            base.LoadMacros();

            // Handle the secondary
            secondaryMacro = String.Empty;
            if (settings.LoadFromFiles)
            {
                secondaryMacro = ReadFile(Settings.SecondaryInputFile);
            }
            else
            {
                secondaryMacro = Settings.SecondaryText;
            }
        }

        #endregion

    }
}
