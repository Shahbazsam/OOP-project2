using System.Globalization;
using MyMenu;

while (true)
{
    var wallet = new Wallet();
    var walletService = new Service(wallet);

    Console.ForegroundColor = ConsoleColor.White;
    LogicService service = new BusinessLogicService();
    Storage storage = Storage.GetInstance();
    var activeUser = storage.GetActiveUser();
    Console.Clear();
    Console.WriteLine("--------------------------------- Welcome ----------------------------------------\n");
    if (activeUser == null)
    {
        Console.WriteLine("1: Register");
        Console.WriteLine("2: Login");
        Console.WriteLine("5: Quit");

    }
    else
    {
        var user = storage.GetActiveUser();
        Console.WriteLine($"Hello: {user.Name}");
        Console.WriteLine("1: Create wallet");
        Console.WriteLine("2: Active wallets");
        Console.WriteLine("4: Log out");
        Console.WriteLine("5: Quit");

    }


    Console.WriteLine(Environment.NewLine + "Choose menu:");

    var userInput = Console.ReadLine();
    if (int.TryParse(userInput, out var output))
    {
        switch (output)
        {
            case 1:
                if (activeUser == null)
                {
                    Register(service);
                }
                else
                {
                    CreateWallet(service);
                }
                break;
            case 2:
                if (activeUser == null)
                {
                    Login(service);
                }
                else
                {
                    ActiveWallets(service, walletService); 
                }
                break;
            case 3:
                if (activeUser == null)
                {
                   
                }
                else
                {
                    
                }
                break;
            case 4:
                if (activeUser == null)
                {
                    
                }
                else
                {
                    storage.LogOut();
                }
                break;
            case 5:
                return;
            default:

                break;
        }
    }
    else
    {
        Console.WriteLine("You must type a number only!");
        Console.ReadLine();
    }
}

static void Register(LogicService service)
{
    Console.Clear();

    Console.WriteLine("Enter your name: ");
    var name = Console.ReadLine();
    Console.WriteLine("Enter your email:");
    var email = Console.ReadLine();
    Console.WriteLine("Create a password:");
    var pass = Console.ReadLine();
    Console.Write("Enter security code of 4 digits: ");
    var code = Console.ReadLine();
    Console.WriteLine("Enter your birthdate in format 'dd.mm.yyyy':");
    var birthDate = Console.ReadLine();

    var result = service.Register(name, email, pass, code, birthDate);
    Console.ForegroundColor = result.Item1 ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(result.Item2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.ReadLine();
}

// LOGIN
static void Login(LogicService service)
{
    Console.Clear();

    Console.WriteLine("Email:");
    var email = Console.ReadLine();
    Console.WriteLine("Password:");
    var pass = Console.ReadLine();
    Console.Clear();

    Console.Write("Enter the security code: ");
    var securityCode = Console.ReadLine();

    var result = service.Login(email, pass, securityCode);
    Console.ForegroundColor = result.Item1 ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(result.Item2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.ReadLine();
}
//  CREATE WALLET
static void CreateWallet(LogicService service)
{
    Console.Clear();
    Console.WriteLine("Choose and type the currency for your wallet: USD, RUB, EUR ");
    var currency = Console.ReadLine().ToUpper();

    if (!IsValidCurrency(currency) || currency == null)
    {
        Console.WriteLine("Invalid currency! Please choose from USD, RUB, EUR");
        Console.ReadLine();
        return;
    }

    Console.WriteLine("Deposit Amount: ");
    if (uint.TryParse(Console.ReadLine(), out var DepositAmount) && DepositAmount > 0)
    {
        // Call the method to create a wallet with the provided currency and start amount
        service.CreateWallet(currency, DepositAmount);
        Console.WriteLine("Wallet created successfully!");
    }
    else
    {
        Console.WriteLine("Invalid amount! Please enter a positive integer.");
    }

    Console.ReadLine();
}

static bool IsValidCurrency(string currency)
{
    string[] validCurrencies = { "USD", "RUB", "EUR" };
    return validCurrencies.Contains(currency);
}


static void ActiveWallets(LogicService service, Service walletService)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("------------------------------ Active Wallets -----------------------------------\n");

        var activeUser = service.GetActiveUser();
        if (activeUser == null)
        {
            Console.WriteLine(" active user not found!");

            Console.ReadLine();
            return;
        }

        int walletIndex = 1;
        foreach (var wallet in activeUser.Wallets)
        {
            Console.WriteLine($"{walletIndex}: {wallet.Currency}");
            walletIndex++;
        }

        Console.WriteLine($"{walletIndex}: Delete Wallet");
        Console.WriteLine($"{walletIndex + 1}: Back to Main Menu");
        Console.WriteLine(Environment.NewLine + "Choose wallet or option:");

        var userInput = Console.ReadLine();
        if (int.TryParse(userInput, out var choice))
        {
            if (choice >= 1 && choice <= activeUser.Wallets.Count)
            {
                // Perform operations in the selected wallet
                PerformWalletOperations(walletService, activeUser.Wallets[choice - 1]);
            }
            else if (choice == walletIndex)
            {
                // Delete wallet option
                DeleteWallet(service, activeUser);
            }
            else if (choice == walletIndex + 1)
            {
                // Back to main menu
                return;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a valid number.");
                Console.ReadLine();
            }
        }
        else
        {
            Console.WriteLine("type number only!");
            Console.ReadLine();
        }
    }
}


static void DeleteWallet(LogicService service, User activeUser)
{
    Console.Clear();
    Console.WriteLine("Choose wallet you want to delete:");

    int walletIndex = 1;
    foreach (var wallet in activeUser.Wallets)
    {
        Console.WriteLine($"{walletIndex}: Currency: {wallet.Currency}, Balance: {wallet.StartAmount}");
        walletIndex++;
    }

    Console.WriteLine($"{walletIndex}: Cancel");
    Console.WriteLine(Environment.NewLine + "Choose wallet or cancel:");

    var userInput = Console.ReadLine();
    if (int.TryParse(userInput, out var choice))
    {
        if (choice >= 1 && choice <= activeUser.Wallets.Count)
        {
            // Delete the selected wallet
            service.DeleteWallet(choice - 1);
            Console.WriteLine("Wallet deleted successfully!");
        }
        else if (choice == walletIndex)
        {
            // Cancel option, go back to Active Wallets
            return;
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter a valid number.");
        }
    }
    else
    {
        Console.WriteLine("You must type a number only!");
    }

    Console.ReadLine();
}

// Updated method to perform operations in a specific wallet
static void PerformWalletOperations(Service walletService, Wallet selectedWallet)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine($"---------------------------------- Wallet Operations ({selectedWallet.Currency}) ------------------------------------\n");

        Console.WriteLine($"1: Income");
        Console.WriteLine($"2: Expense");
        Console.WriteLine($"3: Check Statistics");
        Console.WriteLine($"4: Back to Active Wallets");
        Console.WriteLine(Environment.NewLine + "Choose operation or go back:");

        var userInput = Console.ReadLine();
        if (int.TryParse(userInput, out var choice))
        {
            switch (choice)
            {
                case 1:
                    Console.WriteLine("Type value:");
                    if (decimal.TryParse(Console.ReadLine(), out var incomeValue) && incomeValue > 0)
                    {
                        Console.WriteLine("Enter the date of income (dd.MM.yyyy): ");
                        if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var incomeDate))
                        {
                            Console.WriteLine("Choose category:");
                            DisplayIncomeCategories();
                            if (Enum.TryParse(Console.ReadLine(), out IncomeCategory incomeCategory))
                            {
                                // Add income and update balance
                                if (walletService.AddIncome(incomeValue, incomeDate, incomeCategory))
                                {
                                    Console.WriteLine($"{incomeValue:N2} {selectedWallet.Currency} successfully added at {incomeDate:dd/MM/yyyy}");
                                    Console.WriteLine($" Balance: {walletService.GetCurrentBalance():N2} {selectedWallet.Currency}");
                                }
                                else
                                {
                                    Console.WriteLine("Failed to add income. Please check the amount and try again.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid category. Please enter a valid category.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format. Please enter a valid date.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid positive number.");
                    }
                    break;
                case 2:
                    Console.WriteLine("Type value:");
                    if (decimal.TryParse(Console.ReadLine(), out var expenseValue) && expenseValue > 0)
                    {
                        Console.WriteLine("Enter the date of expense (dd.MM.yyyy): ");
                        if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var expenseDate))
                        {
                            Console.WriteLine("Choose category:");
                            DisplayExpenseCategories();
                            if (Enum.TryParse(Console.ReadLine(), out ExpenseCategory expenseCategory))
                            {
                                // Add expense and update balance
                                if (walletService.AddExpense(expenseValue, expenseCategory))
                                {
                                    Console.WriteLine($"{expenseValue:N2} {selectedWallet.Currency} subtracted successfully at {expenseDate:dd/MM/yyyy}");
                                    Console.WriteLine($"Final Balance: {walletService.GetCurrentBalance():N2} {selectedWallet.Currency}");
                                }
                                else
                                {
                                    Console.WriteLine("Failed to subtract expense. Insufficient balance or invalid amount.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid category. Please enter a valid category.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format. Please enter a valid date.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid positive number.");
                    }
                    break;
                case 3:
                    Console.Clear();
                    CheckStatistics(walletService);
                    break;
                case 4:
                    // Back to Active Wallets
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please enter a valid number.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("You must type a number only!");
        }

        Console.ReadLine();
    }
}

static void DisplayExpenseCategories()
{
    Console.WriteLine($"1: {ExpenseCategory.Food}");
    Console.WriteLine($"2: {ExpenseCategory.Medicine}");
    Console.WriteLine($"3: {ExpenseCategory.Studies}");
    Console.WriteLine($"4: {ExpenseCategory.Home}");
    Console.WriteLine($"5: {ExpenseCategory.Clothing}");
    Console.WriteLine($"6: {ExpenseCategory.Other}");
}

static void DisplayIncomeCategories()
{
    Console.WriteLine($"1: {IncomeCategory.Salary}");
    Console.WriteLine($"2: {IncomeCategory.Cashback}");
    Console.WriteLine($"2: {IncomeCategory.Other}");
}

static void CheckStatistics(Service walletService)
{
    Console.WriteLine("Enter start date (dd.MM.yyyy): ");
    if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
    {
        Console.WriteLine("Enter end date (dd.MM.yyyy): ");
        if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
        {
            Console.Clear();
            if (startDate <= endDate)
            {
                DisplayStatistics(walletService, startDate, endDate);
            }
            else
            {
                Console.WriteLine("End date must be equal or later than the start date.");
            }
        }
        else
        {
            Console.WriteLine("Invalid enddate format.");
        }
    }
    else
    {
        Console.WriteLine("Invalid startdate format.");
    }
}

static void DisplayStatistics(Service walletService, DateTime startDate, DateTime endDate)
{
    var incomeCategories = walletService.GetIncomeCategories();
    var expenseCategories = walletService.GetExpenseCategories();

    Console.WriteLine($"------------------------------------- Statistics ({startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}) -----------------------------------\n");

    Console.WriteLine("Income Categories:");
    foreach (var category in incomeCategories)
    {
        Console.WriteLine($"{category.Key}: {category.Value:N2}");
    }

    Console.WriteLine("\nExpense Categories:");
    foreach (var category in expenseCategories)
    {
        Console.WriteLine($"{category.Key}: {category.Value:N2}");
    }

    var totalIncome = incomeCategories.Values.Sum();
    var totalExpense = expenseCategories.Values.Sum();

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"\nTotal Income: {totalIncome:N2}");
    Console.WriteLine($"Total Expense: {totalExpense:N2}");
    Console.WriteLine($"Final Balance: {totalIncome - totalExpense:N2}");
}