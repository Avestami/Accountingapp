namespace Accounting.Application.Features.Counterparties.Queries
{
    public class GetCounterpartyByIdQuery
    {
        public int Id { get; }

        public GetCounterpartyByIdQuery(int id)
        {
            Id = id;
        }
    }
}