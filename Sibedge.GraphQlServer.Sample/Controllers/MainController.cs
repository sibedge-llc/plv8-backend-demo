namespace Sibedge.GraphQlServer.Sample.Controllers
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
        private GraphQlService _service;

        /// <summary> ctor </summary>
        public MainController(GraphQlService service)
        {
            _service = service;
        }

        [HttpGet]
        public int Get()
        {
            return 100;
        }

        /// <summary> Execute graphQL query </summary>
        /// <param name="query"> Query data </param>
        [HttpPost]
        public async ValueTask<IActionResult> Query([FromBody]GraphQlQuery query)
        {
            var json = await _service.PerformQuery(query);

            return Content(json, "application/json");
        }
    }
}
