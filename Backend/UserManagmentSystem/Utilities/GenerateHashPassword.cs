using System.Text;
using System.Security.Cryptography;


namespace UserManagmentSystem.Utilities
{
    public class GenerateHashPassword
    {
        public string CreateHashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedPasswordBytes).Replace("-", "").ToLower();
            }
        }

        public bool IsValidPassword(string hashedPassword, string Password)
        {
            return CreateHashPassword(Password) == hashedPassword;
        }
    }
}
