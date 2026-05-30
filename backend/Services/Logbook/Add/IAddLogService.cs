using Audacia.Commands;
using MediatR;

namespace Services.Logbook.Add;

/// <summary>
/// The interface for adding a log to the logbook.
/// </summary>
public interface IAddLogService : IRequestHandler<AddLogRequest, CommandResult>
{
}