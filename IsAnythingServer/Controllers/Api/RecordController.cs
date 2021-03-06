﻿using IsAnythingServer.Stores.Records;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace IsAnythingServer.Controllers.Api
{
    [Route("api")]
    public class RecordController : ControllerBase
    {
        private readonly ILogger<RecordController> _logger;
        private readonly IRecordStore _recordStore;

        public RecordController(
            IRecordStore recordStore,
            ILogger<RecordController> logger)
        {
            _recordStore = recordStore ?? throw new ArgumentNullException(nameof(recordStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("record")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecordDTO))]
        public async Task<IActionResult> GetRecordValueAsync([FromQuery] GetRecordRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var value = await _recordStore.GetRecordAsync(request.Subject.ToLowerInvariant(), request.Predicate.ToLowerInvariant());
            return Ok(new RecordDTO
            {
                Subject = request.Subject,
                Predicate = request.Predicate,
                Value = value
            });
        }

        [HttpPut("record")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecordDTO))]
        public async Task<IActionResult> PutRecordValueAsync([FromBody] PutRecordRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultValue = await _recordStore.CreateOrUpdateRecordAsync(
                subject: request.Subject.ToLowerInvariant(),
                predicate: request.Predicate.ToLowerInvariant(),
                date: DateTime.UtcNow.Date.ToString("dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo), 
                value: request.Value);
            return Ok(new RecordDTO
            {
                Subject = request.Subject,
                Predicate = request.Predicate,
                Value = resultValue
            });
        }
    }
}
