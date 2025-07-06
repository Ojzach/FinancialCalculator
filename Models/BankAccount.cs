

namespace FinancialCalculator.Models
{
    internal class BankAccount
    {

        public string accountName;
        public BankAccountType accountType;
        public float currentBalance = 0.0f;
        public List<object> transactions = new List<object>();

        public BankAccount parentBankAcount;


        public BankAccount(string _accountName, BankAccountType _accountType, float _currentBalance = 0, List<object> _transactions = null, BankAccount _parentBankAcount = null)
        {
            accountName = _accountName;
            accountType = _accountType;
            currentBalance = _currentBalance;
            transactions = _transactions is null? new List<object>() : _transactions;
            parentBankAcount = _parentBankAcount;
        }

    }

    public enum BankAccountType
    {
        Checking,
        Savings,
        Credit
    }
}
