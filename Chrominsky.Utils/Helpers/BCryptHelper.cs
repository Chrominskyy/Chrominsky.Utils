namespace Chrominsky.Utils.Helpers;

/// <summary>
/// A helper class for working with BCrypt password hashing.
/// </summary>
public class BCryptHelper
{
    /// <summary>
    /// Hashes a password using BCrypt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifies a password against a hashed password using BCrypt.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to verify against.</param>
    /// <returns>True if the password matches the hashed password; otherwise, false.</returns>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}