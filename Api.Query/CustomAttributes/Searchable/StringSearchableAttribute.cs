using Api.Query.Search;

namespace Api.Query.CustomAttributes.Searchable
{
    public class StringSearchableAttribute : SearchableAttribute
    {
        public StringSearchableAttribute()
        {
            ExpressionProvider = new StringSearchExpressionProvider();
        }
    }
}