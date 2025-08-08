using Accounting.Domain.Enums;

namespace Accounting.Application.Features.Tickets.Queries
{
    public class GetTicketsQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public TicketStatus? Status { get; set; }
        public TicketType? Type { get; set; }
        public int? CounterpartyId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Currency { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public bool? IsWithinFiveDays { get; set; }
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
    }
}