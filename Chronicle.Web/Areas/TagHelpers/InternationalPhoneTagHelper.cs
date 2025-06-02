using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Chronicle.Web.Areas.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "asp-international-phone")]
    public class InternationalPhoneTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("pattern", @"^\+?[1-9]\d{1,14}$");
            output.Attributes.SetAttribute("title", "Enter a valid international phone number (e.g., +1234567890)");
        }
    }
}
