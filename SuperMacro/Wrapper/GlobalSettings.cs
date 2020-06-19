using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMacro.Wrapper
{
    class GlobalSettings
    {
        [JsonProperty(PropertyName = "dictToggleStates")]
        public Dictionary<string, bool> DictToggleStates { get; set; }
    }
}
