using EasyGames.Class.Dtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Class.DATA
{
    public enum UserType
    {
        Admin = 0,
        User = 1,
        Manager = 2,
        Customer = 3,
        Supplier = 4,
        Globle = 5
    }

    public class User : IdentityUser<long>
    {
        
        public UserType UserType { get; set; } = UserType.Customer;

        #region common feild
        public DateTimeOffset CreatedOn { get; set; }= DateTime.UtcNow;
        public long? CreatedById { get; set; } 
        public DateTimeOffset ModifiedOn { get; set; }=DateTime.UtcNow;
        public long? ModifiedById { get; set; }
        public bool Disabled { get; set; } = false;
        public DateTimeOffset EnableDisabled { get; set; }

        [Timestamp]
        /// <summary>
        /// Record row version to support concurrency update for each record!
        /// </summary>
        public byte[] RowVersion { get; set; }
        #endregion
    }

    public class UserDto : ModelBaseDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }

        public UserType UserType { get; set; }

    }

    public class LoginRequest
    {
        public string? UserName { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string Role { get; set; }
    }
}
