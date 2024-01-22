
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMenu

{
    public class Wallet
    {
        public string Currency { get; set; }
        public decimal StartAmount { get; set; }
        private List<Operation> Operations { get; set; } = new List<Operation>();
        public Wallet()
        {
            Operations = new List<Operation>();
        }

        public void AddExpense(Expense value)
        {
            Operations.Add(value);
        }

        public void AddIncome(Income value)
        {
            Operations.Add(value);
        }
    }

    public class Operation
    {
        public decimal Value { get; set; }
    }

    public class Expense : Operation { }
    public class Income : Operation { }
}
