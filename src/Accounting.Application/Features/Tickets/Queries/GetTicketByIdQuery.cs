using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Tickets.Queries
{
    public class GetTicketByIdQuery : IQuery<Result<TicketDto>>
    {
        public int Id { get; }

        public GetTicketByIdQuery(int id)
        {
            Id = id;
        }
    }
}