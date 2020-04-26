namespace Sibedge.GraphQlServer.Controllers
{
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Introspection;
    using Newtonsoft.Json;

    /// <summary> Main controller </summary>
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private IDbConnection _connection;

        /// <summary> ctor </summary>
        public MainController(IDbConnection connection)
        {
            _connection = connection;
        }

        /// <summary> Execute graphQL query </summary>
        /// <param name="query"> Query data </param>
        [HttpPost]
        public async ValueTask<IActionResult> Query([FromBody]GraphQlQuery query)
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

            return Content(json, "application/json");
        }

        private async ValueTask<IntrospectionSchema> GetIntrospectionData()
        {
            var result = new IntrospectionSchema();

            return result;
        }
    }
}
