namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    public class SuggestionRequest
    {
        public string Link { get; set; }

        public string SuggesterName { get; set; }

        public string Search { get; set; }

        public string Filter { get; set; }

        public string Select { get; set; }

        public string OrderBy { get; set; }

        public string HighlightPreTag { get; set; }

        public string HighlightPostTag { get; set; }

        public string SearchFields { get; set; }

        public bool Fuzzy { get; set; } = false;

        public int Top { get; set; } = -1;

        public int MinimumCoverage { get; set; } = 80;
    }
}