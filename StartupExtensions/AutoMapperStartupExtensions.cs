using AutoMapper;
using EasyGames.Services.Services.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace EasyGames.StartupExtensions
{
    public static class AutoMapperStartupExtensions
    {
        public static void AddAutoMapperServices(this IServiceCollection services)
        {
            // Build AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                ConfigureAutomapper.ConfigureDBModels(cfg);//You are creating the AutoMapper configuration root object.,It stores all your mapping rules.
            });

            // Create mapper instance
            var mapper = config.CreateMapper();//actual IMapper instance

            // Register into DI
            services.AddSingleton(config);//DI stores mapping rules
            services.AddSingleton(mapper);//DI shares working mapper
        }
    }
}


