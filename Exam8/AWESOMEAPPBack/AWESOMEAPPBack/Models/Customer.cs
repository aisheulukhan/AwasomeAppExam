using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AWESOMEAPPBack.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Founder { get; set; }
        public string Desc { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
