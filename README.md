_As Azure Search has been deprecated / un-supported for some time now, this repo is being archived._
---

# Sitecore-Azure-Search-Suggestions
Adding [Azure Search Suggestions](https://docs.microsoft.com/en-us/azure/search/index-add-suggesters) capabilities to Sitecore

This branch contains the implementation based on SXA.  See other branch for non-SXA version.
Unfortunately many of the Sitecore classes/methods/properties could not be overridden as they are internal or private, so numerous files are copies of the decompiled sources.  These files are commented and can hopefully be removed if the product team makes the classes/methods/properties public/protected.

Also requires the default Microsoft.Spatial assembly version to be updated in your Web.config
```xml
<dependentAssembly>
    <assemblyIdentity name="Microsoft.Spatial" publicKeyToken="31bf3856ad364e35" culture="neutral" xmlns="urn:schemas-microsoft-com:asm.v1" />
    <bindingRedirect oldVersion="0.0.0.0-7.5.3.21218" newVersion="7.5.3.21218" xmlns="urn:schemas-microsoft-com:asm.v1" />
</dependentAssembly>
```

`Foundation.Search.config` contains the following settings:
- *AzureSearchSuggesterName*  name of the suggester which is created in Azure Search
- *AzureSearchSuggesterFields* fields on which to offer suggestions
- *AzureSearchSuggesterFuzzy* whether to use fuzzy searching
- *AzureSearchSuggesterHighlight* whether to highlight matches with HTML tag
- *AzureSearchSuggesterHighlightTag* tag to use to highlight matches 

See https://docs.microsoft.com/en-us/rest/api/searchservice/suggestions#request for more details

For a quick way to test using OOTB components, add the following to the top of your Search Box.cshtml

```
@{ Model.JsonDataProperties = Model.JsonDataProperties.Replace("search/suggestions", "searchsuggestions/SuggestionsEx"); }
```

You can also call the endpoint manually

https://your.habitathome.local/sxa/searchsuggestions/SuggestionsEx/?q=home&itemid={10083CA0-5093-4D67-99DB-2579ABB421D3}
