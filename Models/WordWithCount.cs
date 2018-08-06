using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace FactFlux.Models
{
    public class WordWithCount
    {
        public int WordId { get; set; }
        public string Word { get; set; }
        public bool Banned { get; set; }
        public int Count { get; set; }
        public string wordSlug { get; set; }
    }
}