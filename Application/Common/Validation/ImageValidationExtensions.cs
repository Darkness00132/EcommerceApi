using FluentValidation;

namespace Application.Common.Validation;

public static class ImageValidationExtensions
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public static IRuleBuilderOptions<T, FileDto?> ValidImage<T>(
        this IRuleBuilder<T, FileDto?> ruleBuilder)
    {
        return ruleBuilder
            .Must(BeAValidImageExtension)
            .WithMessage($"Image must be a valid format ({string.Join(", ", AllowedExtensions)}).")
            .Must(BeWithinAllowedSize)
            .WithMessage("Image size must not exceed 5MB.");
    }

    private static bool BeAValidImageExtension(FileDto? file)
    {
        // Null means "no file provided", so format check is skipped/valid
        if (file is null) return true;

        if (string.IsNullOrWhiteSpace(file.FileName)) return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private static bool BeWithinAllowedSize(FileDto? file)
    {
        if (file is null) return true;

        return !file.Content.CanSeek || file.Content.Length <= MaxFileSizeBytes;
    }
}