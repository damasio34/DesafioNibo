using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DesafioNibo.Models
{
    public class LancamentoBacarioViewModel
    {
        [DataType(DataType.Upload)]
        public IEnumerable<HttpPostedFileBase> Arquivos { get; set; }
    }
}