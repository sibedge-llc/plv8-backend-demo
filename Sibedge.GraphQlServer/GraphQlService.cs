namespace Sibedge.GraphQlServer
{
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Models;
    using Models.Introspection;
    using Newtonsoft.Json;

    /// <summary> GraphQL service </summary>
    public class GraphQlService
    {
        private IDbConnection _connection;

        /// <summary> ctor </summary>
        public GraphQlService(IDbConnection connection)
        {
            _connection = connection;
        }

        /// <summary> Execute graphQL query </summary>
        /// <param name="query"> Query data </param>
        public async ValueTask<string> PerformQuery(GraphQlQuery query)
        {
            string json;

            if (query.OperationName == "IntrospectionQuery")
            {
                var schema = await this.GetIntrospectionData();

                var data = new
                {
                    Data = new
                    {
                        __schema = schema
                    }
                };

                json = JsonConvert.SerializeObject(data);
            }
            else
            {
                var sql = $"SELECT graphql.execute('{query.Query}', 'public');";
                json = (await _connection.QueryAsync<string>(sql)).First();
            }

            return json;
        }

        private async ValueTask<IntrospectionSchema> GetIntrospectionData()
        {
            var result = new IntrospectionSchema();

            return result;
        }
    }
}
