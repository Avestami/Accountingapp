using MediatR;
using Accounting.Application.Common.Commands;
using Accounting.Application.Common.Models;

namespace Accounting.Application.Features.Airlines.Commands
{
    public class DeleteAirlineCommand : ICommand<Result<bool>>
    {
        public int Id { get; set; }
        public string Company { get; set; } = string.Empty;
        
        public DeleteAirlineCommand(int id)
        {
            Id = id;
        }
    }
}