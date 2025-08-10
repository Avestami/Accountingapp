using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Banks.Commands
{
    public class DeleteBankCommand
    {
        public int Id { get; set; }

        public DeleteBankCommand(int id)
        {
            Id = id;
        }
    }
}