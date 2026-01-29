using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Pozoriste.Web.Areas.Identity.Pages.Account.Manage;

namespace Pozoriste.Tests;

public class HelperMethodTests
{
    [Fact]
    public void PageNavClass_ReturnsActive_WhenPageMatches()
    {
        var viewContext = new ViewContext
        {
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        };
        viewContext.ViewData["ActivePage"] = "Email";

        var result = ManageNavPages.PageNavClass(viewContext, "Email");

        Assert.Equal("active", result);
    }
}
