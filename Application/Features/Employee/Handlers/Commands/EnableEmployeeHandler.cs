using Application.DTOs;
using Application.Exceptions;
using Application.Features.Employee.Requests.Commands;

using AutoMapper;

using Domain.Contracts.Infrastructure;

using MediatR;

using Microsoft.Extensions.Logging;


namespace Application.Features.Employee.Handlers.Commands;

public class EnableEmployeeHandler : IRequestHandler<EnableEmployeeCommand, EmployeeDto>
{
    private readonly ILogger<EnableEmployeeHandler> _logger;
    private readonly IEmployeeRepository _repo;
    private readonly IMapper _mapper;

    public EnableEmployeeHandler(ILogger<EnableEmployeeHandler> logger, IEmployeeRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EmployeeDto> Handle(EnableEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start handling EnableEmployeeCommand for EmployeeId={EmployeeId}", request.Id);

        var emp = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (emp == null)
        {
            _logger.LogWarning("Employee not found with Id={EmployeeId}", request.Id);
            throw new NotFoundException(nameof(Employee), request.Id);
        }

        emp.Enable = request.Enable;

        var updatedOk = await _repo.UpdateAsync(emp, cancellationToken);
        if (!updatedOk)
        {
            _logger.LogError("Failed to update Employee with Id={EmployeeId}", request.Id);
            throw new Exception($"Failed to update Employee ({request.Id}).");
        }

        var result = _mapper.Map<EmployeeDto>(emp);
        _logger.LogInformation("Successfully updated EmployeeId={EmployeeId} Enabled={Enabled}", emp.Id, emp.Enable);

        return result;
    }
}
