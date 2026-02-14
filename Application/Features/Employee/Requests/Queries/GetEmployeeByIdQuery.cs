using Application.DTOs;

using MediatR;


namespace Application.Features.Employee.Requests.Queries;

public sealed record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeDto?>;