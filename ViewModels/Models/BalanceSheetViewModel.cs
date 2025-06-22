using System.Collections.ObjectModel;
using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using FinancialCalculator.Commands;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace FinancialCalculator.ViewModel
{
    internal class BalanceSheetViewModel : ViewModelBase
    {
        protected BalanceSheet balanceSheet;

        public string BalanceSheetName { get => balanceSheet.name; set { balanceSheet.name = value; OnPropertyChanged("BalanceSheetName"); } }

        public float TotalBalanceSheetAmount
        {
            get
            {
                float amount = 0;
                foreach(BalanceItem bi in balanceSheet.BalanceItems) amount += bi.MonthlyAmount;
                return amount;
            }
        }

        public float TotalBalanceSheetPercent { get => TotalBalanceSheetAmount / _paycheck.TakeHomeAmount; }

        public ObservableCollection<BalanceItem> BalanceSheetItems
        {  get => balanceSheet.BalanceItems; set { balanceSheet.BalanceItems = value; } }


        protected bool addBalanceItemVisible = false;
        public Visibility AddBalanceItemBoxVisible { get => addBalanceItemVisible? Visibility.Visible: Visibility.Collapsed; }

        protected string addBalanceItemName = "";
        public string AddBalanceItemName { get => addBalanceItemName; set { addBalanceItemName = value; OnPropertyChanged("AddBalanceItemName"); } }


        protected PaycheckStore _paycheck;
        public BalanceSheetViewModel(PaycheckStore paycheck, string balanceSheetName)
        {
            _paycheck = paycheck;
            balanceSheet = new BalanceSheet();
            BalanceSheetName = balanceSheetName;


            OpenAddBalanceItemBoxCommand = new RelayCommand(execute => ToggleAddBalanceItemBox());
            CreateBalanceItemCommand = new RelayCommand(execute => CreateBalanceSheetItem(), canExecute => { return addBalanceItemVisible && AddBalanceItemName != ""; });

        }


        public RelayCommand OpenAddBalanceItemBoxCommand { get; }
        public RelayCommand CreateBalanceItemCommand { get; }

        private void ToggleAddBalanceItemBox()
        {

            AddBalanceItemName = "";
            addBalanceItemVisible = !addBalanceItemVisible;
            OnPropertyChanged("AddBalanceItemBoxVisible");
        }

        public void CreateBalanceSheetItem()
        {
            AddBalanceSheetItem(new BalanceItem(_paycheck, AddBalanceItemName));
            ToggleAddBalanceItemBox();
        }


        public void AddBalanceSheetItem(BalanceItem item)
        {
            balanceSheet.BalanceItems.Add(item);
            balanceSheet.BalanceItems[balanceSheet.BalanceItems.Count - 1].NumbersChanged += 
                () => { OnPropertyChanged("TotalBalanceSheetAmount"); OnPropertyChanged("TotalBalanceSheetPercent"); };
        }
    }
}
