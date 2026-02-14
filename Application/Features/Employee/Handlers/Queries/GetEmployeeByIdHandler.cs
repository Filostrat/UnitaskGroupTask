using Application.DTOs;
using Application.Exceptions;
using Application.Features.Employee.Requests.Queries;

using AutoMapper;

using Domain.Contracts.Infrastructure;

using MediatR;

using Microsoft.Extensions.Logging;


namespace Application.Features.Employee.Handlers.Queries;

public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly ILogger<GetEmployeeByIdHandler> _logger;
    private readonly IEmployeeRepository _repo;
    private readonly IMapper _mapper;

    public GetEmployeeByIdHandler(ILogger<GetEmployeeByIdHandler> logger, IEmployeeRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start handling GetEmployeeByIdQuery for EmployeeId={EmployeeId}", request.Id);

        var subtree = await _repo.GetSubtreeAsync(request.Id, cancellationToken);
        if (subtree == null || subtree.Count == 0)
        {
            _logger.LogWarning("No employees found in subtree for EmployeeId={EmployeeId}", request.Id);
            throw new NotFoundException(nameof(Employee), request.Id);
        }

        _logger.LogInformation("Found {Count} employees in subtree for EmployeeId={EmployeeId}", subtree.Count, request.Id);

        var dtoList = _mapper.Map<List<EmployeeDto>>(subtree);
        var result = BuildTree(dtoList, request.Id);

        _logger.LogInformation("Successfully built employee tree for EmployeeId={EmployeeId}", request.Id);

        return result;
    }

    private EmployeeDto? BuildTree(List<EmployeeDto> employees,int rootId,HashSet<int>? ancestorIds = null)
    {
        var parent = employees.FirstOrDefault(x => x.Id == rootId);
        if (parent == null) 
        {
            return null;
        }

        if (ancestorIds == null)
        {
            ancestorIds = new HashSet<int>();
        }

        if (ancestorIds.Contains(parent.Id))
        {
            return null;
        }

        ancestorIds.Add(parent.Id);

        var children = employees
            .Where(x => x.ManagerId == parent.Id && x.Id != x.ManagerId)
            .DistinctBy(x => x.Id)
            .ToList();

        foreach (var child in children)
        {
            var subtree = BuildTree(employees, child.Id, new HashSet<int>(ancestorIds));
            if (subtree != null)
            {
                parent.Children.Add(subtree);
            }
        }

        return parent;
    }
}