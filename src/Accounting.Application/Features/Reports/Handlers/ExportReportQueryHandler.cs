using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using Accounting.Application.Features.Reports.Queries;
using Accounting.Application.Common.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace Accounting.Application.Features.Reports.Handlers
{
    public class ExportReportQueryHandler : IQueryHandler<ExportReportQuery, Result<byte[]>>
    {
        private readonly IQueryHandler<GetFinancialReportQuery, Result<FinancialReportDto>> _financialReportHandler;
        private readonly IQueryHandler<GetSalesReportQuery, Result<SalesReportDto>> _salesReportHandler;
        private readonly IQueryHandler<GetProfitLossReportQuery, Result<ProfitLossReportDto>> _profitLossReportHandler;
        private readonly IQueryHandler<GetBalanceSheetReportQuery, Result<BalanceSheetReportDto>> _balanceSheetReportHandler;

        public ExportReportQueryHandler(
            IQueryHandler<GetFinancialReportQuery, Result<FinancialReportDto>> financialReportHandler,
            IQueryHandler<GetSalesReportQuery, Result<SalesReportDto>> salesReportHandler,
            IQueryHandler<GetProfitLossReportQuery, Result<ProfitLossReportDto>> profitLossReportHandler,
            IQueryHandler<GetBalanceSheetReportQuery, Result<BalanceSheetReportDto>> balanceSheetReportHandler)
        {
            _financialReportHandler = financialReportHandler;
            _salesReportHandler = salesReportHandler;
            _profitLossReportHandler = profitLossReportHandler;
            _balanceSheetReportHandler = balanceSheetReportHandler;
        }

        public async Task<Result<byte[]>> Handle(ExportReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                byte[] data;

                switch (request.ReportType.ToLower())
                {
                    case "financial":
                        var financialQuery = new GetFinancialReportQuery
                        {
                            StartDate = request.StartDate,
                            EndDate = request.EndDate
                        };
                        var financialResult = await _financialReportHandler.Handle(financialQuery, cancellationToken);
                        if (!financialResult.IsSuccess)
                            return Result.Failure<byte[]>(financialResult.Error);
                        
                        data = ExportFinancialReport(financialResult.Value, request.Format);
                        break;

                    case "sales":
                        var salesQuery = new GetSalesReportQuery
                        {
                            StartDate = request.StartDate,
                            EndDate = request.EndDate
                        };
                        var salesResult = await _salesReportHandler.Handle(salesQuery, cancellationToken);
                        if (!salesResult.IsSuccess)
                            return Result.Failure<byte[]>(salesResult.Error);
                        
                        data = ExportSalesReport(salesResult.Value, request.Format);
                        break;

                    case "profit-loss":
                        var plQuery = new GetProfitLossReportQuery
                        {
                            StartDate = request.StartDate,
                            EndDate = request.EndDate
                        };
                        var plResult = await _profitLossReportHandler.Handle(plQuery, cancellationToken);
                        if (!plResult.IsSuccess)
                            return Result.Failure<byte[]>(plResult.Error);
                        
                        data = ExportProfitLossReport(plResult.Value, request.Format);
                        break;

                    case "balance-sheet":
                        var bsQuery = new GetBalanceSheetReportQuery
                        {
                            AsOfDate = request.EndDate
                        };
                        var bsResult = await _balanceSheetReportHandler.Handle(bsQuery, cancellationToken);
                        if (!bsResult.IsSuccess)
                            return Result.Failure<byte[]>(bsResult.Error);
                        
                        data = ExportBalanceSheetReport(bsResult.Value, request.Format);
                        break;

                    default:
                        return Result.Failure<byte[]>($"Unknown report type: {request.ReportType}");
                }

                return Result<byte[]>.Success(data);
            }
            catch (Exception ex)
            {
                return Result.Failure<byte[]>($"Error exporting report: {ex.Message}");
            }
        }

        private byte[] ExportFinancialReport(FinancialReportDto report, string format)
        {
            if (format.ToLower() == "excel")
            {
                // Simplified Excel export - in production, use a library like EPPlus
                var csv = new StringBuilder();
                csv.AppendLine("Financial Report");
                csv.AppendLine($"Period: {report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}");
                csv.AppendLine($"Currency: {report.Currency}");
                csv.AppendLine();
                
                csv.AppendLine("Summary");
                csv.AppendLine($"Total Income,{report.TotalIncome}");
                csv.AppendLine($"Total Costs,{report.TotalCosts}");
                csv.AppendLine($"Net Profit,{report.NetProfit}");
                csv.AppendLine();
                
                csv.AppendLine("Income Items");
                csv.AppendLine("Category,Amount,Currency,Transaction Count");
                foreach (var item in report.IncomeItems)
                {
                    csv.AppendLine($"{item.Category},{item.Amount},{item.Currency},{item.TransactionCount}");
                }
                
                csv.AppendLine();
                csv.AppendLine("Cost Items");
                csv.AppendLine("Category,Amount,Currency,Transaction Count");
                foreach (var item in report.CostItems)
                {
                    csv.AppendLine($"{item.Category},{item.Amount},{item.Currency},{item.TransactionCount}");
                }

                return Encoding.UTF8.GetBytes(csv.ToString());
            }
            else
            {
                // JSON export
                var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                return Encoding.UTF8.GetBytes(json);
            }
        }

        private byte[] ExportSalesReport(SalesReportDto report, string format)
        {
            if (format.ToLower() == "excel")
            {
                var csv = new StringBuilder();
                csv.AppendLine("Sales Report");
                csv.AppendLine($"Period: {report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}");
                csv.AppendLine();
                
                csv.AppendLine("Summary");
                csv.AppendLine($"Total Tickets,{report.TotalTickets}");
                csv.AppendLine($"Issued Tickets,{report.IssuedTickets}");
                csv.AppendLine($"Cancelled Tickets,{report.CancelledTickets}");
                csv.AppendLine($"Total Revenue,{report.TotalRevenue}");
                csv.AppendLine($"Average Ticket Value,{report.AverageTicketValue}");
                csv.AppendLine();
                
                csv.AppendLine("Tickets by Airline");
                csv.AppendLine("Airline,Ticket Count,Revenue,Percentage");
                foreach (var item in report.TicketsByAirline)
                {
                    csv.AppendLine($"{item.Name},{item.TicketCount},{item.Revenue},{item.Percentage:F2}%");
                }

                return Encoding.UTF8.GetBytes(csv.ToString());
            }
            else
            {
                var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                return Encoding.UTF8.GetBytes(json);
            }
        }

        private byte[] ExportProfitLossReport(ProfitLossReportDto report, string format)
        {
            if (format.ToLower() == "excel")
            {
                var csv = new StringBuilder();
                csv.AppendLine("Profit & Loss Report");
                csv.AppendLine($"Period: {report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}");
                csv.AppendLine();
                
                csv.AppendLine("Summary");
                csv.AppendLine($"Total Revenue,{report.TotalRevenue}");
                csv.AppendLine($"Total Expenses,{report.TotalExpenses}");
                csv.AppendLine($"Gross Profit,{report.GrossProfit}");
                csv.AppendLine($"Net Profit,{report.NetProfit}");
                csv.AppendLine($"Profit Margin,{report.ProfitMargin:F2}%");
                csv.AppendLine();
                
                csv.AppendLine("Revenue Items");
                csv.AppendLine("Account Name,Account Code,Amount,Percentage");
                foreach (var item in report.RevenueItems)
                {
                    csv.AppendLine($"{item.AccountName},{item.AccountCode},{item.Amount},{item.Percentage:F2}%");
                }

                return Encoding.UTF8.GetBytes(csv.ToString());
            }
            else
            {
                var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                return Encoding.UTF8.GetBytes(json);
            }
        }

        private byte[] ExportBalanceSheetReport(BalanceSheetReportDto report, string format)
        {
            if (format.ToLower() == "excel")
            {
                var csv = new StringBuilder();
                csv.AppendLine("Balance Sheet Report");
                csv.AppendLine($"As of: {report.AsOfDate:yyyy-MM-dd}");
                csv.AppendLine();
                
                csv.AppendLine("Summary");
                csv.AppendLine($"Total Assets,{report.TotalAssets}");
                csv.AppendLine($"Total Liabilities,{report.TotalLiabilities}");
                csv.AppendLine($"Total Equity,{report.TotalEquity}");
                csv.AppendLine();
                
                csv.AppendLine("Assets");
                csv.AppendLine("Account Name,Account Code,Amount");
                foreach (var section in report.Assets)
                {
                    csv.AppendLine($"{section.SectionName} (Total: {section.TotalAmount})");
                    foreach (var item in section.Items)
                    {
                        csv.AppendLine($"  {item.AccountName},{item.AccountCode},{item.Amount}");
                    }
                }

                return Encoding.UTF8.GetBytes(csv.ToString());
            }
            else
            {
                var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                return Encoding.UTF8.GetBytes(json);
            }
        }
    }
}