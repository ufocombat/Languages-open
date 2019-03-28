using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Languages.Models
{
    public class Review
    {
        [Display(Name = "Review")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Required field")]
        public String Description = String.Empty;
    }
}
