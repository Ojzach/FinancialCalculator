using FinancialCalculator.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Model
{
    internal class BalanceSheet
    {

        public float paycheckAmount;
        public float federalTaxAmount;
        public float stateTaxAmount;


        public float takeHomeAmount { get => paycheckAmount - federalTaxAmount - stateTaxAmount; }


        public ObservableCollection<BalanceItem> investmentBalanceItems = new ObservableCollection<BalanceItem>();
        public ObservableCollection<BalanceItem> fixedCostsBalanceItems = new ObservableCollection<BalanceItem>();
        public ObservableCollection<BalanceItem> savingsBalanceItems = new ObservableCollection<BalanceItem>();
        public ObservableCollection<BalanceItem> freeSpendingBalanceItems = new ObservableCollection<BalanceItem>();

        public BalanceSheet(BalanceSheetViewModel viewModel) 
        {
            paycheckAmount = 9333.56f;
            federalTaxAmount = 2333.39f;
            stateTaxAmount = 746.68f;


            investmentBalanceItems.Add(new BalanceItem(this, "401K", _monthlyAmount: 933.36f));
            investmentBalanceItems.Add(new BalanceItem(this, "RothIRA", _monthlyPercent: 0.05f));
            investmentBalanceItems.Add(new BalanceItem(this, "HSA"));

            fixedCostsBalanceItems.Add(new BalanceItem(this, "Studen Loans"));
            fixedCostsBalanceItems.Add(new BalanceItem(this, "Motorcycle Insurance"));
            fixedCostsBalanceItems.Add(new BalanceItem(this, "AMO Dues"));


            freeSpendingBalanceItems.Add(new BalanceItem(this, "Food"));
            freeSpendingBalanceItems.Add(new BalanceItem(this, "Gas"));
            freeSpendingBalanceItems.Add(new BalanceItem(this, "Food"));
            freeSpendingBalanceItems.Add(new BalanceItem(this, "Spotify", _monthlyAmount: 12.71f));
            freeSpendingBalanceItems.Add(new BalanceItem(this, "Adobe", _monthlyAmount: 15.89f));
            freeSpendingBalanceItems.Add(new BalanceItem(this, "Google Photos", _monthlyAmount: 2.11f));
            freeSpendingBalanceItems.Add(new BalanceItem(this, "Other"));
        }

    }
}
