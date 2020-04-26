namespace Sibedge.GraphQlServer.Models.Introspection
{
    using System.Collections.Generic;

    /// <summary> Any element </summary>
    public class Element
    {
        /// <summary> Input fields </summary>
        public List<object> InputFields { get; set; }

        /// <summary> Name </summary>
        public string Name { get; set; }

        /// <summary> Description </summary>
        public string Description { get; set; }

        /// <summary> Interfaces </summary>
        public List<Type> Interfaces { get; set; }

        /// <summary> Enum values </summary>
        public List<string> EnumValues { get; set; }

        /// <summary> Fields </summary>
        public List<object> Fields { get; set; }

        /// <summary> Kind </summary>
        public string Kind { get; set; }

        /// <summary> Possible types </summary>
        public List<Type> PossibleTypes { get; set; }
    }
}
