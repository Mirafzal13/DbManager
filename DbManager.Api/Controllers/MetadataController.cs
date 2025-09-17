using DbManager.Application.UseCases.Metadata.Models;
using DbManager.Application.UseCases.Metadata.Queries;

namespace DbManager.Api.Controllers;

/// <summary>
/// MetadataController
/// </summary>
[Authorize]
[Route("api/metadata")]
[ApiController]
public class MetadataController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Returns the list of all databases.
    /// </summary>
    [HttpGet("{connectionId:guid}/databases")]
    public async Task<ActionResult<List<string>>> GetDatabases(Guid connectionId)
    {
        var result = await sender.Send(new GetServerDatabasesQuery(connectionId));
        return Ok(result);
    }

    /// <summary>
    /// Returns all schemas within the specified database.
    /// </summary>
    [HttpGet("{connectionId:guid}/{database}/schemas")]
    public async Task<ActionResult<List<string>>> GetSchemas(Guid connectionId, string database)
    {
        var result = await sender.Send(new GetDatabaseSchemasQuery(connectionId, database));
        return Ok(result);
    }

    /// <summary>
    /// Returns all tables within the specified schema.
    /// </summary>
    [HttpGet("{connectionId:guid}/{database}/{schema}/tables")]
    public async Task<ActionResult<List<string>>> GetTables(Guid connectionId, string database, string schema)
    {
        var result = await sender.Send(new GetSchemaTablesQuery(connectionId, database, schema));
        return Ok(result);
    }

    /// <summary>
    /// Returns all columns for the specified table.
    /// </summary>
    [HttpGet("{connectionId:guid}/{database}/{schema}/{table}/columns")]
    public async Task<ActionResult<List<ColumnInfoModel>>> GetColumns(Guid connectionId, string database, string schema, string table)
    {
        var result = await sender.Send(new GetTableColumnsQuery(connectionId, database, schema, table));
        return Ok(result);
    }
}
