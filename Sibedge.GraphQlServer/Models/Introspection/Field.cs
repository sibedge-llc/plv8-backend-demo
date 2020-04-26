namespace Sibedge.GraphQlServer.Models.Introspection
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Field
    {
        /// <summary> Name </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary> Description </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary> Args </summary>
        [JsonProperty("args")]
        public List<object> InputFields { get; set; } = new List<object>();

        /// <summary> Name </summary>
        [JsonProperty("isDeprecated")]
        public bool IsDeprecated { get; set; }

        /// <summary> DeprecationReason </summary>
        [JsonProperty("deprecation reason")]
        public string DeprecationReason { get; set; }

        /// <summary> Type </summary>
        [JsonProperty("type")]
        public Type Type { get; set; }
    }
}
