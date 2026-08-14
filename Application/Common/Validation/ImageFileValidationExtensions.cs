using Application.Common.Files;
using FluentValidation;

namespace Application.Common.Validation;

public static class ImageFileValidationExtensions
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    ];

    public static IRuleBuilderOptions<T, FileDto?> ValidImageFile<T>(
        this IRuleBuilder<T, FileDto?> ruleBuilder,
        long maxSizeInBytes)
    {
        return ruleBuilder
            .NotNull()
            .WithMessage("Image file is required.")
            .Must(file => file!.Length <= maxSizeInBytes)
            .WithMessage($"Image file cannot exceed {maxSizeInBytes / 1024 / 1024} MB.")
            .Must(file =>
                AllowedContentTypes.Contains(
                    file!.ContentType,
                    StringComparer.OrdinalIgnoreCase))
            .WithMessage("Only JPG, PNG And WEBP images are allowed.");
    }
}
