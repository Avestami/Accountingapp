using System;

namespace Accounting.Application.Features.Reports.Models
{
    public class CashFlowReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public OperatingActivitiesDto OperatingActivities { get; set; } = new();
        public InvestingActivitiesDto InvestingActivities { get; set; } = new();
        public FinancingActivitiesDto FinancingActivities { get; set; } = new();
        public decimal NetCashFlow { get; set; }
        public decimal BeginningCashBalance { get; set; }
        public decimal EndingCashBalance { get; set; }
    }

    public class OperatingActivitiesDto
    {
        public decimal CashFromTicketSales { get; set; }
        public decimal CashFromOtherIncome { get; set; }
        public decimal CashPaidForExpenses { get; set; }
        public decimal NetCashFromOperations { get; set; }
    }

    public class InvestingActivitiesDto
    {
        public decimal NetCashFromInvesting { get; set; }
    }

    public class FinancingActivitiesDto
    {
        public decimal NetCashFromFinancing { get; set; }
    }
}