
using System.Text.Json;

namespace MyMenu
{
    public class Storage
    {
        private static Storage _storage;
        private const string FilePath = "users.json";  // File path to store user data

        public static Storage GetInstance()
        {
            if (_storage == null)
            {
                _storage = new Storage();
            }

            return _storage;
        }

        private List<User> Users { get; }
        private User? ActiveUser { get; set; }

        private Storage()
        {
            Users = LoadUsers(); 
        }

        public void AddUser(User user)
        {
            if (Users.Any(x => x.Email.ToLower() == user.Email.ToLower()))
            {
                throw new ArgumentException(" user exist with same email");
            }

            Users.Add(user);
            SaveUsers(); 
        }

        private void SaveUsers()
        {
            string json = JsonSerializer.Serialize(Users);
            File.WriteAllText(FilePath, json);
        }

        private List<User> LoadUsers()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }

            return new List<User>();
        }

        public User FindUserByEmail(string email)
        {
            var user = Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email {email} does not found!");
            }

            return user;
        }

        public void SetActiveUser(string email)
        {
            var user = Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email {email} does not found!");
            }

            ActiveUser = user;
        }

        public User? GetActiveUser()
        {
            return ActiveUser;
        }

        public List<Wallet> GetActiveUserWallets()
        {
            var activeUser = GetActiveUser();
            return activeUser?.Wallets ?? new List<Wallet>();
        }

        public void SaveActiveUserWallets(List<Wallet> wallets)
        {
            var activeUser = GetActiveUser();
            if (activeUser != null)
            {
                activeUser.Wallets = wallets;
                SaveUsers();
            }
        }

        public void LogOut()
        {
            ActiveUser = null;
        }
       


    }
}

