using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace FactFlux.Models
{

    public class WordApiWordInfo
    {
        public int WordId { get; set; }
        public string Word { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public int? DailyCount { get; set; }
        public int? WeeklyCount { get; set; }
        public int? MonthlyCount { get; set; }
        public int? YearlyCount { get; set; }
        public string Color { get; set; }
    }
}