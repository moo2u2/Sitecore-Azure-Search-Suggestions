using System.Collections.Generic;

namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    public class Suggester
    {
        public string Name { get; set; }

        public string SearchMode { get; set; }

        public List<string> SourceFields { get; set; }

        public Suggester()
        {
            SourceFields = new List<string>();
        }
    }
}