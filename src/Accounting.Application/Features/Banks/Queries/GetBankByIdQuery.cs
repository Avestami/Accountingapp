using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Banks.Queries
{
    public class GetBankByIdQuery
    {
        public int Id { get; set; }

        public GetBankByIdQuery(int id)
        {
            Id = id;
        }
    }
}