using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.ION;
using Api.Query.CustomAttributes.Direction;
using Api.Query.CustomAttributes.Searchable;
using Api.Query.CustomAttributes.Sortable;
using Api.Query.Search;
using Api.Query.Sort;

namespace Api.Query
{
    public class FormHelper
    {
        public static Form FromResource<T>(Link self)
        {
            var allProperties = typeof(T).GetTypeInfo().DeclaredProperties.ToArray();
            var sortableProperties = allProperties
                .Where(p => p.GetCustomAttributes<SortableAttribute>().Any()).ToArray();
            var searchableProperties = allProperties
                .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any()).ToArray();

            if (!sortableProperties.Any() && !searchableProperties.Any())
            {
                return new Form {Self = self};
            }

            var orderByOptions = new List<FormFieldOption>();
            foreach (var prop in sortableProperties)
            {
                var name = prop.Name.ToCamelCase();

                orderByOptions.Add(
                    new FormFieldOption {Label = $"Sort by {name}", Value = name});
                orderByOptions.Add(
                    new FormFieldOption {Label = $"Sort by {name} descending", Value = $"{name} desc"});
            }

            string searchPattern = null;
            if (searchableProperties.Any())
            {
                var applicableOperators = searchableProperties
                    .SelectMany(x => x
                        .GetCustomAttribute<SearchableAttribute>()
                        .ExpressionProvider.GetOperators())
                    .Distinct();

                var opGroup = $"{string.Join("|", applicableOperators)}";
                var nameGroup = $"{string.Join("|", searchableProperties.Select(x => x.Name.ToCamelCase()))}";

                searchPattern = $"/({nameGroup}) ({opGroup}) (.*)/i";
            }

            var formFields = new List<FormField>();
            if (orderByOptions.Any())
            {
                formFields.Add(new FormField
                {
                    Name = nameof(SortOptions<string, string>.OrderBy).ToCamelCase(),
                    Type = "set",
                    Options = orderByOptions.ToArray()
                });
            }

            if (!string.IsNullOrEmpty(searchPattern))
            {
                formFields.Add(new FormField
                {
                    Name = nameof(SearchOptions<string, string>.Search).ToCamelCase(),
                    Type = "set",
                    Pattern = searchPattern
                });
            }

            return new Form()
            {
                Self = self,
                Value = formFields.ToArray()
            };
        }

        public static Form DirectionsQuery<T>(Link self)
        {
            var locationsProperties = typeof(T).GetTypeInfo().DeclaredProperties
                .Where(p => p.GetCustomAttributes<LocationAttribute>().Any());
            if (!locationsProperties.Any())
            {
                return new Form()
                {
                    Self = self
                };
            }
            var formFields = new List<FormField>();
            foreach (var property in locationsProperties)
            {
                var name = property.Name.ToCamelCase();
                var label = property.GetCustomAttribute<LocationAttribute>().Label;
                formFields.Add(new FormField()
                {
                    Name = name,
                    Label = label,
                    Type = "string",
                    Pattern = "^[0-9]+\\.[0-9]+\\,[0-9]+\\.[0-9]+$",
                    Required = true
                });
            }
            return new Form()
            {
                Self = self,
                Value = formFields.ToArray()
            };
        }
    }
}