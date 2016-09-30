using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DesafioNibo.Models
{
    public class LancamentoBacarioViewModel
    {
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Arquivo { get; set; }
    }
}