

namespace FinancialCalculator.Models
{
    public class FinancialAccount
    {

        public string accountName;
        public BankAccountType accountType;
        public float currentBalance = 0.0f;
        public List<Transaction> transactions = new List<Transaction>();


        //public FinancialAccount parentAccount; --Future Idea To Create Network For Transfers


        public FinancialAccount(string _accountName, BankAccountType _accountType, float _currentBalance = 0, List<Transaction>? _transactions = null)
        {
            accountName = _accountName;
            accountType = _accountType;
            currentBalance = _currentBalance;
            transactions = _transactions is null? new List<Transaction>() : _transactions;
        }

    }

    public enum BankAccountType
    {
        Checking,
        Savings,
        Credit
    }
}
