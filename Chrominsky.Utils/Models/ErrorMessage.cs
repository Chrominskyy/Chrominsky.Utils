using System.Text.Json.Serialization;

namespace Chrominsky.Utils.Models;

/// <summary>
/// Represents an error message with key-value pairs.
/// </summary>
public class ErrorMessage
{
    /// <summary>
    /// A dictionary containing error messages with their respective keys.
    /// </summary>
    [JsonPropertyName("messages")]
    public Dictionary<string, string> Messages { get; set; } = new();

    /// <summary>
    /// Adds a new error message to the Messages dictionary.
    /// </summary>
    /// <param name="key">The key for the error message.</param>
    /// <param name="message">The error message.</param>
    public void AddError(string key, string message)
    {
        Messages.Add(key, message);
    }

    public bool HasErrors() => Messages.Count > 0;
}