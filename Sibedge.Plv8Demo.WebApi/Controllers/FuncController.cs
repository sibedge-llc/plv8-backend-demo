namespace Sibedge.Plv8Demo.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services;

    /// <summary> V8 functions controller </summary>
    [ApiController]
    [Route("[controller]")]
    public class FuncController : ControllerBase
    {
        private readonly V8FuncService _service;

        /// <inheritdoc />
        public FuncController(V8FuncService service)
        {
            this._service = service;
        }
    }
}
