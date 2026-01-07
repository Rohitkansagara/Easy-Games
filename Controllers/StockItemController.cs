using EasyGames.Class;
using EasyGames.Class.Dtos;
using EasyGames.Class.Enum;
using EasyGames.Class.NewFolder;
using EasyGames.Services.ExtensionMethod;
using EasyGames.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockItemController : ControllerBase
    {
        private readonly IStockItemService _stockItemService;
        private readonly IGenericFilterService _genericFilterService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<StockItemController> _logger;

        public StockItemController(IStockItemService service, ILogger<StockItemController> logger, IGenericFilterService genericFilterService,
            ApplicationDbContext dbContext
            )
        {
            _stockItemService = service;
            _logger = logger;
            _genericFilterService = genericFilterService;
            _dbContext = dbContext;

        }


        [HttpGet]
        [ProducesResponseType(typeof(List<StockItemDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all stock items");

                var result = await _stockItemService.GetAllAsync();
                return this.OkResponse(EnumEntityType.StockItem, EnumEntityEvents.COMMON_LIST, result);
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.COMMON_LIST_EXCEPTION, ex, _logger);
            }
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(StockItemDto), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching StockItem by Id: {Id}", id);

                var item = await _stockItemService.GetByIdAsync(id);

                if (item == null)
                {
                    _logger.LogWarning("StockItem not found for Id: {Id}", id);
                    return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.NOT_FOUND, "Item not found", _logger);
                }

                return this.OkResponse(EnumEntityType.StockItem, EnumEntityEvents.COMMON_GET, item);
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.COMMON_GET_EXCEPTION, ex, _logger);
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(StockItemDto), 200)]
        public async Task<IActionResult> Create([FromBody] StockItemDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new StockItem");

                var result = await _stockItemService.AddAsync(dto);

                return this.CreatedResponse(EnumEntityType.StockItem, EnumEntityEvents.COMMON_CREATE, result, $"api/StockItem/{result.Id}");
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.COMMON_CREATE_EXCEPTION, ex, _logger);
            }
        }

        [HttpPut("{Id}")]
        [ProducesResponseType(typeof(StockItemDto), 200)]
        public async Task<IActionResult> Update(long id, [FromBody] StockItemDto dto)
        {
            try
            {
                _logger.LogInformation("Updating StockItem Id: {Id}", id);
                
                var result = await _stockItemService.UpdateAsync(dto);

                if (result == null)
                {
                    _logger.LogWarning("Update failed - StockItem not found for Id: {Id}", id);
                    return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.NOT_FOUND, "Item not found", _logger);
                }

                return this.OkResponse(EnumEntityType.StockItem, EnumEntityEvents.COMMON_UPDATE, result);
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.COMMON_UPDATE_EXCEPTION, ex, _logger);
            }
        }


        [HttpDelete("{Id}")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting StockItem Id: {Id}", id);

                var deleted = await _stockItemService.DeleteAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Delete failed - StockItem not found for Id: {Id}", id);
                    return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.NOT_FOUND, "Item not found", _logger);
                }

                return this.OkResponse(EnumEntityType.StockItem, EnumEntityEvents.COMMON_DELETE, "Deleted successfully");
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.COMMON_DELETE_EXCEPTION, ex, _logger);
            }
        }


        [HttpGet("stock-items")]
        public async Task<IActionResult> GetStockItems(int pageNo = 1,int pageSize = 10,string? filter = null, string? orderBy = null)
        {
            var result = await _genericFilterService.GetFilteredDataAsync<StockItem, StockItemDto>(
                    _dbContext.StockItems.AsQueryable(),
                    pageNo,
                    pageSize,
                    filter,
                    orderBy,
                    x => new StockItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Quantity = x.Quantity,
                        AvailableQuantity = x.AvailableQuantity,
                        Price = x.Price
                    },
                    defaultFilters: new Dictionary<string, string>
                    {
                { "Disabled", "eq:false" } // auto-filter
                    }
                );

            return Ok(result);
        }

    }
}
