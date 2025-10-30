

using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Models
{
    public class FinancialAccount
    {
        public int ID { get; set; } = -1;
        public string accountName;
        public int financialInstitutionID = -1;
        public BankAccountType accountType;
        public float currentBalance = 0.0f;
        public List<Transaction> transactions = new List<Transaction>();
        public bool isPreTaxAccount;

        public FinancialAccount(int id, string _accountName, int _financialInstitutionID, BankAccountType _accountType, float _currentBalance = 0, bool _isPreTaxAccount = false, List<Transaction>? _transactions = null)
        {
            ID = id;
            accountName = _accountName;
            financialInstitutionID = _financialInstitutionID;
            accountType = _accountType;
            currentBalance = _currentBalance;
            isPreTaxAccount = _isPreTaxAccount;
            transactions = _transactions is null? new List<Transaction>() : _transactions;
        }

        public FinancialAccount(FinancialAccountViewModel accountViewModel)
        {
            accountName = accountViewModel.AccountName;
            accountType = accountViewModel.AccountType;
            currentBalance = accountViewModel.AccountBalance;
        }

    }

    public enum BankAccountType
    {
        Checking,
        Savings,
        Credit,
        Investment
    }
}
