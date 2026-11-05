using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KYS.EFCore.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for registering <see cref="IUnitOfWork"/> in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ExplicitUnitOfWorkServiceCollections
    {
        /// <summary>
        /// Registers <see cref="IUnitOfWork"/> as a scoped service implemented by <see cref="DbContextUnitOfWork{TDbContext}"/>,
        /// sharing the <typeparamref name="TDbContext"/> instance of the same scope.
        /// </summary>
        /// <typeparam name="TDbContext">The type of <see cref="DbContext"/>. It must be registered separately (e.g. with <c>AddDbContext</c>).</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance this method extends.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance, so that calls can be chained.</returns>
        public static IServiceCollection AddExplicitUnitOfWork<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddScoped<IUnitOfWork>(provider =>
            {
                var context = provider.GetService<TDbContext>();
                return new DbContextUnitOfWork<TDbContext>(context);
            });

            return services;
        }
    }
}
