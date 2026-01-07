using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Class
{
    public enum RoleType
    {
        None = 0,
        Admin = 1,
        Manager = 2,
        Employee = 3,
        Customer = 4
    }
    public class Role : IdentityRole<long>
    {
        public string Description { get; set; }
        public RoleType RoleType { get; set; } = RoleType.None;
        #region common feild
        public DateTimeOffset CreatedOn { get; set; }
        public long? CreatedById { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
        public long? ModifiedById { get; set; }
        public bool Disabled { get; set; }
        public DateTimeOffset EnableDisabled { get; set; }

        [Timestamp]
        /// <summary>
        /// Record row version to support concurrency update for each record!
        /// </summary>
        public byte[] RowVersion { get; set; }
        #endregion
    }
}
