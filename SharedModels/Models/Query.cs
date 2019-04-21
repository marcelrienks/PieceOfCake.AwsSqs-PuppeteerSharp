using System;
using static SharedModels.Models.Enums;

namespace SharedModels.Models
{
    public class Query
    {
        public Guid Id { get; set; }
        public Sites Site { get; set; }
    }
}
