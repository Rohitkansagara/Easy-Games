using AutoMapper;
using EasyGames.Class.DATA;
using EasyGames.Class.Dtos;
using EasyGames.Class.NewFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Services.Services.AutoMapper
{
    public static class ConfigureAutomapper
    {
        public static void ConfigureDBModels(IMapperConfigurationExpression cfg) //define mapping rules
        {
            cfg.CreateMap<User, UserDto>();

            cfg.CreateMap<StockItem, StockItemDto>();
            cfg.CreateMap<StockItemDto, StockItem>();
        }
    }
}

