using DbManager.Application.UseCases.QueryActions.Commands;
using DbManager.Application.UseCases.QueryActions.Models;
using DbManager.Application.UseCases.QueryActions.Queries;

namespace DbManager.Api.Controllers;

/// <summary>
/// QueryController
/// </summary>
/// <param name="sender"></param>
[Authorize]
[Route("api/query")]
[ApiController]
public class QueryController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Execute SQL query (SELECT, DML, DDL).
    /// </summary>
    [HttpPost("execute")]
    public async Task<ActionResult<ExecuteQueryResultModel>> Execute([FromBody] QueryExecuteCommand request)
    {
        try
        {
            var result = await sender.Send(request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get history of executed queries.
    /// </summary>
    [HttpGet("history/{connectionId:guid}")]
    public async Task<ActionResult<List<QueryHistoryModel>>> GetHistory(Guid connectionId)
    {
        var history = await sender.Send(new GetQueriesHistoryQuery(connectionId));
        return Ok(history);
    }

    /// <summary>
    /// Export query results to CSV.
    /// </summary>
    [HttpPost("export-csv")]
    public async Task<IActionResult> ExportCsv([FromBody] QueryResultExportCsvQuery request)
    {
        var csvBytes = await sender.Send(request);

        return File(csvBytes, "text/csv", "query-result.csv");
    }
}
