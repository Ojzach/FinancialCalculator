

using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Models
{
    public class FinancialAccount
    {
        public int ID { get; set; } = -1;
        public string accountName { get; set; } = "";
        public int financialInstitutionID { get; set; } = -1;
        public BankAccountType accountType { get; set; }
        public float currentBalance { get; set; } = 0.0f;
        public bool isPreTaxAccount { get; set; }

        public FinancialAccount() { }

        public FinancialAccount(int id, string _accountName, int _financialInstitutionID, BankAccountType _accountType, float _currentBalance = 0, bool _isPreTaxAccount = false)
        {
            ID = id;
            accountName = _accountName;
            financialInstitutionID = _financialInstitutionID;
            accountType = _accountType;
            currentBalance = _currentBalance;
            isPreTaxAccount = _isPreTaxAccount;
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
