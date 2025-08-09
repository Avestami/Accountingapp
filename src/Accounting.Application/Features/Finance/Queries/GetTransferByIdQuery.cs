using MediatR;
using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Finance.Queries
{
    public class GetTransferByIdQuery : IQuery<Result<TransferDto>>
    {
        public int Id { get; set; }
        public string Company { get; set; } = string.Empty;
    }
}