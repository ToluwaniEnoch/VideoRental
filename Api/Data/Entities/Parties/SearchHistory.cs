using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data.Entities.Parties
{
    public class SearchHistory : BaseEntity
    {
        public string Title { get; set; }
        public Decimal Price { get; set; }
        public string UserFirstName { get; set; }
        public int NumberOfDays { get; set; }
    }
}
