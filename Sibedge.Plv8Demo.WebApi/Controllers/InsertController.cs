namespace Sibedge.Plv8Demo.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Plv8Server;
    using Plv8Server.Helpers;

    /// <summary> Controller for inserting / updating data </summary>
    [ApiController]
    [Route("[controller]")]
    public class InsertController : ControllerBase
    {
        private readonly InsertService _service;

        /// <inheritdoc />
        public InsertController(InsertService service)
        {
            this._service = service;
        }

        /// <summary> Insert data into table </summary>
        /// <param name="tableName"> Table name </param>
        /// <param name="body"> Data to insert </param>
        /// <param name="idKeys"> Primary key fields </param>
        [HttpPost("{tableName}")]
        public ValueTask<IActionResult> Insert(
            [FromBody] JsonElement body,
            [FromRoute] string tableName,
            [FromQuery] IList<string> idKeys = null)
        {
            var data = JsonSerializer.Serialize(body);

            return this._service.Insert(tableName, data, idKeys)
                .GetFuncData(this);
        }

        /// <summary> Insert data into table </summary>
        /// <param name="tableName"> Table name </param>
        /// <param name="body"> Data to insert </param>
        /// <param name="idKeys"> Primary key fields </param>
        [HttpPut("{tableName}")]
        public ValueTask<IActionResult> Upsert(
            [FromBody] JsonElement body,
            [FromRoute] string tableName,
            [FromQuery] IList<string> idKeys = null)
        {
            var data = JsonSerializer.Serialize(body);

            return this._service.Insert(tableName, data, idKeys, true)
                .GetFuncData(this);
        }
    }
}
