using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Api.ION;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using TravelActive.Models.ViewModels;

namespace TravelActive.Filters
{
    public class LinkRewritingFilter : IAsyncResultFilter
    {
        private readonly IUrlHelperFactory urlHelperFactory;

        public LinkRewritingFilter(IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var objectResult = context.Result as ObjectResult;
            bool shouldSkip = objectResult?.Value == null || objectResult?.StatusCode != (int)HttpStatusCode.OK;
            if (shouldSkip)
            {
                await next();
                return;
            }

            
            var rewriter = new LinkRewriter(urlHelperFactory.GetUrlHelper(context));
            if (objectResult.Value is Collection<BusDirections[]>)
            {
                var allProperties = objectResult.Value
                    .GetType()
                    .GetProperties()
                    .Where(p => p.CanRead)
                    .ToArray();
                var linkProperties = allProperties
                    .Where(p => p.CanWrite && p.PropertyType == typeof(Link));
                LinkRewriteProperties(objectResult.Value, rewriter, allProperties, linkProperties);
                await next();
                return;
            }
            RewriteAllLinks(objectResult.Value, rewriter);
            await next();
        }

        private static void RewriteAllLinks(object model, LinkRewriter rewriter)
        {
            if (model == null)
            {
                return;
            }

            var allProperties = model
                .GetType()
                .GetProperties()
                .Where(p => p.CanRead)
                .ToArray();
            var linkProperties = allProperties
                .Where(p => p.CanWrite && p.PropertyType == typeof(Link));
            LinkRewriteProperties(model, rewriter, allProperties, linkProperties);

            var arrayProperties = allProperties.Where(p => p.PropertyType.IsArray);
            RewriteLinksInArray(arrayProperties, model, rewriter);
            var objectProperties = allProperties.Except(linkProperties).Except(arrayProperties).Where(p => p.PropertyType != typeof(BusDirections));
            RewriteLinksInNestedObject(objectProperties, model, rewriter);
        }

        private static void LinkRewriteProperties(object model, LinkRewriter rewriter, PropertyInfo[] allProperties, IEnumerable<PropertyInfo> linkProperties)
        {
            foreach (var linkProperty in linkProperties)
            {
                var rewritten = rewriter.Rewrite(linkProperty.GetValue(model) as Link);
                if (rewritten == null) continue;
                linkProperty.SetValue(model, rewritten);
                if (linkProperty.Name == nameof(Resource.Self))
                {
                    allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Href))
                        ?.SetValue(model, rewritten.Href);
                    allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Method))
                        ?.SetValue(model, rewritten.Method);
                    allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Relations))
                        ?.SetValue(model, rewritten.Relations);

                }
            }
        }

        private static void RewriteLinksInNestedObject(IEnumerable<PropertyInfo> objectProperties, object model, LinkRewriter rewriter)
        {
            foreach (var objectProperty in objectProperties)
            {
                if (objectProperty.PropertyType == typeof(string))
                {
                    continue;
                }

                var typeInfo = objectProperty.PropertyType.GetTypeInfo();
                if (typeInfo.IsClass)
                {
                    RewriteAllLinks(objectProperty.GetValue(model), rewriter);
                }
            }
        }

        private static void RewriteLinksInArray(IEnumerable<PropertyInfo> arrayProperties, object model, LinkRewriter rewriter)
        {
            foreach (var arrayProperty in arrayProperties)
            {
                var array = arrayProperty.GetValue(model) as Array ?? new Array[0];
                foreach (var element in array)
                {
                    RewriteAllLinks(element, rewriter);
                }
            }
        }
    }
}