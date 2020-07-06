using IsAnythingServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace IsAnythingServer.Controllers.Api
{
    [Route("api")]
    public class IsController : ControllerBase
    {
        private readonly ILogger<IsController> _logger;
        private readonly IDataStorage _dataStorage;

        public IsController(
            IDataStorage dataStorage,
            ILogger<IsController> logger)
        {
            _dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(logger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("is")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetIsValue(string subject, string predicate)
        {
            return Ok(_dataStorage.ReadRecord(subject, predicate));
        }

        [HttpPut("is")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult PutIsValue(string subject, string predicate, bool value)
        {
            return Ok(_dataStorage.WriteRecord(subject, predicate, value));
        }
    }
}
