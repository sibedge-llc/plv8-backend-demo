namespace Sibedge.GraphQlServer.Models.Introspection
{
    using System.Collections.Generic;

    /// <summary> Introspection schema </summary>
    public class IntrospectionSchema
    {
        /// <summary> Directives </summary>
        public List<object> Directives { get; set; }

        /// <summary> Mutation type </summary>
        public NamedItem MutationType { get; set; }

        /// <summary> Subscription type </summary>
        public NamedItem SubscriptionType { get; set; }

        /// <summary> Query type </summary>
        public NamedItem QueryType { get; set; }

        /// <summary> Types </summary>
        public List<Element> Types { get; set; }
    }
}
