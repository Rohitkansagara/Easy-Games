using EasyGames.Class.DATA;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Class.Dtos
{
    public class ModelBaseDto:IDocumentId,IRowVersion
    {
        public long Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public long? CreatedById { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
        public long? ModifiedById { get; set; }
        public bool Disabled { get; set; }
        public DateTimeOffset EnableDisabled { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
