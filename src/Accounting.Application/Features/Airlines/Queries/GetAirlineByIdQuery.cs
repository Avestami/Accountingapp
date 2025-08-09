using MediatR;
using Accounting.Application.Common.Queries;
using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Airlines.Queries
{
    public class GetAirlineByIdQuery : IQuery<Result<AirlineDto>>
    {
        public int Id { get; set; }
        public string Company { get; set; } = string.Empty;
        
        public GetAirlineByIdQuery(int id)
        {
            Id = id;
        }
    }
}