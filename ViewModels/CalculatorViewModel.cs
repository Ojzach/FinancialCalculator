using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FinancialCalculator.Model;
using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using NodaTime;

namespace FinancialCalculator.ViewModels
{
    internal class CalculatorViewModel : ViewModelBase
    {
        public float PaycheckAmount
        {
            get => _paycheck.PaycheckAmount;
            set { _paycheck.PaycheckAmount = value; UpdateCalculatedValues(); }
        }

        public float EstimatedYearlyIncome { get => _paycheck.EstimatedyearlyIncome; set { _paycheck.EstimatedyearlyIncome = value; UpdateCalculatedValues(); } }
        public int MonthsCoveredByPaycheck { get => _paycheck.MonthsCoveredByPaycheck; }

        public float TakeHomeAmount { get => _paycheck.TakeHomeAmount; }
        public float TakeHomePercent { get => BalanceSheets.Sum(item => item.TotalBalanceSheetPercent); }



        private PaycheckStore _paycheck;


        private ObservableCollection<BalanceSheetBaseViewModel> balanceSheets = new ObservableCollection<BalanceSheetBaseViewModel>();
        public ObservableCollection<BalanceSheetBaseViewModel> BalanceSheets { get => balanceSheets; set { balanceSheets = value; UpdateCalculatedValues(); } }

        public BalanceSheetBaseViewModel PaycheckDeductionsBalanceSheet { get; set; }

        public CalculatorViewModel()
        {
            _paycheck = new PaycheckStore();

            PaycheckDeductionsBalanceSheet = new BalanceSheetViewModel(_paycheck, "Paycheck Deductions", preTaxBalanceSheet: true);
            PaycheckDeductionsBalanceSheet.BalanceSheetUpdated +=
                () =>
                {
                    _paycheck.PaycheckDeductions = PaycheckDeductionsBalanceSheet.TotalBalanceSheetAmount;
                    UpdateCalculatedValues();
                };


            PaycheckDeductionsBalanceSheet.AddBalanceSheetItem(new BalanceItem(_paycheck, "Federal Tax"));
            PaycheckDeductionsBalanceSheet.AddBalanceSheetItem(new BalanceItem(_paycheck, "Medicare"));
            PaycheckDeductionsBalanceSheet.AddBalanceSheetItem(new BalanceItem(_paycheck, "Social Security"));
            PaycheckDeductionsBalanceSheet.AddBalanceSheetItem(new BalanceItem(_paycheck, "State Tax"));
            PaycheckDeductionsBalanceSheet.AddBalanceSheetItem(new BalanceItem(_paycheck, "401K"));


            BalanceSheets.Add(new BalanceSheetViewModel(_paycheck, "Investments"));
            BalanceSheets.Add(new BalanceSheetViewModel(_paycheck, "Fixed Costs"));
            BalanceSheets.Add(new SavingsBalanceSheetViewModel(_paycheck, "Savings"));
            BalanceSheets.Add(new BalanceSheetViewModel(_paycheck, "Free Spending"));
            

            BalanceSheets[0].AddBalanceSheetItem(new BalanceItem(_paycheck, "401K", _monthlyAmount: 0.00f/*933.36f*/));
            BalanceSheets[0].AddBalanceSheetItem(new BalanceItem(_paycheck, "RothIRA", _monthlyPercent: 0.05f));
            BalanceSheets[0].AddBalanceSheetItem(new BalanceItem(_paycheck, "HSA"));

            BalanceSheets[1].AddBalanceSheetItem(new BalanceItem(_paycheck, "Studen Loans", _monthlyAmount: 500.0f));
            //BalanceSheets[1].AddBalanceSheetItem(new BalanceItem(_paycheck, "Motorcycle Insurance", _monthlyAmount: 43.32f));
            BalanceSheets[1].AddBalanceSheetItem(new BalanceItem(_paycheck, "AMO Dues", _monthlyAmount: 141.67f));

            BalanceSheets[2].AddBalanceSheetItem(new SavingsBalanceItem(_paycheck, "Emergency Fund", new BankAccount("EmergencyFund", BankAccountType.Savings, 2000), 10000, new LocalDate(2025, 8, 25)));
            //BalanceSheets[2].AddBalanceSheetItem(new SavingsBalanceItem(_paycheck, "Rally Car", new BankAccount("RallyCar", BankAccountType.Savings, 0), 15000, new LocalDate(2026, 1, 1)));
            //BalanceSheets[2].AddBalanceSheetItem(new SavingsBalanceItem(_paycheck, "House", new BankAccount("House", BankAccountType.Savings, 0), 50000, new LocalDate(2026, 10, 20)));

            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Food"));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Gas"));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Food"));
            //BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Spotify", _monthlyAmount: 12.71f));
            //BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Adobe", _monthlyAmount: 15.89f));
            //BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Google Photos", _monthlyAmount: 2.11f));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Other"));

            foreach (BalanceSheetBaseViewModel balanceSheet in BalanceSheets) balanceSheet.BalanceSheetUpdated += UpdateCalculatedValues;

        }



        public void UpdateCalculatedValues()
        {
            OnPropertyChanged("TakeHomeAmount");
            OnPropertyChanged("TakeHomePercent");
            OnPropertyChanged("EstimatedYearlyIncome");
            OnPropertyChanged("MonthsCoveredByPaycheck");

            OnPropertyChanged("BalanceSheets");

        }
    }

}
