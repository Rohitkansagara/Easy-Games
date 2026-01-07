using AutoMapper;
using EasyGames.Class;
using EasyGames.Class.NewFolder;
using EasyGames.Services.ExtensionMethod;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Services.Services
{
    public interface IStockItemService : IBaseService<StockItem>
    {

    }

    public class StockItemService : BaseService<StockItem>, IStockItemService
    {

        #region CTOR
        private readonly ILogger<StockItemService> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUserInfo _currentUserInfo;
        public StockItemService(ApplicationDbContext applicationDbContext,
            IMapper mapper,
            ICurrentUserInfo currentUserInfo,
            ILogger<StockItemService> logger
            ) : base(applicationDbContext, mapper,currentUserInfo)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _logger = logger;
            _currentUserInfo = currentUserInfo;
        }
        #endregion

    }
}
