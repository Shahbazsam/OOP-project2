
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace MyMenu
{
    public interface LogicService
    {
        (bool, string) Register(string name, string email, string password, string securityCode, string birthDate);
        (bool, string) Login(string email, string password, string securityCode);
        void CreateWallet(string currency, uint startAmount);
        (bool, string) DeleteWallet(int walletIndex);
        User? GetActiveUser();
    }

    public class BusinessLogicService : LogicService
    {
        private Storage Storage { get; }

        public BusinessLogicService()
        {
            Storage = Storage.GetInstance();
        }
        public User? GetActiveUser()
        {
            return Storage.GetActiveUser();
        }

        public (bool, string) Register(string name, string email, string password, string securityCode, string birthDate)
        {
            try
            {
                var mail = new MailAddress(email);
            }
            catch (FormatException)
            {
                return (false, " incorrect Email ");
            }

            var passwordScore = PasswordCheck.CheckStrength(password);
            if (passwordScore < PasswordScore.Medium)
            {
                return (false, " weak Password ");
            }

            if (!DateTime.TryParseExact(birthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bithDateParsed))
            {
                return (false, "DateTime has incorrect format!");
            }

            // Hash the password before storing
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            try
            {
                Storage.AddUser(new User
                {
                    Name = name,
                    BirthDate = bithDateParsed,
                    Email = email,
                    Password = hashedPassword,  // Store the hashed password
                    SecurityCode = securityCode
                });
            }
            catch (ArgumentException)
            {
                return (false, "user already exist.!");
            }

            // Login
            Storage.SetActiveUser(email);

            return (true, "Successfull registration!\n enter to logg in ");
        }

        public (bool, string) Login(string email, string password, string securityCode)
        {
            try
            {
                var user = Storage.FindUserByEmail(email);
                // Verify the hashed password
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    // Check if the provided security code matches the user's security code
                    if (user.SecurityCode == securityCode)
                    {
                        Storage.SetActiveUser(email);
                        return (true, $"Hello, {user.Name}! Successful login!");
                    }
                    else
                    {
                        return (false, " incorrect code!");
                    }
                }

                return (false, " incorrect Password!");
            }
            catch (KeyNotFoundException)
            {
                return (false, "Invalid email!");
            }
        }

        public void CreateWallet(string currency, uint startAmount)
        {
            // Get the active user
            var activeUser = Storage.GetActiveUser();
            if (activeUser != null)
            {
                // Create a wallet for the active user with the provided currency and start amount
                var wallet = new Wallet { Currency = currency, StartAmount = startAmount };
                activeUser.Wallets.Add(wallet);
            }
        }
        public (bool, string) DeleteWallet(int walletIndex)
        {
            var activeUser = Storage.GetActiveUser();
            if (activeUser != null && walletIndex >= 0 && walletIndex < activeUser.Wallets.Count)
            {
                activeUser.Wallets.RemoveAt(walletIndex);
                return (true, "Wallet deleted successfully!");
            }

            return (false, "Invalid wallet index or no wallets available for deletion.");
        }
    }

    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }

    public class PasswordCheck  
    {
        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 5)
                score++;
            if (password.Length >= 7)
                score++;
            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if (validateGuidRegex.IsMatch(password))
            {
                score += 3;
            }

            return (PasswordScore)score;
        }
    }
}

