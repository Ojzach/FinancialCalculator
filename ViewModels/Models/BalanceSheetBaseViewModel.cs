using System.Collections.ObjectModel;
using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using FinancialCalculator.Commands;
using System.Windows;
using FinancialCalculator.Models;

namespace FinancialCalculator.ViewModels
{
    internal abstract class BalanceSheetBaseViewModel : ViewModelBase
    {
        protected BalanceSheet balanceSheet;

        public string BalanceSheetName { get => balanceSheet.name; set { balanceSheet.name = value; OnPropertyChanged("BalanceSheetName"); } }

        public float TotalBalanceSheetAmount
        {
            get
            {
                float amount = 0;
                foreach(BalanceItem bi in balanceSheet.BalanceItems) amount += bi.MonthlyAmt;
                return amount;
            }
        }

        public float TotalBalanceSheetPercent { get => TotalBalanceSheetAmount / _paycheck.GetPaycheckAmount(balanceSheet.isPreTax); }

        public ObservableCollection<BalanceItem> BalanceSheetItems
        {  get => balanceSheet.BalanceItems; set { balanceSheet.BalanceItems = value; } }


        protected bool addBalanceItemVisible = false;
        public Visibility AddBalanceItemBoxVisible { get => addBalanceItemVisible? Visibility.Visible: Visibility.Collapsed; }

        protected string addBalanceItemName = "";
        public string AddBalanceItemName { get => addBalanceItemName; set { addBalanceItemName = value; OnPropertyChanged("AddBalanceItemName"); } }

        public Action? BalanceSheetUpdated;

        private BalanceItem? selectedBalanceSheetItem;
        public BalanceItem? SelectedBalanceSheetItem { get => selectedBalanceSheetItem; set => selectedBalanceSheetItem = value; }


        protected PaycheckStore _paycheck;

        public BalanceSheetBaseViewModel(PaycheckStore paycheck, string balanceSheetName, bool isPreTax = false)
        {
            balanceSheet = new BalanceSheet(balanceSheetName, _isPreTax: isPreTax);
            _paycheck = paycheck;

            OpenAddBalanceItemBoxCommand = new RelayCommand(execute => ToggleAddBalanceItemBox());
            CreateBalanceItemCommand = new RelayCommand(execute => CreateBalanceSheetItem(), canExecute => { return addBalanceItemVisible && AddBalanceItemName != ""; });

            DeleteItemCommand = new RelayCommand(DeleteItem, canExecute => SelectedBalanceSheetItem != null);

        }


        public RelayCommand OpenAddBalanceItemBoxCommand { get; }
        public RelayCommand CreateBalanceItemCommand { get; }

        protected void ToggleAddBalanceItemBox()
        {
            AddBalanceItemName = "";
            addBalanceItemVisible = !addBalanceItemVisible;
            OnPropertyChanged("AddBalanceItemBoxVisible");
        }

        public virtual void CreateBalanceSheetItem()
        {
            AddBalanceSheetItem(new BalanceItem(_paycheck, new BankAccount(AddBalanceItemName, BankAccountType.Checking), AddBalanceItemName));
            ToggleAddBalanceItemBox();
        }


        public void AddBalanceSheetItem(BalanceItem item)
        {
            if(balanceSheet.isPreTax) item.isPreTaxBalanceItem = true;
            balanceSheet.BalanceItems.Add(item);
            balanceSheet.BalanceItems[balanceSheet.BalanceItems.Count - 1].NumbersChanged += BalanceItemChanged;
        }

        protected virtual void BalanceItemChanged()
        {
            OnPropertyChanged("TotalBalanceSheetAmount");
            OnPropertyChanged("TotalBalanceSheetPercent");

            BalanceSheetUpdated?.Invoke();
        }


        public RelayCommand DeleteItemCommand { get; }

        protected void DeleteItem(object sender)
        {
            SelectedBalanceSheetItem.NumbersChanged -= BalanceItemChanged;
            BalanceSheetItems.Remove(SelectedBalanceSheetItem);

            BalanceItemChanged();
        }
    }
}
