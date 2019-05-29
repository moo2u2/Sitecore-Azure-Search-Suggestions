# Sitecore-Azure-Search-Suggestions
Adding [Azure Search Suggestions](https://docs.microsoft.com/en-us/azure/search/index-add-suggesters) capabilities to Sitecore

This branch contains the implementation that does *not* depend on SXA.  See other branch for slightly simpler SXA version.
Unfortunately many of the Sitecore classes/methods/properties could not be overridden as they are internal or private, so numerous files are copies of the decompiled sources.  These files are commented and can hopefully be removed if the product team makes the classes/methods/properties public/protected.

`Foundation.Search.config` contains the following settings:
- *AzureSearchSuggesterName*  name of the suggester which is created in Azure Search
- *AzureSearchSuggesterFields* fields on which to offer suggestions
- *AzureSearchSuggesterFuzzy* whether to use fuzzy searching
- *AzureSearchSuggesterHighlight* whether to highlight matches with HTML tag
- *AzureSearchSuggesterHighlightTag* tag to use to highlight matches 

See https://docs.microsoft.com/en-us/rest/api/searchservice/suggestions#request for more details

For a quick way to test using OOTB components, add the following to the top of your Search Box.cshtml

You can call the endpoints manually

https://your.habitathome.local/api/sitecore/Search/Suggestions?query=hom&itemID={10083CA0-5093-4D67-99DB-2579ABB421D3}
https://your.habitathome.local/api/sitecore/Search/Autocomplete?query=hom&itemID={10083CA0-5093-4D67-99DB-2579ABB421D3}
