using System.Collections.ObjectModel;
using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using FinancialCalculator.Commands;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

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

        public float TotalBalanceSheetPercent { get => TotalBalanceSheetAmount / _paycheck.TakeHomeAmount; }

        public ObservableCollection<BalanceItem> BalanceSheetItems
        {  get => balanceSheet.BalanceItems; set { balanceSheet.BalanceItems = value; } }


        protected bool addBalanceItemVisible = false;
        public Visibility AddBalanceItemBoxVisible { get => addBalanceItemVisible? Visibility.Visible: Visibility.Collapsed; }

        protected string addBalanceItemName = "";
        public string AddBalanceItemName { get => addBalanceItemName; set { addBalanceItemName = value; OnPropertyChanged("AddBalanceItemName"); } }

        public Action BalanceSheetUpdated;

        private BalanceItem selectedBalanceSheetItem;
        public BalanceItem SelectedBalanceSheetItem { get => selectedBalanceSheetItem; set => selectedBalanceSheetItem = value; }


        protected PaycheckStore _paycheck;

        public BalanceSheetBaseViewModel(PaycheckStore paycheck, string balanceSheetName)
        {
            balanceSheet = new BalanceSheet();
            _paycheck = paycheck;
            BalanceSheetName = balanceSheetName;

            OpenAddBalanceItemBoxCommand = new RelayCommand(execute => ToggleAddBalanceItemBox());
            CreateBalanceItemCommand = new RelayCommand(execute => CreateBalanceSheetItem(), canExecute => { return addBalanceItemVisible && AddBalanceItemName != ""; });

            DeleteItemCommand = new RelayCommand(DeleteItem, canExecute => SelectedBalanceSheetItem != null);

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


        public virtual void AddBalanceSheetItem(BalanceItem item)
        {
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
