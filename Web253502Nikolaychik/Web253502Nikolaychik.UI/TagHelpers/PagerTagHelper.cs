using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security.Policy;
namespace Web253502Nikolaychik.UI.TagHelpers
{
    [HtmlTargetElement("Pager", Attributes = "current-page, total-pages")]
    public class PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) : TagHelper
    {
        private readonly LinkGenerator _linkGenerator = linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        [HtmlAttributeName("current-page")]
        public int CurrentPage { get; set; }

        [HtmlAttributeName("total-pages")]
        public int TotalPages { get; set; }

        [HtmlAttributeName("category")]
        public string Category { get; set; }

        [HtmlAttributeName("admin")]
        public bool IsAdmin { get; set; } = false;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var nav = new TagBuilder("nav");
            nav.Attributes.Add("aria-label", "Page navigation example");
            nav.AddCssClass("mt-4");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            if (CurrentPage == 1)
            {
                li.AddCssClass("disabled");
            }
            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            
            if (IsAdmin)
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var link = _linkGenerator.GetPathByPage(
                    page: "/Product/Index",
                    values: new { area = "admin", pageNo = CurrentPage - 1 },
                    httpContext: httpContext
                );
                a.Attributes.Add("href", link);
            }
            else
            {
                a.Attributes.Add("href", _linkGenerator.GetPathByAction("Index", "Product", 
                                                                        new { category = Category, pageNo = CurrentPage - 1}));
            }
            a.InnerHtml.AppendHtml("Previous");
            li.InnerHtml.AppendHtml(a);
            ul.InnerHtml.AppendHtml(li);
            for (int i = 1; i <= TotalPages; i++)
            {
                li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (i == CurrentPage)
                {
                    li.AddCssClass("active");
                }
                a = new TagBuilder("a");
                a.AddCssClass("page-link");
                if (IsAdmin)
                {
                    var httpContext = _httpContextAccessor.HttpContext;

                    var link = _linkGenerator.GetPathByPage(
                        page: "/Product/Index",  
                        values: new { area = "admin", pageNo = i }, 
                        httpContext: httpContext  
                    );
                    a.Attributes.Add("href", link);
                }
                else
                {
                    a.Attributes.Add("href", _linkGenerator.GetPathByAction("Index", "Product", new { category = Category, pageNo = i }));
                }
                a.InnerHtml.AppendHtml(i.ToString());
                li.InnerHtml.AppendHtml(a);
                ul.InnerHtml.AppendHtml(li);
            }
            li = new TagBuilder("li");
            li.AddCssClass("page-item");
            if (CurrentPage == TotalPages)
            {
                li.AddCssClass("disabled");
            }
            a = new TagBuilder("a");
            a.AddCssClass("page-link");
            if (IsAdmin)
            {
                var httpContext = _httpContextAccessor.HttpContext;

                var link = _linkGenerator.GetPathByPage(
                    page: "/Product/Index",
                    values: new { area = "admin", pageNo = CurrentPage + 1 },
                    httpContext: httpContext
                );
                a.Attributes.Add("href", link);
            }
            else
            {
                a.Attributes.Add("href", _linkGenerator.GetPathByAction("Index", "Product", new { category = Category, pageNo = CurrentPage + 1 }));
            }
            a.InnerHtml.AppendHtml("Next");
            li.InnerHtml.AppendHtml(a);
            ul.InnerHtml.AppendHtml(li);
            nav.InnerHtml.AppendHtml(ul);
            output.Content.AppendHtml(nav);
        }
    }
}
