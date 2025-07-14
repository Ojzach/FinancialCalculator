using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    internal class FinancialInstitution
    {


        public string Name { get; set; }

        public ObservableCollection<FinancialAccount> financialAccounts { get; set; } = new ObservableCollection<FinancialAccount>();

        public FinancialInstitution(string name)
        {
            Name = name;
        }
    }
}
