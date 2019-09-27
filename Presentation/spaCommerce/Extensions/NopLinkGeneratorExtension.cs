using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using spaCommerce.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaCommerce.Extensions
{
    public static class NopLinkGeneratorExtension
    {
        //we have two pagers:
        //The first one can have custom routes
        //The second one just adds query string parameter
        //public static IHtmlContent Pager<TModel>(this LinkGenerator generator, PagerModel model)
        //{
        //    if (model.TotalRecords == 0)
        //        return new HtmlString("");

        //    var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        //    var links = new StringBuilder();
        //    if (model.ShowTotalSummary && (model.TotalPages > 0))
        //    {
        //        links.Append("<li class=\"total-summary\">");
        //        links.Append(string.Format(model.CurrentPageText, model.PageIndex + 1, model.TotalPages, model.TotalRecords));
        //        links.Append("</li>");
        //    }
        //    if (model.ShowPagerItems && (model.TotalPages > 1))
        //    {
        //        if (model.ShowFirst)
        //        {
        //            //first page
        //            if ((model.PageIndex >= 3) && (model.TotalPages > model.IndividualPagesDisplayedCount))
        //            {
        //                model.RouteValues.pageNumber = 1;

        //                links.Append("<li class=\"first-page\">");
        //                if (model.UseRouteLinks)
        //                {
        //                    var link = $"<a href=\"{generator.GetPathByRouteValues(model.RouteActionName, model.RouteValues) }\" title=\"{localizationService.GetResource("Pager.FirstPageTitle")}\">{model.FirstButtonText}</a>";
                            
        //                    var link1 = generator.RouteLink(model.FirstButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.FirstPageTitle") });
        //                    links.Append(link.ToHtmlString());
        //                }
        //                else
        //                {
        //                    var link = html.ActionLink(model.FirstButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.FirstPageTitle") });
        //                    links.Append(link.ToHtmlString());
        //                }
        //                links.Append("</li>");
        //            }
        //        }
        //        if (model.ShowPrevious)
        //        {
        //            //previous page
        //            if (model.PageIndex > 0)
        //            {
        //                model.RouteValues.pageNumber = (model.PageIndex);

        //                links.Append("<li class=\"previous-page\">");
        //                if (model.UseRouteLinks)
        //                {
        //                    var link = html.RouteLink(model.PreviousButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.PreviousPageTitle") });
        //                    links.Append(link.ToHtmlString());
        //                }
        //                else
        //                {
        //                    var link = html.ActionLink(model.PreviousButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.PreviousPageTitle") });
        //                    links.Append(link.ToHtmlString());
        //                }
        //                links.Append("</li>");
        //            }
        //        }
        //        if (model.ShowIndividualPages)
        //        {
        //            //individual pages
        //            var firstIndividualPageIndex = model.GetFirstIndividualPageIndex();
        //            var lastIndividualPageIndex = model.GetLastIndividualPageIndex();
        //            for (var i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
        //            {
        //                if (model.PageIndex == i)
        //                {
        //                    links.AppendFormat("<li class=\"current-page\"><span>{0}</span></li>", (i + 1));
        //                }
        //                else
        //                {
        //                    model.RouteValues.pageNumber = (i + 1);

        //                    links.Append("<li class=\"individual-page\">");
        //                    if (model.UseRouteLinks)
        //                    {
        //                        var link = html.RouteLink((i + 1).ToString(), model.RouteActionName, model.RouteValues, new { title = string.Format(localizationService.GetResource("Pager.PageLinkTitle"), (i + 1)) });
        //                        links.Append(link.ToHtmlString());
        //                    }
        //                    else
        //                    {
        //                        var link = html.ActionLink((i + 1).ToString(), model.RouteActionName, model.RouteValues, new { title = string.Format(localizationService.GetResource("Pager.PageLinkTitle"), (i + 1)) });
        //                        links.Append(link.ToHtmlString());
        //                    }
        //                    links.Append("</li>");
        //                }
        //            }
        //        }
        //        if (model.ShowNext)
        //        {
        //            //next page
        //            if ((model.PageIndex + 1) < model.TotalPages)
        //            {
        //                model.RouteValues.pageNumber = (model.PageIndex + 2);

        //                links.Append("<li class=\"next-page\">");
        //                if (model.UseRouteLinks)
        //                {
        //                    var link = html.RouteLink(model.NextButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.NextPageTitle") });
        //                    links.Append(link.ToHtmlString());
        //                }
        //                else
        //                {
        //                    var link = html.ActionLink(model.NextButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.NextPageTitle") });
        //                    links.Append(link.ToHtmlString());
        //                }
        //                links.Append("</li>");
        //            }
        //        }
        //        if (model.ShowLast)
        //        {
        //            //last page
        //            if (((model.PageIndex + 3) < model.TotalPages) && (model.TotalPages > model.IndividualPagesDisplayedCount))
        //            {
        //                model.RouteValues.pageNumber = model.TotalPages;

        //                links.Append("<li class=\"last-page\">");
        //                if (model.UseRouteLinks)
        //                {
        //                    var link = html.RouteLink(model.LastButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.LastPageTitle") });
        //                    links.Append(link.ToHtmlString());
        //                }
        //                else
        //                {
        //                    var link = html.ActionLink(model.LastButtonText, model.RouteActionName, model.RouteValues, new { title = localizationService.GetResource("Pager.LastPageTitle") });
        //                    links.Append(link.ToHtmlString());
        //                }
        //                links.Append("</li>");
        //            }
        //        }
        //    }
        //    var result = links.ToString();
        //    if (!string.IsNullOrEmpty(result))
        //    {
        //        result = "<ul>" + result + "</ul>";
        //    }
        //    return new HtmlString(result);
        //}

    }
}
