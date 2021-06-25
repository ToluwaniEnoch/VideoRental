using Api.Models.Enums;
using Api.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Payloads
{
    public class PriceCalculatorPayload
    {
        public string FirstName { get; set; }
        public string Title { get; init; }
        public int NumberOfDays { get; init; }        
    }
}
