using System.Collections.Generic;
using Accounting.Application.Common.Models;
using Accounting.Application.Common.Queries;
using Accounting.Application.DTOs;

namespace Accounting.Application.Features.Locations.Queries
{
    public class GetCitiesByCountryQuery : IQuery<Result<List<LocationDto>>>
    {
        public int CountryId { get; set; }
        public bool? IsActive { get; set; }

        public GetCitiesByCountryQuery(int countryId)
        {
            CountryId = countryId;
        }
    }
}