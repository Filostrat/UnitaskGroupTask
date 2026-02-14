using Domain.Models;


namespace Domain.Contracts.Infrastructure;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Employee>> GetAllAsync(CancellationToken ct = default);
    Task<List<Employee>> GetSubtreeAsync(int rootId, CancellationToken ct = default);
    Task<bool> UpdateAsync(Employee employee, CancellationToken ct = default);
}