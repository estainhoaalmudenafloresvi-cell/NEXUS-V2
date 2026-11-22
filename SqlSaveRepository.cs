using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace NEXUS;

/// <summary>Persistencia real de partidas en SQL Server.</summary>
public sealed class SqlSaveRepository
{
    private readonly string connectionString;

    public SqlSaveRepository()
    {
        connectionString = Environment.GetEnvironmentVariable("NEXUS_SQL_CONNECTION")
            ?? "Server=localhost;Database=NEXUS;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;Connect Timeout=5;";
    }

    public void Guardar(GameState state)
    {
        const string query = """
            INSERT INTO dbo.Partidas (NombreExplorador, MisionActual, EstadoJson, FechaGuardado)
            VALUES (@nombre, @mision, @estado, SYSDATETIME());
            """;
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);
        string nombre = string.IsNullOrWhiteSpace(state.Nombre) ? "Nova" : state.Nombre.Trim();
        if (nombre.Length > 60) nombre = nombre[..60]; // la columna es NVARCHAR(60)
        command.Parameters.Add("@nombre", SqlDbType.NVarChar, 60).Value = nombre;
        command.Parameters.Add("@mision", SqlDbType.TinyInt).Value = (byte)Math.Clamp(state.Mision + 1, 1, 4);
        command.Parameters.Add("@estado", SqlDbType.NVarChar, -1).Value = JsonSerializer.Serialize(state);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public GameState? CargarUltima()
    {
        const string query = """
            SELECT TOP (1) EstadoJson
            FROM dbo.Partidas
            ORDER BY FechaGuardado DESC, IdPartida DESC;
            """;
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(query, connection);
        connection.Open();
        var json = command.ExecuteScalar() as string;
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<GameState>(json);
    }

    public void ProbarConexion()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
    }
}
