namespace Sibedge.GraphQlServer.Models.Introspection
{
    /// <summary> Type </summary>
    public class Type
    {
        /// <summary> Kind </summary>
        public string Kind { get; set; }

        /// <summary> Name </summary>
        public string Name { get; set; }

        /// <summary> Of type </summary>
        public Type OfType { get; set; }
    }
}
