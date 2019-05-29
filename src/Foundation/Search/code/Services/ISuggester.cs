using System.Collections.Generic;
using Sitecore.HabitatHome.Foundation.Search.Models;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    public interface ISuggester
    {
        IEnumerable<Suggestion> GetSuggestions(SuggesterModel model, out Timer queryTimer, out string indexName);

        IEnumerable<Suggestion> GetAutocomplete(SuggesterModel model, out Timer queryTimer, out string indexName);
    }
}
