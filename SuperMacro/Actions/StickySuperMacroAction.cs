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
    [PluginActionId("com.barraider.supermacrostickymacro")]
    public class StickySuperMacroAction : SuperMacroBase
    {
        protected class PluginSettings : MacroSettingsBase
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
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

        #region Private members
        private const int DEFAULT_AUTO_STOP_NUM = 0;

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

        private string enabledFile = null;
        private string disabledFile = null;
        private bool keyPressed = false;
        private int autoStopNum = DEFAULT_AUTO_STOP_NUM;

        #endregion

        #region Public Methods

        public StickySuperMacroAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
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
            InitializeSettings();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Pressed {this.GetType()}");
            keyPressed = !keyPressed;
            if (keyPressed)
            {
                LoadMacros(); // Refresh the macros, relevant for if you're reading from a file
                Logger.Instance.LogMessage(TracingLevel.INFO, $"Command Started");
                SendStickyInput(primaryMacro);
            }
        }

        public async override void OnTick()
        {
            string imgBase64;
            if (keyPressed)
            {
                imgBase64 = Properties.Settings.Default.StickyEnabled;

                if (!String.IsNullOrWhiteSpace(enabledFile))
                {
                    imgBase64 = enabledFile;
                }
                await Connection.SetImageAsync(imgBase64);
            }
            else
            {
                imgBase64 = Properties.Settings.Default.StickyDisabled;
                if (!String.IsNullOrWhiteSpace(disabledFile))
                {
                    imgBase64 = disabledFile;
                }
                await Connection.SetImageAsync(imgBase64);
            }
        }


        public override void Dispose()
        {
            keyPressed = false;
            Logger.Instance.LogMessage(TracingLevel.INFO, "Destructor called");
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
            InitializeSettings();
        }

        #endregion

        #region Private Methods

        private void HandleFilenames()
        {
            enabledFile = Tools.FileToBase64(Settings.EnabledImageFilename, true);
            disabledFile = Tools.FileToBase64(Settings.DisabledImageFilename, true);
            Connection.SetSettingsAsync(JObject.FromObject(Settings));
        }

        protected async void SendStickyInput(string inputText)
        {
            if (String.IsNullOrEmpty(inputText))
            {
                return;
            }

            inputRunning = true;
            await Task.Run(() =>
            {
                InputSimulator iis = new InputSimulator();
                string text = inputText;

                if (settings.IgnoreNewline)
                {
                    text = text.Replace("\r\n", "\n").Replace("\n", "");
                }
                else if (Settings.EnterMode)
                {
                    text = text.Replace("\r\n", "\n");
                }

                bool isAutoStopMode = autoStopNum > 0;
                int counter = autoStopNum;
                while (keyPressed)
                {
                    for (int idx = 0; idx < text.Length; idx++)
                    {
                        if (!keyPressed && !Settings.RunUntilEnd) // Stop as soon as user presses button
                        {
                            break;
                        }
                        if (Settings.EnterMode && text[idx] == '\n')
                        {
                            iis.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
                        }
                        else if (text[idx] == CommandTools.MACRO_START_CHAR)
                        {
                            string macro = CommandTools.ExtractMacro(text, idx);
                            if (String.IsNullOrWhiteSpace(macro)) // Not a macro, just input the character
                            {
                                iis.Keyboard.TextEntry(text[idx]);
                            }
                            else // This is a macro, run it
                            {
                                idx += macro.Length - 1;
                                macro = macro.Substring(1, macro.Length - 2);

                                HandleMacro(macro);
                            }
                        }
                        else
                        {
                            iis.Keyboard.TextEntry(text[idx]);
                        }
                        Thread.Sleep(Settings.Delay);
                    }
                    if (isAutoStopMode)
                    {
                        counter--; // First decrease, then check if equals zero 
                        if (counter <= 0)
                        {
                            keyPressed = false;
                        }
                    }

                }
            });
            inputRunning = false;
        }

        private void InitializeSettings()
        {
            if (!Int32.TryParse(Settings.AutoStopNum, out autoStopNum))
            {
                Settings.AutoStopNum = DEFAULT_AUTO_STOP_NUM.ToString();
                SaveSettings();
            }
        }

        #endregion
    }
}
