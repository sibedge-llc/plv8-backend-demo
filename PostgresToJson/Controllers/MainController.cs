using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace PostgresToJson.Controllers
{
    using Dapper;
    using Newtonsoft.Json;

    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private IDbConnection _connection;

        public MainController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpPost("sql")]
        public async Task<IActionResult> ExecuteSql([FromBody] Container container)
        {
            try
            {
                string json = (await _connection.QueryAsync<string>(container.Content)).First();
                return Content(json, "application/json");
            }
            catch (Exception e)
            {
                return this.BadRequest();
            }
        }

        [HttpPost("graphql")]
        public async Task<IActionResult> ExecuteGrapgQl([FromBody] Container container)
        {
            try
            {
                var sql = $"SELECT public.execute_qraphql('{container.Content}', 'public');";
                string json = (await _connection.QueryAsync<string>(sql)).First();
                return Content(json, "application/json");
            }
            catch (Exception e)
            {
                return this.BadRequest();
            }
        }
    }
}
