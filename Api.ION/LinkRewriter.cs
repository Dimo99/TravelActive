using Microsoft.AspNetCore.Mvc;

namespace Api.ION
{
    public class LinkRewriter
    {
        private readonly IUrlHelper urlHelper;

        public LinkRewriter(IUrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }

        public Link Rewrite(Link original)
        {
            if (original == null)
            {
                return null;
            }

            return new Link()
            {
                Href = urlHelper.Link(original.RouteName, original.RouteValues),
                Method = original.Method,
                Relations = original.Relations
            };
        }
    }
}