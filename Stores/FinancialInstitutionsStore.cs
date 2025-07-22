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
        private List<FinancialInstitution> financialInstitutions = new List<FinancialInstitution>();
        public List<FinancialInstitution> FinancialInstitutions { get => financialInstitutions; set => financialInstitutions = value; }


        public void AddFinancialInstitution(FinancialInstitution fi)
        {
            FinancialInstitutions.Add(fi);
        }

        public void UpdateFinancialInstitutionsList(List<FinancialInstitutionViewModel> _financialInstitutions)
        {
            financialInstitutions.Clear();
            foreach (FinancialInstitutionViewModel fi in _financialInstitutions) financialInstitutions.Add(fi.BaseFinancialInstitutionItem);
        }
    }
}
