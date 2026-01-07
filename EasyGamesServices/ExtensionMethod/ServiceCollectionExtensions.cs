using EasyGames.Class;
using EasyGames.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Services.ExtensionMethod
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register all app-level services here
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<IStockItemService,StockItemService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserInfo, CurrentUserInfo>();
            services.AddScoped<IGenericFilterService, GenericFilterService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
