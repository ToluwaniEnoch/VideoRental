using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Validation
{
    public class NewReleaseAttribute : RangeAttribute
    {
        private static readonly int minimum = DateTime.Now.Year;
        private static readonly double maximum = DateTime.Now.AddYears(20).Year;

        public NewReleaseAttribute() : base(minimum, maximum)
        {
        }

    }
}
