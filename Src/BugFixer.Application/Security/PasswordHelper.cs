namespace BugFixer.Application.Security
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // Use a hashing algorithm to hash the password
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        public static bool VerifyPassword(string hashedPassword, string password)
        {
            // Hash the input password and compare it with the stored hashed password
            var hashedInputPassword = HashPassword(password);
            return hashedInputPassword == hashedPassword;
        }
    }
}
