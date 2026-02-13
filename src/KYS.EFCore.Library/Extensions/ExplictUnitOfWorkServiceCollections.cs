using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KYS.EFCore.Library.Extensions
{
    public static class ExplictUnitOfWorkServiceCollections
    {
        public static IServiceCollection AddExplicitUnitOfWork<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddScoped<IUnitOfWork>(provider =>
            {
                var context = provider.GetService<TDbContext>();
                return new DbContextUnitOfWork<TDbContext>(context, true);
            });

            return services;
        }
    }
}
