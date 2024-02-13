using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AccountServices.Utilities
{
    public class GeneralUtilities
    {
        public static bool ComparePassword(string password, byte[] hashPass)
        {
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            SHA512 hash = SHA512.Create();
            var hashedBytes = hash.ComputeHash(bytes);
            return Convert.ToBase64String(hashPass) == Convert.ToBase64String(hashedBytes);
        }

        public static byte[] ValidatePassword(string password)
        {
            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            SHA512 hash = SHA512.Create();
            var hashedBytes = hash.ComputeHash(bytes);

            var forbiddenPwd = Regex.IsMatch(password, @"(((A|a)(D|d)(M|m)(I|i)(N|n))|((P|p)(A|a)(S|s)(S|s)(W|w)(O|o)(R|r)(D|d))|((T|t)(H|h)(G|g)))([123]|5420)");
            var hasNumber = Regex.IsMatch(password, @"[0-9]");
            var hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            var hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            var hasSpecialCharacter = password.Any(c => !char.IsLetterOrDigit(c));
            var noWhiteSpace = password.Any(c => !char.IsWhiteSpace(c));
            if (!noWhiteSpace || password.Length < 8 || forbiddenPwd)
            {
               return null;
            }
            else if (!hasNumber)
            {
                return hasLowerCase && hasUpperCase && hasSpecialCharacter ? hashedBytes : null;
            }
            else if (!hasLowerCase)
            {
                return hasNumber && hasUpperCase && hasSpecialCharacter ? hashedBytes : null;
            }
            else if (!hasUpperCase)
            {
                return hasLowerCase && hasNumber && hasSpecialCharacter ? hashedBytes : null;
            }
            else if (!hasSpecialCharacter)
            {
                return hasLowerCase && hasUpperCase && hasNumber ? hashedBytes : null;
            }
            else
            {
                return hashedBytes;
            }
        }
    }
}
