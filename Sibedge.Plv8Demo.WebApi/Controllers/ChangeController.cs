namespace Sibedge.Plv8Demo.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Plv8Server;
    using Plv8Server.Helpers;
    using Plv8Server.Models;

    /// <summary> Controller for inserting / updating data </summary>
    [ApiController]
    [Route("[controller]")]
    public class ChangeController : ControllerBase
    {
        private readonly ChangeService service;

        /// <inheritdoc />
        public ChangeController(ChangeService service)
        {
            this.service = service;
        }

        /// <summary> Insert data into table </summary>
        /// <param name="body"> Data to insert </param>
        /// <param name="tableName"> Table name </param>
        /// <param name="cancellationToken"> Propagates notification that operations should be canceled </param>
        /// <param name="idKeys"> Primary key fields </param>
        [HttpPost("{tableName}")]
        public Task<IActionResult> Insert(
            [FromBody] JsonElement body,
            [FromRoute] string tableName,
            CancellationToken cancellationToken,
            [FromQuery] IList<string> idKeys = null)
        {
            var data = JsonSerializer.Serialize(body);

            return this.service.Change(tableName, data, idKeys, ChangeOperation.Insert, null, cancellationToken)
                .GetFuncData(this);
        }

        /// <summary> Update or insert data in table </summary>
        /// <param name="body"> Data to update </param>
        /// <param name="tableName"> Table name </param>
        /// <param name="cancellationToken"> Propagates notification that operations should be canceled </param>
        /// <param name="idKeys"> Primary key fields </param>
        [HttpPut("{tableName}")]
        public Task<IActionResult> Upsert(
            [FromBody] JsonElement body,
            [FromRoute] string tableName,
            CancellationToken cancellationToken,
            [FromQuery] IList<string> idKeys = null)
        {
            var data = JsonSerializer.Serialize(body);

            return this.service.Change(tableName, data, idKeys, ChangeOperation.Update, null, cancellationToken)
                .GetFuncData(this);
        }

        /// <summary> Delete data from table </summary>
        /// <param name="body"> Data to update </param>
        /// <param name="tableName"> Table name </param>
        /// <param name="cancellationToken"> Propagates notification that operations should be canceled </param>
        /// <param name="idKeys"> Primary key fields </param>
        [HttpDelete("{tableName}")]
        public Task<IActionResult> Delete(
            [FromBody] JsonElement body,
            [FromRoute] string tableName,
            CancellationToken cancellationToken,
            [FromQuery] IList<string> idKeys = null)
        {
            var data = JsonSerializer.Serialize(body);

            return this.service.Change(tableName, data, idKeys, ChangeOperation.Delete, null, cancellationToken)
                .GetFuncData(this);
        }

        /// <summary> Returns Open API schema JSON for change methods </summary>
        /// <param name="tableName"> Table name (if specified, schema data will be created only for this table) </param>
        /// <param name="cancellationToken"> Propagates notification that operations should be canceled </param>
        [HttpGet]
        public ValueTask<IActionResult> GetSchema([FromQuery] string tableName, CancellationToken cancellationToken)
        {
            var filter = !string.IsNullOrWhiteSpace(tableName) ? new[] { tableName } : null;
            var schema = this.service.GetSchema("change", filter, cancellationToken);

            return schema.GetFuncData(this);
        }
    }
}
