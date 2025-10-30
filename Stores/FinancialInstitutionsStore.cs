using FinancialCalculator.Models;
using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Stores
{
    class FinancialInstitutionsStore
    {
        private List<FinancialInstitution> financialInstitutions = new();
        private Dictionary<int, FinancialAccount> financialAccounts = new();
        public List<FinancialInstitution> FinancialInstitutions { get => financialInstitutions; set => financialInstitutions = value; }

        public FinancialInstitutionsStore(List<FinancialInstitution> financialInstitutions = null, List<FinancialAccount> financialAccounts = null)
        {
            if(financialInstitutions != null)
                foreach(FinancialInstitution financialInstitution in financialInstitutions) this.financialInstitutions.Add(financialInstitution);

            if (financialAccounts != null)
                foreach (FinancialAccount financialAccount in financialAccounts) this.financialAccounts.Add(financialAccount.ID, financialAccount);
        }


        public void AddFinancialInstitution(FinancialInstitution fi)
        {
            FinancialInstitutions.Add(fi);
        }

        public FinancialAccount GetFinancialAccount(int accountID) => financialAccounts[accountID];

        public void UpdateFinancialInstitutionsList(List<FinancialInstitutionViewModel> _financialInstitutions)
        {
            financialInstitutions.Clear();
            foreach (FinancialInstitutionViewModel fi in _financialInstitutions) financialInstitutions.Add(fi.BaseFinancialInstitutionItem);
        }
    }
}
