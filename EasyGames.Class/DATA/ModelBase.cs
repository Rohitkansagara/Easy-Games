using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Class.DATA
{
    public  interface IDocumentId
    {
        long Id { get; set; }
    }
    public class ModelBase:IDocumentId,IRowVersion
    {
        [Required]
        [Key]
        public long Id { get; set; }
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
    }
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
    public interface IRecordModifiedInfo
    {
        /// <summary>
        /// Record Last Modified On DateTime information.
        /// </summary>
        DateTimeOffset ModifiedOn { get; set; }

        /// <summary>
        /// Record last modified by UserId
        /// </summary>
        long? ModifiedById { get; set; }
    }
    public interface IRecordCreatedInfo
    {
        /// <summary>
        /// Record created DateTime information.
        /// </summary>
        DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Record created by UserId
        /// </summary>
        long? CreatedById { get; set; }
    }
    public interface IRowVersion
    {
        byte[] RowVersion { get; set; }
    }


}
