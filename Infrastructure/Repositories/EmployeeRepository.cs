using Domain.Contracts.Infrastructure;
using Domain.Models;

using Microsoft.Data.SqlClient;
using System.Data;


namespace Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly string _connectionString;

    public EmployeeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private static Employee Map(SqlDataReader r)
    {
        return new Employee
        {
            Id = r.GetInt32("Id"),
            Name = r.GetString("Name"),
            ManagerId = r.IsDBNull("ManagerId")
                ? null
                : r.GetInt32("ManagerId"),
            Enable = r.GetBoolean("Enable")
        };
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT Id, Name, ManagerId, Enable
            FROM hr.Employees
            WHERE Id = @Id
        """;

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await using var reader = await cmd.ExecuteReaderAsync(ct);

        return await reader.ReadAsync(ct)
            ? Map(reader)
            : null;
    }

    public async Task<List<Employee>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = """
            SELECT Id, Name, ManagerId, Enable
            FROM hr.Employees
        """;

        var result = new List<Employee>();

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            result.Add(Map(reader));
        }

        return result;
    }

    public async Task<List<Employee>> GetSubtreeAsync(int rootId, CancellationToken ct = default)
    {
        const string sql = """
            WITH EmployeeTree AS (
                SELECT Id, Name, ManagerId, Enable,
                       CAST('[' + CAST(Id AS NVARCHAR) + ']' AS NVARCHAR(MAX)) AS PathJson
                FROM hr.Employees
                WHERE Id = @RootId

                UNION ALL

                SELECT e.Id, e.Name, e.ManagerId, e.Enable,
                       JSON_MODIFY(t.PathJson, 'append $', e.Id) AS PathJson
                FROM hr.Employees e
                INNER JOIN EmployeeTree t ON e.ManagerId = t.Id
                WHERE NOT EXISTS (
                    SELECT 1 FROM OPENJSON(t.PathJson) AS x WHERE x.value = e.Id
                ) AND e.Id != e.ManagerId
            )
            SELECT Id, Name, ManagerId, Enable
            FROM EmployeeTree;
        """;

        var result = new List<Employee>();

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@RootId", SqlDbType.Int).Value = rootId;

        await using var reader = await cmd.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            result.Add(Map(reader));
        }

        return result;
    }

    public async Task<bool> UpdateAsync(Employee employee, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE hr.Employees
            SET Name = @Name,
                ManagerId = @ManagerId,
                Enable = @Enable
            WHERE Id = @Id
        """;

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = employee.Id;
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = employee.Name;
        cmd.Parameters.Add("@ManagerId", SqlDbType.Int)
            .Value = (object?)employee.ManagerId ?? DBNull.Value;
        cmd.Parameters.Add("@Enable", SqlDbType.Bit).Value = employee.Enable;

        var rows = await cmd.ExecuteNonQueryAsync(ct);

        return rows > 0;
    }
}