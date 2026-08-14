using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Validation
{
    internal static class HasAtLeastOneValueValidationExtensions
    {
        public static IRuleBuilderOptions<T, T> HasAtLeastOneValue<T>(
        this IRuleBuilder<T, T> ruleBuilder,
        params string[] excludedProperties)
        {
            return ruleBuilder.Must(model =>
            {
                var excluded = excludedProperties.ToHashSet();

                return typeof(T)
                    .GetProperties()
                    .Where(p => !excluded.Contains(p.Name))
                    .Any(p => p.GetValue(model) is not null);
            });
        }
    }
}
