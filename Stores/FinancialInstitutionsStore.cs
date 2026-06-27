using FinancialCalculator.Data;
using FinancialCalculator.Models;
using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Stores
{
    class FinancialInstitutionsStore
    {
        private List<FinancialInstitution> financialInstitutions = new();
        private Dictionary<int, FinancialAccount> financialAccounts = new();
        public List<FinancialInstitution> FinancialInstitutions { get => financialInstitutions; set => financialInstitutions = value; }

        private readonly FinancialDbContext _db;

        public FinancialInstitutionsStore(FinancialDbContext db,
            List<FinancialInstitution>? financialInstitutions = null,
            List<FinancialAccount>? financialAccounts = null)
        {
            _db = db;

            if (financialInstitutions != null)
                foreach (var fi in financialInstitutions) this.financialInstitutions.Add(fi);

            if (financialAccounts != null)
                foreach (var account in financialAccounts) this.financialAccounts.Add(account.ID, account);
        }

        public void AddFinancialInstitution(FinancialInstitution fi)
        {
            FinancialInstitutions.Add(fi);
        }

        public FinancialAccount GetFinancialAccount(int accountID) => financialAccounts[accountID];

        public Dictionary<int, FinancialAccount> GetAllAccounts() => financialAccounts;

        public string GetInstitutionNameForAccount(int accountID)
        {
            if (!financialAccounts.TryGetValue(accountID, out var account)) return "Unknown";
            var institution = financialInstitutions.FirstOrDefault(i => i.Id == account.financialInstitutionID);
            return institution?.Name ?? "Unknown";
        }

        public void AddTransaction(Transaction transaction)
        {
            _db.Transactions.Add(transaction);
            _db.SaveChanges();
        }

        public void UpdateFinancialInstitutionsList(List<FinancialInstitutionViewModel> _financialInstitutions)
        {
            financialInstitutions.Clear();
            foreach (var fi in _financialInstitutions) financialInstitutions.Add(fi.BaseFinancialInstitutionItem);
        }

        /// <summary>
        /// Saves changes to the financial institutions and accounts back to the DB.
        /// Call this when the user finishes editing on the Accounts page.
        /// </summary>
        public void SaveAll()
        {
            foreach (var fi in financialInstitutions)
            {
                var existing = _db.FinancialInstitutions.Find(fi.Id);
                if (existing == null)
                    _db.FinancialInstitutions.Add(fi);
                else
                {
                    existing.Name = fi.Name;
                    _db.FinancialInstitutions.Update(existing);
                }

                foreach (var account in fi.financialAccounts)
                {
                    var existingAccount = _db.FinancialAccounts.Find(account.ID);
                    if (existingAccount == null)
                        _db.FinancialAccounts.Add(account);
                    else
                    {
                        existingAccount.accountName = account.accountName;
                        existingAccount.accountType = account.accountType;
                        existingAccount.currentBalance = account.currentBalance;
                        _db.FinancialAccounts.Update(existingAccount);
                    }
                }
            }
            _db.SaveChanges();
        }
    }
}
