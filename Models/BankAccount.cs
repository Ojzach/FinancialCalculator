using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    internal class BankAccount
    {

        public string accountName;
        public BankAccountType accountType;
        public float currentBalance = 0.0f;


        public BankAccount(string _accountName, BankAccountType _accountType, float _currentBalance = 0)
        {
            accountName = _accountName;
            accountType = _accountType;
            currentBalance = _currentBalance;
        }

    }

    public enum BankAccountType
    {
        Checking,
        Savings,
        Credit
    }
}
