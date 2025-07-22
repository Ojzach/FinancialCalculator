using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancialCalculator.Models;

namespace FinancialCalculator.ViewModels
{
    public class TransactionViewModel : ViewModelBase
    {
        private Transaction transaction;

        public string TransactionName { get => transaction.Name; set { transaction.Name = value; OnPropertyChanged(nameof(TransactionName)); } }

        public TransactionViewModel(Transaction transaction)
        {
            this.transaction = transaction;
        }
    }
}
