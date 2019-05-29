# Sitecore-Azure-Search-Suggestions
Adding Azure Search Suggestions capabilities to Sitecore

This branch contains the implementation based on SXA.  See other branch for non-SXA version.
This branch does not contain an additional (NuGet) reference to the Azure Search libraries, it makes the API calls directly, like the rest of the Sitecore implementation. 
Unfortunately many of the Sitecore classes/methods/properties could not be overridden as they are internal or private, so numerous files are copies of the decompiled sources.  These files are commented and can hopefully be removed if the product team makes the classes/methods/properties public/protected.

`Foundation.Search.config` contains the following settings:
* *AzureSearchSuggesterName*  name of the suggester which is created in Azure Search
* *AzureSearchSuggesterFields* fields on which to offer suggestions
* *AzureSearchSuggesterFuzzy* whether to use fuzzy searching
* *AzureSearchSuggesterHighlight* whether to highlight matches with HTML tag
* *AzureSearchSuggesterHighlightTag* tag to use to highlight matches 
See https://docs.microsoft.com/en-us/rest/api/searchservice/suggestions#request for more details

For a quick way to test using OOTB components, add the following to the top of your Search Box.cshtml
```@{ Model.JsonDataProperties = Model.JsonDataProperties.Replace("search/suggestions", "searchsuggestions/SuggestionsEx"); }```

You can also call the endpoint manually
https://your.habitathome.local/sxa/searchsuggestions/SuggestionsEx/?q=home&itemid={10083CA0-5093-4D67-99DB-2579ABB421D3}
