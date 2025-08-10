using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Locations.Queries
{
    public class GetLocationByIdQuery
    {
        public int Id { get; set; }

        public GetLocationByIdQuery(int id)
        {
            Id = id;
        }
    }
}