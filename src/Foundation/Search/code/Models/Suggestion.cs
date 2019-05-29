namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    public class Suggestion
    {
        public string Term { get; set; }

        public string Payload { get; set; }

        public string Html => Term;
    }
}