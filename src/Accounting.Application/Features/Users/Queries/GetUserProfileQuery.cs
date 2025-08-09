using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Application.DTOs;
using Accounting.Application.Interfaces;
using Accounting.Application.Common.Queries;

namespace Accounting.Application.Features.Users.Queries
{
    public class GetUserProfileQuery : IQuery<UserProfileDto>
    {
        public int UserId { get; set; }
    }

    public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IAccountingDbContext _context;

        public GetUserProfileQueryHandler(IAccountingDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileDto> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(query.UserId);
            
            if (user == null)
                return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicture = user.ProfilePicture
            };
        }
    }
}