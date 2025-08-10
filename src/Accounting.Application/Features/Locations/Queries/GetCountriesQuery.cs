using Accounting.Application.Common.Models;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Locations.Queries
{
    public class GetCountriesQuery
    {
        public bool? IsActive { get; set; }
    }
}