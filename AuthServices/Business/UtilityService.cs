using AccountServices.Business.Interfaces;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AccountServices.Business
{
    public class UtilityService : IUtilityService
    {
        public byte[] GeneratePassword()
        {
            int length = 15;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
            var random = new Random();
            string password = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            var hashPassword = ValidatePassword(password);
            while (hashPassword == null)
            {
                password = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
                hashPassword = ValidatePassword(password);
            }
            return hashPassword;
        }

        public bool ComparePassword(string password, byte[] hashPass)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            SHA512 hash = SHA512.Create();
            var hashedBytes = hash.ComputeHash(bytes);
            return Convert.ToBase64String(hashPass) == Convert.ToBase64String(hashedBytes);
        }

        public byte[] ValidatePassword(string password)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
            SHA512 hash = SHA512.Create();
            var hashedBytes = hash.ComputeHash(bytes);

            var forbiddenPwd = Regex.IsMatch(password, @"(((A|a)(D|d)(M|m)(I|i)(N|n))|((P|p)(A|a)(S|s)(S|s)(W|w)(O|o)(R|r)(D|d))|)([12345])");
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
