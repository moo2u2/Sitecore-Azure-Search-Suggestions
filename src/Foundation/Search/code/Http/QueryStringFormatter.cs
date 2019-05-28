using Newtonsoft.Json.Linq;
using Sitecore.HabitatHome.Foundation.Search.Models;

namespace Sitecore.HabitatHome.Foundation.Search.Http
{
    public class QueryStringFormatter : ContentSearch.Azure.Http.QueryStringFormatter
    {
        public virtual string RenderJsonBody(SuggestionRequest request)
        {
            JObject jObject = new JObject();
            if (!string.IsNullOrEmpty(request.Search))
            {
                jObject.Add(new JProperty("search", request.Search));
                jObject.Add(new JProperty("suggesterName", request.SuggesterName));
            }
            if (!string.IsNullOrEmpty(request.HighlightPreTag))
            {
                jObject.Add(new JProperty("highlightPreTag", request.HighlightPreTag));
            }
            if (!string.IsNullOrEmpty(request.HighlightPostTag))
            {
                jObject.Add(new JProperty("highlightPostTag", request.HighlightPostTag));
            }
            if (!string.IsNullOrEmpty(request.SearchFields))
            {
                jObject.Add(new JProperty("searchFields", request.SearchFields));
            }
            if (!string.IsNullOrEmpty(request.Filter))
            {
                jObject.Add(new JProperty("filter", request.Filter));
            }
            if (!string.IsNullOrEmpty(request.Select))
            {
                jObject.Add(new JProperty("select", request.Select));
            }
            if (request.Top >= 0)
            {
                jObject.Add(new JProperty("top", request.Top));
            }
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                jObject.Add(new JProperty("orderby", request.OrderBy));
            }
            jObject.Add(new JProperty("minimumCoverage", request.MinimumCoverage));
            jObject.Add(new JProperty("fuzzy", request.Fuzzy));
            return jObject.ToString();
        }

        public virtual string RenderJsonBody(AutocompleteRequest request)
        {
            JObject jObject = new JObject();
            if (!string.IsNullOrEmpty(request.Search))
            {
                jObject.Add(new JProperty("search", request.Search));
                jObject.Add(new JProperty("suggesterName", request.SuggesterName));
                jObject.Add(new JProperty("autocompleteMode", request.AutocompleteMode));
            }
            if (!string.IsNullOrEmpty(request.HighlightPreTag))
            {
                jObject.Add(new JProperty("highlightPreTag", request.HighlightPreTag));
            }
            if (!string.IsNullOrEmpty(request.HighlightPostTag))
            {
                jObject.Add(new JProperty("highlightPostTag", request.HighlightPostTag));
            }
            if (!string.IsNullOrEmpty(request.SearchFields))
            {
                jObject.Add(new JProperty("searchFields", request.SearchFields));
            }
            if (!string.IsNullOrEmpty(request.Filter))
            {
                jObject.Add(new JProperty("filter", request.Filter));
            }
            if (!string.IsNullOrEmpty(request.Select))
            {
                jObject.Add(new JProperty("select", request.Select));
            }
            if (request.Top >= 0)
            {
                jObject.Add(new JProperty("top", request.Top));
            }
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                jObject.Add(new JProperty("orderby", request.OrderBy));
            }
            jObject.Add(new JProperty("minimumCoverage", request.MinimumCoverage));
            jObject.Add(new JProperty("fuzzy", request.Fuzzy));
            return jObject.ToString();
        }
    }
}