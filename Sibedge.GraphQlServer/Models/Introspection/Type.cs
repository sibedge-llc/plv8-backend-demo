namespace Sibedge.GraphQlServer.Models.Introspection
{
    using Newtonsoft.Json;

    /// <summary> Type </summary>
    public class Type
    {
        /// <summary> Kind </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary> Name </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary> Of type </summary>
        [JsonProperty("ofType")]
        public Type OfType { get; set; }
    }
}
