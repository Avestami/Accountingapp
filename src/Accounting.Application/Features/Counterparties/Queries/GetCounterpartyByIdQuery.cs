using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Counterparties.Queries
{
    public class GetCounterpartyByIdQuery : IQuery<Result<CounterpartyDto>>
    {
        public int Id { get; }

        public GetCounterpartyByIdQuery(int id)
        {
            Id = id;
        }
    }
}