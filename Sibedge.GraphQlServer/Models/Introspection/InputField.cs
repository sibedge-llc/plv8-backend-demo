namespace Sibedge.GraphQlServer.Models.Introspection
{
    using Newtonsoft.Json;

    public class InputField : FieldBase
    {
        /// <summary> Default value </summary>
        [JsonProperty("defaultValue")]
        public object DefaultValue { get; set; }
    }
}
