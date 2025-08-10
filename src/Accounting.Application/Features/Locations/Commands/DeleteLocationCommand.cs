using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Locations.Commands
{
    public class DeleteLocationCommand : ICommand<Result<bool>>
    {
        public int Id { get; set; }

        public DeleteLocationCommand(int id)
        {
            Id = id;
        }
    }
}