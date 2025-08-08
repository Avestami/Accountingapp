namespace Accounting.Application.Features.Tickets.Queries
{
    public class GetTicketByIdQuery
    {
        public int Id { get; }

        public GetTicketByIdQuery(int id)
        {
            Id = id;
        }
    }
}