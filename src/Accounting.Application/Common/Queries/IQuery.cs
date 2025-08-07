using MediatR;

namespace Accounting.Application.Common.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}