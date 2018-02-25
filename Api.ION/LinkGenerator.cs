namespace Api.ION
{
    public class LinkGenerator
    {
        public const string GetMethod = "GET";
        public const string PostMethod = "POST";
        public static Link To(string routeName, object routeValues = null)
        {
            return new Link()
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = null
            };
        }
        public static Link ToForm(
            string routeName,
            object routeValues = null,
            string method = PostMethod,
            params string[] relations)
            => new Link()
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = method,
                Relations = relations
            };
        public static Link ToCollection(string routeName, object routeValues = null)
        {
            return new Link()
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = new string[] { "collection" }
            };
        }
    }
}