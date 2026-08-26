namespace Atrio.Application.Common;

public class AppValidationException(IDictionary<string, string[]> errors) : Exception("One or more validation errors occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;

    public static AppValidationException Single(string field, string message) =>
        new(new Dictionary<string, string[]> { [field] = [message] });
}
