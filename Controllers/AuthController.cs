using EasyGames.Class.DATA;
using EasyGames.Class.Dtos;
using EasyGames.Class.Enum;
using EasyGames.Services.ExtensionMethod;
using EasyGames.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EasyGames.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(IAuthService authService,
            ILogger<AuthController> logger
            )
        {
            _authService = authService;
            _logger = logger;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginRequest user)
        {
            try
            {

                this._logger.LogInformation("Register Api.");
             
                var response = await _authService.RegisterAsync(user);
                return this.OkResponse(EnumEntityType.User, EnumEntityEvents.COMMON_LIST, response);


            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.User, EnumEntityEvents.COMMON_LIST_EXCEPTION, ex, _logger);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                this._logger.LogInformation("login Api.");
                var response = await _authService.LoginAsync(request);
                return this.OkResponse(EnumEntityType.User, EnumEntityEvents.COMMON_LIST, response);


            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.User, EnumEntityEvents.COMMON_LIST_EXCEPTION, ex, _logger);
            }
        }

        [Authorize]
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(List<User>), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all stock items");

                var result = await _authService.GetAllAsync();
                return this.OkResponse(EnumEntityType.User, EnumEntityEvents.COMMON_LIST, result);
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.User, EnumEntityEvents.COMMON_LIST_EXCEPTION, ex, _logger);
            }
        }

        [Authorize]
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching USER by Id: {Id}", id);

                var item = await _authService.GetByIdAsync(id);

                if (item == null)
                {
                    _logger.LogWarning("USER not found for Id: {Id}", id);
                    return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.NOT_FOUND, "Item not found", _logger);
                }

                return this.OkResponse(EnumEntityType.User, EnumEntityEvents.COMMON_GET, item);
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.User, EnumEntityEvents.COMMON_GET_EXCEPTION, ex, _logger);
            }
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 200)]
        public async Task<IActionResult> Create([FromBody] UserDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new User");

                var result = await _authService.AddAsync(dto);

                return this.CreatedResponse(EnumEntityType.User, EnumEntityEvents.COMMON_CREATE, result, $"api/StockItem/{result.Id}");
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.User, EnumEntityEvents.COMMON_CREATE_EXCEPTION, ex, _logger);
            }
        }

        [Authorize]
        [HttpPut("{Id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        public async Task<IActionResult> Update(long id, [FromBody] UserDto dto)
        {
            try
            {
                _logger.LogInformation("Updating StockItem Id: {Id}", id);

                var result = await _authService.UpdateAsync(dto);

                if (result == null)
                {
                    _logger.LogWarning("Update failed - StockItem not found for Id: {Id}", id);
                    return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.NOT_FOUND, "Item not found", _logger);
                }

                return this.OkResponse(EnumEntityType.User, EnumEntityEvents.COMMON_UPDATE, result);
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.User, EnumEntityEvents.COMMON_UPDATE_EXCEPTION, ex, _logger);
            }
        }

        [Authorize]
        [HttpDelete("{Id}")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting StockItem Id: {Id}", id);

                var deleted = await _authService.DeleteAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Delete failed - StockItem not found for Id: {Id}", id);
                    return this.CreateBadRequest(EnumEntityType.StockItem, EnumEntityEvents.NOT_FOUND, "Item not found", _logger);
                }

                return this.OkResponse(EnumEntityType.User, EnumEntityEvents.COMMON_DELETE, "Deleted successfully");
            }
            catch (Exception ex)
            {
                return this.CreateBadRequest(EnumEntityType.User, EnumEntityEvents.COMMON_DELETE_EXCEPTION, ex, _logger);
            }
        }
    }


}
