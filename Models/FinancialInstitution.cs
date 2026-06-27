using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public class FinancialInstitution
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public ObservableCollection<FinancialAccount> financialAccounts { get; set; } = new ObservableCollection<FinancialAccount>();

        public FinancialInstitution() { }

        public FinancialInstitution(string name)
        {
            Name = name;
        }
    }
}
