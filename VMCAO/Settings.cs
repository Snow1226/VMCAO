using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VMCAO
{
    [JsonObject(MemberSerialization.OptIn)]

    public class Settings
    {
        [JsonProperty]
        public bool PPS_AO_Enable;
        [JsonProperty]
        public bool PPS_AO_IsScalable;
        [JsonProperty]
        public float PPS_AO_Intensity;
        [JsonProperty]
        public float PPS_AO_Thickness;

        [JsonProperty]
        public float PPS_AO_Color_a;
        [JsonProperty]
        public float PPS_AO_Color_r;
        [JsonProperty]
        public float PPS_AO_Color_g;
        [JsonProperty]
        public float PPS_AO_Color_b;
    }
}
