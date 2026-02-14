using Application.DTOs;
using Application.Features.Employee.Requests.Commands;
using Application.Features.Employee.Requests.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using UnitaskGroupApi.Errors;
using UnitaskGroupApi.Requests;


namespace UnitaskGroupApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Employee tree", typeof(EmployeeDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Employee not found", typeof(ApiError))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error", typeof(ApiError))]
    public async Task<ActionResult<EmployeeDto>> Get(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEmployeeByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/enable")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Employee not found", typeof(ApiError))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error", typeof(ApiError))]
    public async Task<ActionResult<EmployeeDto>> Enable(int id, [FromBody] EnableRequest request, CancellationToken ct)
    {
        var updated = await _mediator.Send(new EnableEmployeeCommand(id, request.Enable), ct);
        return Ok(updated);
    }
}