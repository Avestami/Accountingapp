using Accounting.Application.Common.Models;
using Accounting.Application.Features.Reports.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Reports.Services
{
    public interface IReportGenerationService
    {
        Task<Result<SalesReportDto>> GenerateSalesReportAsync(ReportFilterDto filters, CancellationToken cancellationToken = default);
        Task<Result<FinancialReportDto>> GenerateFinancialReportAsync(ReportFilterDto filters, CancellationToken cancellationToken = default);
        Task<Result<ProfitLossReportDto>> GenerateProfitLossReportAsync(ReportFilterDto filters, CancellationToken cancellationToken = default);
        Task<Result<BalanceSheetReportDto>> GenerateBalanceSheetReportAsync(ReportFilterDto filters, CancellationToken cancellationToken = default);
        Task<Result<CashFlowReportDto>> GenerateCashFlowReportAsync(ReportFilterDto filters, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<T>>> GetPagedReportDataAsync<T>(IQueryable<T> query, ReportFilterDto filter, CancellationToken cancellationToken = default) where T : class;
    }
}