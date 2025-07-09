using BuildingManager.API.Application.Features.Blocks.Commands.CreateBlock;
using BuildingManager.API.Application.Features.Blocks.Commands.DeleteBlock;
using BuildingManager.API.Application.Features.Blocks.Commands.UpdateBlock;
using BuildingManager.API.Application.Features.Blocks.Queries.GetAllBlocks;
using BuildingManager.API.Application.Features.Blocks.Queries.GetBlockById;
using MediatR;
using Microsoft.AspNetCore.Authorization; // For later use
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Add authorization policy as needed
    public class BlocksController : ControllerBase
    {
        private readonly ISender _mediator;

        public BlocksController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        // [Authorize(Policy = "CanCreateBlock")]
        public async Task<IActionResult> CreateBlock([FromBody] CreateBlockCommand command)
        {
            var blockPublicId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetBlockByPublicId), new { publicId = blockPublicId }, new { Id = blockPublicId });
        }

        [HttpGet("{publicId:guid}")]
        public async Task<IActionResult> GetBlockByPublicId(Guid publicId)
        {
            var query = new GetBlockByIdQuery { PublicId = publicId };
            var result = await _mediator.Send(query);
            return result != null ? Ok(result) : NotFound($"Block with PublicId {publicId} not found.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlocks([FromQuery] GetAllBlocksQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{publicId:guid}")]
        // [Authorize(Policy = "CanUpdateBlock")]
        public async Task<IActionResult> UpdateBlock(Guid publicId, [FromBody] UpdateBlockCommand command)
        {
            if (publicId != command.PublicId)
            {
                return BadRequest("Route PublicId does not match command PublicId.");
            }
            var success = await _mediator.Send(command);
            return success ? NoContent() : NotFound($"Block with PublicId {publicId} not found for update.");
        }

        [HttpDelete("{publicId:guid}")]
        // [Authorize(Policy = "CanDeleteBlock")]
        public async Task<IActionResult> DeleteBlock(Guid publicId)
        {
            // DeletedByUserId would come from authenticated user claims
            var command = new DeleteBlockCommand { PublicId = publicId /*, DeletedByUserId = GetCurrentUserId() */ };
            var success = await _mediator.Send(command);

            if (!success)
            {
                var block = await _mediator.Send(new GetBlockByIdQuery { PublicId = publicId });
                if (block == null) return NotFound($"Block with PublicId {publicId} not found.");
                return BadRequest("Block could not be deleted, possibly due to active units or other constraints.");
            }
            return NoContent();
        }
    }
}
