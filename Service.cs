namespace MyMenu
{
    internal class Service
    {
        private Wallet _wallet;
        private Dictionary<ExpenseCategory, decimal> _expenseCategories;
        private Dictionary<IncomeCategory, decimal> _incomeCategories;
        private Storage _storage;

        public Service(Wallet wallet)
        {
            _wallet = wallet;
            _expenseCategories = new Dictionary<ExpenseCategory, decimal>();
            _incomeCategories = new Dictionary<IncomeCategory, decimal>();
            _storage = Storage.GetInstance();
        }

        private List<Wallet> GetActiveUserWallets()
        {
            return _storage.GetActiveUserWallets();
        }

        private void SaveActiveUserWallets(List<Wallet> wallets)
        {
            _storage.SaveActiveUserWallets(wallets);
        }

        public bool AddIncome(decimal value, DateTime incomeDate, IncomeCategory incomeCategory)
        {
            if (_wallet.StartAmount + value >= _wallet.StartAmount)
            {
                _wallet.StartAmount += value;

                if (_incomeCategories.ContainsKey(incomeCategory))
                    _incomeCategories[incomeCategory] += value;
                else
                    _incomeCategories.Add(incomeCategory, value);

                _wallet.AddIncome(new Income { Value = value });
                return true;
            }

            return false;
        }

        public bool AddExpense(decimal value, ExpenseCategory expenseCategory)
        {
            // Ensure that there is enough balance
            if (_wallet.StartAmount >= value)
            {
                _wallet.StartAmount -= value;

                if (_expenseCategories.ContainsKey(expenseCategory))
                    _expenseCategories[expenseCategory] += value;
                else
                    _expenseCategories.Add(expenseCategory, value);

                _wallet.AddExpense(new Expense { Value = value });
                return true;
            }

            return false;
        }

        public decimal GetCurrentBalance()
        {
            return _wallet.StartAmount;
        }

        public Dictionary<ExpenseCategory, decimal> GetExpenseCategories()
        {
            return _expenseCategories;
        }

        public Dictionary<IncomeCategory, decimal> GetIncomeCategories()
        {
            return _incomeCategories;
        }

        public void CreateWallet(string currency, uint startAmount)
        {
            // Get the active user wallets
            var activeUserWallets = GetActiveUserWallets();

            // Create a new wallet
            var wallet = new Wallet { Currency = currency, StartAmount = startAmount };

            // Add the new wallet to the active user wallets
            activeUserWallets.Add(wallet);

            // Save the updated active user wallets
            SaveActiveUserWallets(activeUserWallets);
        }

        public (bool, string) DeleteWallet(int walletIndex)
        {
            // Get the active user wallets
            var activeUserWallets = GetActiveUserWallets();

            // Check if the walletIndex is valid
            if (walletIndex >= 0 && walletIndex < activeUserWallets.Count)
            {
                // Remove the wallet at the specified index
                activeUserWallets.RemoveAt(walletIndex);

                // Save the updated active user wallets
                SaveActiveUserWallets(activeUserWallets);

                return (true, "Wallet deleted successfully!");
            }

            return (false, "Invalid wallet index or no wallets available for deletion.");
        }
    }

    public enum ExpenseCategory
    {
        Food,
        Medicine,
        Studies,
        Home,
        Clothing,
        Other
    }

    public enum IncomeCategory
    {
        Salary,
        Cashback,
        Other
    }
}
