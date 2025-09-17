using DbManager.Application.Common.Models;
using DbManager.Application.UseCases.ConnectionConfigs.Commands;
using DbManager.Application.UseCases.ConnectionConfigs.Models;
using DbManager.Application.UseCases.ConnectionConfigs.Queries;

namespace DbManager.Api.Controllers;

/// <summary>
/// ConnectionsController
/// </summary>
/// <param name="sender"></param>
[Authorize]
[Route("api/connections")]
[ApiController]
public class ConnectionsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Create and (optionally) save a new database connection.
    /// </summary>
    [HttpPost("connect")]
    public async Task<ActionResult<Guid>> Connect([FromBody] ConnectToServerCommand request)
    {
        return await sender.Send(request);
    }

    /// <summary>
    /// Tests an existing connection by Id.
    /// </summary>
    [HttpGet("{connectionId:guid}/test")]
    public async Task<ActionResult> TestConnection(Guid connectionId)
    {
        var result = await sender.Send(new TestConnectionCommand(connectionId));

        return Ok(new { Success = result });
    }

    /// <summary>
    /// Remove a connection by Id (from cache).
    /// </summary>
    [HttpDelete("{connectionId:guid}")]
    public async Task<IActionResult> Remove(Guid connectionId)
    {
        await sender.Send(new RemoveConnectionCommand(connectionId));
        return NoContent();
    }

    /// <summary>
    /// List all active connection Ids and server names (from cache).
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<PagedList<ActiveConnectionModel>>> GetActiveConnections()
    {
        var result = await sender.Send(new GetActiveConnectionsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of saved database connection configurations.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedList<ConnectionConfigModel>>> GetSavedConnections()
    {
        var result = await sender.Send(new GetSavedConnectionConfigsQuery());
        return Ok(result);
    }
}
