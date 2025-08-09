using MediatR;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Finance.Commands
{
    public class DeleteIncomeCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string Company { get; set; } = string.Empty;
    }
}