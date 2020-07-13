using IsAnythingServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace IsAnythingServer.Controllers.Api
{
    [Route("api")]
    public class RecordController : ControllerBase
    {
        private readonly ILogger<RecordController> _logger;
        private readonly IDataStorage _dataStorage;

        public RecordController(
            IDataStorage dataStorage,
            ILogger<RecordController> logger)
        {
            _dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(logger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("is")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecordDTO))]
        public IActionResult GetIsValue([FromQuery] GetRecordRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var value = _dataStorage.ReadRecord(request.Subject, request.Predicate);
            return Ok(new RecordDTO
            {
                Subject = request.Subject,
                Predicate = request.Predicate,
                Value = value
            });
        }

        [HttpPut("is")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecordDTO))]
        public IActionResult PutIsValue([FromBody] PutRecordRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultValue = _dataStorage.WriteRecord(request.Subject, request.Predicate, request.Value);
            return Ok(new RecordDTO
            {
                Subject = request.Subject,
                Predicate = request.Predicate,
                Value = resultValue
            });
        }
    }
}
