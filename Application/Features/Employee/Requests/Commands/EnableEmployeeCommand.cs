using Application.DTOs;

using MediatR;


namespace Application.Features.Employee.Requests.Commands;

public sealed record EnableEmployeeCommand(int Id, bool Enable) : IRequest<EmployeeDto>;