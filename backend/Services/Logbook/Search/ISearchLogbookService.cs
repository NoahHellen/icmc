using Audacia.Commands;
using MediatR;
using Services.Logbook.Dtos;

namespace Services.Logbook.Search;

/// <summary>
/// Interface for the search logbook service.
/// </summary>
public interface ISearchLogbookService : IRequestHandler<SearchLogbookRequest, CommandResult<LogDto[]>>
{
}
