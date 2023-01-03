using Api.Domain.Interfaces;
using Api.Infrastructure.Data;
using Api.Utils.Helpers;
using System.Security.Claims;

namespace Api.Utils.Extensions;

public static class DataContextExtensions
{
    public static IQueryable<TEntity> BelongsToUser<TEntity>(this DataContext context, ClaimsPrincipal user) where TEntity : class, IEntity<int>, IHasUser<string>
    {
        return context.Set<TEntity>()
            .Where(x => x.UserId == JwtUtils.GetUserId(user));
    }
}
