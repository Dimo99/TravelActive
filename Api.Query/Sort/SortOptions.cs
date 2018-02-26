using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Api.Query.Sort
{
    public class SortOptions<T,TEntity> : IValidatableObject
    {
        public string[] OrderBy { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var processor = new SortOptionsProcessor<T, TEntity>(OrderBy);
            var validTerms = processor.GetValidTerms().Select(x => x.Name);
            var indalidTerms = processor.GetAllTerms().Select(x => x.Name)
                .Except(validTerms, StringComparer.OrdinalIgnoreCase);
            foreach (var term in indalidTerms)
            {
                yield return new ValidationResult($"Invalid sort term '{term}'.",new[] {nameof(OrderBy)});
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var processor = new SortOptionsProcessor<T,TEntity>(OrderBy);
            return processor.Apply(query);
        }
    }
}