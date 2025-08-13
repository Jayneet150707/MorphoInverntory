using Microsoft.AspNetCore.Mvc;
using MorphoInventoryApi.Models;
using MorphoInventoryApi.Services;

namespace MorphoInventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReturnController : ControllerBase
    {
        private readonly ReturnService _returnService;

        public ReturnController(ReturnService returnService)
        {
            _returnService = returnService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeviceReturnRequest>>> GetAllReturnRequests()
        {
            var requests = await _returnService.GetAllReturnRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("{requestId}")]
        public async Task<ActionResult<DeviceReturnRequest>> GetReturnRequestById(int requestId)
        {
            var request = await _returnService.GetReturnRequestByIdAsync(requestId);
            
            if (request == null)
            {
                return NotFound($"Return request with ID {requestId} not found");
            }
            
            return Ok(request);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<DeviceReturnRequest>>> GetReturnRequestsByStatus(string status)
        {
            var requests = await _returnService.GetReturnRequestsByStatusAsync(status);
            return Ok(requests);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateReturnRequest([FromBody] DeviceReturnRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var requestId = await _returnService.CreateReturnRequestAsync(request);
            
            if (requestId == null)
            {
                return BadRequest($"Device with serial number {request.SerialNumber} is not eligible for return");
            }
            
            return CreatedAtAction(nameof(GetReturnRequestById), new { requestId }, requestId);
        }

        [HttpPost("accept")]
        public async Task<ActionResult> AcceptReturnRequest([FromBody] AcceptReturnRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var success = await _returnService.AcceptReturnRequestAsync(request);
            
            if (!success)
            {
                return BadRequest("Failed to accept return request");
            }
            
            return NoContent();
        }
    }
}

