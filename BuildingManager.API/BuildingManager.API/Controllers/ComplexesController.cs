using BuildingManager.API.Application.Features.Complexes.Commands.CreateComplex;
using BuildingManager.API.Application.Features.Complexes.Commands.DeleteComplex;
using BuildingManager.API.Application.Features.Complexes.Commands.UpdateComplex;
using BuildingManager.API.Application.Features.Complexes.Queries.GetAllComplexes;
using BuildingManager.API.Application.Features.Complexes.Queries.GetComplexById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Add authorization policy as needed, e.g., [Authorize(Roles = "SuperAdmin,ComplexManager")]
    public class ComplexesController : ControllerBase
    {
        private readonly ISender _mediator;

        public ComplexesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        // [Authorize(Policy = "CanCreateComplex")] // Example policy
        public async Task<IActionResult> CreateComplex([FromBody] CreateComplexCommand command)
        {
            var complexPublicId = await _mediator.Send(command);
            // Return a 201 Created response with the location of the new resource
            return CreatedAtAction(nameof(GetComplexByPublicId), new { publicId = complexPublicId }, new { Id = complexPublicId });
        }

        [HttpGet("{publicId:guid}")]
        public async Task<IActionResult> GetComplexByPublicId(Guid publicId)
        {
            var query = new GetComplexByIdQuery { PublicId = publicId };
            var result = await _mediator.Send(query);
            return result != null ? Ok(result) : NotFound($"Complex with PublicId {publicId} not found.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComplexes([FromQuery] GetAllComplexesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{publicId:guid}")]
        // [Authorize(Policy = "CanUpdateComplex")]
        public async Task<IActionResult> UpdateComplex(Guid publicId, [FromBody] UpdateComplexCommand command)
        {
            if (publicId != command.PublicId)
            {
                return BadRequest("Route PublicId does not match command PublicId.");
            }
            var success = await _mediator.Send(command);
            return success ? NoContent() : NotFound($"Complex with PublicId {publicId} not found for update.");
        }

        [HttpDelete("{publicId:guid}")]
        // [Authorize(Policy = "CanDeleteComplex")]
        public async Task<IActionResult> DeleteComplex(Guid publicId)
        {
            // In a real app, DeletedByUserId would come from the authenticated user's claims
            var command = new DeleteComplexCommand { PublicId = publicId /*, DeletedByUserId = GetCurrentUserId() */ };
            var success = await _mediator.Send(command);

            if (!success)
            {
                 // You might want to distinguish between "Not Found" and "Deletion Forbidden (e.g. due to active blocks)"
                 // For now, NotFound is a generic way to indicate it wasn't deleted.
                var complex = await _mediator.Send(new GetComplexByIdQuery { PublicId = publicId });
                if (complex == null) return NotFound($"Complex with PublicId {publicId} not found.");
                return BadRequest("Complex could not be deleted, possibly due to active blocks or other constraints.");
            }
            return NoContent();
        }
    }
}
