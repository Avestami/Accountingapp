using Accounting.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.Reports.Queries
{
    public class GetProfitLossReportQuery : IRequest<Result<ProfitLossReportDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ProfitLossReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<ProfitLossItemDto> RevenueItems { get; set; } = new();
        public List<ProfitLossItemDto> ExpenseItems { get; set; } = new();
    }

    public class ProfitLossItemDto
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}