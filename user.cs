

namespace MyMenu
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string SecurityCode { get; set; } 
        public List<Wallet> Wallets { get; set; } = new List<Wallet>();
    }
}
