namespace Sibedge.Plv8Demo.Services
{
    using System.Data;
    using Plv8Server;

    /// <summary> Service for executing stored functions (not only plv8) </summary>
    public class V8FuncService : V8FuncServiceBase
    {
        /// <summary> ctor </summary>
        public V8FuncService(IDbConnection connection)
            : base(connection)
        {
        }
    }
}
