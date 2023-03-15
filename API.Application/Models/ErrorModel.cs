using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Application.Models
{
    public record ErrorModel
    {
        public string ErrorMsg { get; set; }
        public string Time { get; set; }
    }
}
