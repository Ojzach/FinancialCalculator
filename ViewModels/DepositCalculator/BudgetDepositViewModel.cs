using FinancialCalculator.Models;
using System.Collections.ObjectModel;
using NodaTime;
using FinancialCalculator.Stores;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using FinancialCalculator.Commands;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace FinancialCalculator.ViewModels
{
    internal class BudgetDepositViewModel : ViewModelBase
    {
        private ObservableCollection<BudgetDepositViewModel> subItems = new ObservableCollection<BudgetDepositViewModel>();
        public ObservableCollection<BudgetDepositViewModel> SubItems { 
            get => subItems; 
            set { 
                subItems = value; 
                OnPropertyChanged(nameof(SubItems)); 
                OnPropertyChanged(nameof(SubItemVisibility));
            } }

        protected BudgetDepositViewModel UnallocattedBudget;

        public virtual bool IsAmtEditable { get => (_budget is not FillBudget && _budget.Name != "Deposit"); }
        private bool _isVisible = true;
        private bool isVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); } }
        public Visibility IsVisible { get => isVisible ? Visibility.Visible : Visibility.Collapsed; }
        public Visibility SubItemVisibility { get => SubItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }


        protected Budget _budget;
        private int budgetID;
        protected DepositStore _deposit;


        public string BudgetName { get => _budget.Name; }
        public string BudgetType { get => _budget.BudgetType; }
        public float BudgetRecommendedAmtPerMonth { get => _budget.GetRecommendedMonthlyDepositAmt(_deposit.GetDepositAmount(_budget.AssociatedFinancialAccount.isPreTaxAccount)); }


        private AmountPercentModel depositAmtPct { get => _deposit.BudgetDeposits[budgetID].DepositAmtPct; } 
        public bool IsUsrSet { get => _deposit.BudgetDeposits[budgetID].DepositIsUserSet; set { _deposit.BudgetDeposits[budgetID].DepositIsUserSet = value; OnPropertyChanged(nameof(IsUsrSet)); } }
        public bool IsSetByAmt { get => depositAmtPct.IsSetByAmount; }

        public float UsrDepositPct { get => DepositPct; set { IsUsrSet = true; DepositPct = value; OnPropertyChanged(nameof(IsSetByAmt)); BudgetValueChanged?.Invoke(this); } }
        public float DepositPct { 
            get => depositAmtPct.GetPercent(_deposit.GetBudgetReferenceAmount(budgetID)); 
            set {
                float startAmt = depositAmtPct.GetAmount(_deposit.GetBudgetReferenceAmount(budgetID));
                depositAmtPct.Percent = value; 
                OnPropertyChanged(nameof(UsrDepositPct));
                OnPropertyChanged(nameof(UsrDepositAmt));
                
                if(startAmt != depositAmtPct.GetAmount(_deposit.GetBudgetReferenceAmount(budgetID))) ValueChanged();
            } 
        }
        public float UsrDepositAmt { get => DepositAmt; set { IsUsrSet = true; DepositAmt = value; OnPropertyChanged(nameof(IsSetByAmt)); BudgetValueChanged?.Invoke(this); } }
        public float DepositAmt { 
            get => depositAmtPct.GetAmount(_deposit.GetBudgetReferenceAmount(budgetID)); 
            set {
                float startAmt = depositAmtPct.GetAmount(_deposit.DepositAmount);
                depositAmtPct.Amount = value;
                OnPropertyChanged(nameof(UsrDepositPct));
                OnPropertyChanged(nameof(UsrDepositAmt));
                if (startAmt != depositAmtPct.GetAmount(_deposit.GetBudgetReferenceAmount(budgetID))) ValueChanged();
            } 
        }
        public void TotalDepositAmtChanged()
        {
            float startAmt = depositAmtPct.GetAmount(_deposit.GetBudgetReferenceAmount(budgetID));
            OnPropertyChanged(nameof(UsrDepositPct));
            OnPropertyChanged(nameof(UsrDepositAmt));

            if(startAmt != depositAmtPct.GetAmount(_deposit.GetBudgetReferenceAmount(budgetID)))
            {
                ValueChanged();
                budgetDebugColor = Color.Black;
            }    
        }


        protected float TotalDepositAmt { get => _budget.AssociatedFinancialAccount.isPreTaxAccount ? _deposit.DepositAmount : _deposit.TakeHomeAmount; }
        public float MaxAmt { get => _budget.GetMaxMonthlyDepositAmt(_deposit.GetBudgetReferenceAmount(_budget.ID)); }
        public float MinAmt { get => _budget.GetMinMonthlyDepositAmt(_deposit.GetBudgetReferenceAmount(_budget.ID)); }




        public ICommand EditBudgetCommand { get; set; }

        public BudgetDepositViewModel(int _budgetID, BudgetStore budgetStore, DepositStore _depositStore)
        {
            budgetID = _budgetID;
            _budget = budgetStore.GetBudget(_budgetID);
            _deposit = _depositStore;

            foreach (int budgetID in _budget.ChildBudgets)
            {
                SubItems.Add(new BudgetDepositViewModel(budgetID, budgetStore, _depositStore));
            }


            EditBudgetCommand = new RelayCommand(execute => EditBudgetAction?.Invoke(this._budget.ToViewModel()));
        }

        public Action<BudgetDepositViewModel> BudgetValueChanged;
        public Action<ViewModelBase> EditBudgetAction;

        public void ValueChanged()
        {

            if (SubItems.Count > 0) RebalanceSubItems();

        }

        public void BudgetUpdated()
        {
            Debug.Print("Hello");
        }


        //Only Applies To Fixed And Flexible Budgets
        private void SubItemValueChanged(BudgetDepositViewModel changedBudget)
        {
            

            budgetDebugColor = Color.Gray;

            List<BudgetDepositViewModel> usrSetValueBudgets = SubItems.Where(item => item.IsUsrSet).ToList();

            float UsrSetSum = usrSetValueBudgets.Sum(item => item.DepositAmt);
            float OtherItemsMinSum = SubItems.Except(usrSetValueBudgets).Sum(item => item.MinAmt);


            /*string t = "";
            foreach (BudgetDepositViewModel item in SubItems) t += "\n\t" + item.BudgetName + " " + item.MinAmt;

            Debug.Print(BudgetName + " UsrSet: " + UsrSetSum + " MinSum: " + OtherItemsMinSum + " DepositAmt: " + DepositAmt + t);*/

            if (UsrSetSum + OtherItemsMinSum > DepositAmt)
            {
                if (UsrSetSum + OtherItemsMinSum - changedBudget.DepositAmt > MaxAmt) BudgetError();
                else
                {
                    if(_budget is FlexibleBudget)
                    {
                        BudgetValueChanged.Invoke(this);
                    }
                    else
                    {
                        budgetDebugColor = Color.Orange;
                        changedBudget.DepositAmt = (DepositAmt - (UsrSetSum + OtherItemsMinSum - changedBudget.DepositAmt));
                        RebalanceSubItems();
                    }      
                }
            }
            else
            {
                RebalanceSubItems();
            }

        }

        private void RebalanceSubItems()
        {

            budgetDebugColor = Color.Gray;
            foreach (BudgetDepositViewModel si in SubItems) si.budgetDebugColor = Color.Gray;

            float availableSum = DepositAmt;

            List<BudgetDepositViewModel> unAssignedBudgets = SubItems.ToList();
            unAssignedBudgets.Remove(UnallocattedBudget);


            //Find the sum of the budgets that are set by the user. These are not editable so they will be subtracted out of rebalance

            foreach(BudgetDepositViewModel budget in unAssignedBudgets.Where(b => b.IsUsrSet))
            {
                budget.TotalDepositAmtChanged();
                availableSum = availableSum - budget.DepositAmt;
            }
            unAssignedBudgets.RemoveAll(b => b.IsUsrSet);

            if(availableSum < 0)
            {
                BudgetError();
                availableSum = 0;
            }


            //Find the sum of the budgets that are fixed amounts. These are not editable so they will be subtracted out of rebalance
            //Where function does not add fixed cost budgets that are also user set as to not double subtract
            if(availableSum >= unAssignedBudgets.Where(item => item._budget is FixedBudget || item._budget is RecurringExpenseBudget).Sum(item => item.BudgetRecommendedAmtPerMonth))
            {
                foreach (BudgetDepositViewModel budget in unAssignedBudgets.Where(item => item._budget is FixedBudget || item._budget is RecurringExpenseBudget))
                {
                    budget.DepositAmt = budget.BudgetRecommendedAmtPerMonth;
                    availableSum = availableSum - budget.DepositAmt;
                }
                unAssignedBudgets.RemoveAll(item => item._budget is FixedBudget || item._budget is RecurringExpenseBudget);

            }
            else
            {
                BudgetError();
                foreach (KeyValuePair<BudgetDepositViewModel, float> keyValue in DistributeAmtAmongBudgets(unAssignedBudgets.Where(item => item._budget is FixedBudget || item._budget is RecurringExpenseBudget).ToList(), availableSum, (budget) => 1, (budget) => 0))
                {
                    keyValue.Key.DepositAmt = keyValue.Value;
                    availableSum = availableSum - keyValue.Value;
                    unAssignedBudgets.Remove(keyValue.Key);
                }
                availableSum = 0;
            }


            //Distribute Felexible Budgets

            List<BudgetDepositViewModel> savingBudgets = unAssignedBudgets.Where(item => item._budget is SavingsBudget).ToList();
            List<BudgetDepositViewModel> flexibleBudgets = unAssignedBudgets.Where(item => item._budget is FlexibleBudget).ToList();


            if(availableSum >= savingBudgets.Sum(item => item.MaxAmt) + flexibleBudgets.Sum(item => item.MaxAmt))
            {
                availableSum = availableSum - savingBudgets.Sum(item => item.MaxAmt) + flexibleBudgets.Sum(item => item.MaxAmt);
                foreach (BudgetDepositViewModel budget in savingBudgets) { budget.DepositAmt = budget.MaxAmt; unAssignedBudgets.Remove(budget); }
                foreach (BudgetDepositViewModel budget in flexibleBudgets) { budget.DepositAmt = budget.MaxAmt; unAssignedBudgets.Remove(budget); }
            }
            else
            {
                

                if (availableSum >= savingBudgets.Sum(item => item.MinAmt) + flexibleBudgets.Sum(item => item.MinAmt))
                {

                    availableSum = availableSum - savingBudgets.Sum(item => item.MinAmt) - flexibleBudgets.Sum(item => item.MinAmt);

                    if (availableSum >= 0)
                    {

                        foreach (KeyValuePair<BudgetDepositViewModel, float> keyValue in DistributeAmtAmongBudgets(unAssignedBudgets.Where(budget => budget._budget is SavingsBudget).ToList(), availableSum, (budget) => 1, (budget) => budget.MinAmt))
                        {
                            keyValue.Key.DepositAmt = keyValue.Key.MinAmt + keyValue.Value;
                            availableSum = availableSum - keyValue.Value;
                            unAssignedBudgets.Remove(keyValue.Key);
                        }
                    }

                    if (availableSum >= 0)
                    {
                        foreach (KeyValuePair<BudgetDepositViewModel, float> keyValue in DistributeAmtAmongBudgets(unAssignedBudgets.Where(budget => budget._budget is FlexibleBudget).ToList(), availableSum, (budget) => 1, (budget) => budget.MinAmt))
                        {
                            keyValue.Key.DepositAmt = keyValue.Key.MinAmt + keyValue.Value;
                            availableSum = availableSum - keyValue.Value;
                            unAssignedBudgets.Remove(keyValue.Key);
                        }
                    }

                }
                else
                {
                    BudgetError();
                    availableSum = 0;
                }



            }

            if(!UnallocattedBudget.IsUsrSet)
            {
                UnallocattedBudget.DepositAmt = availableSum;
                unAssignedBudgets.Remove(UnallocattedBudget);

                if (_budget is FixedBudget && UnallocattedBudget.DepositAmt == 0) UnallocattedBudget.isVisible = false;
                else UnallocattedBudget.isVisible = true;
            }


            foreach (BudgetDepositViewModel budget in unAssignedBudgets) budget.DepositAmt = 0;


            if (MathF.Round(SubItems.Sum(item => item.DepositAmt), 2) != MathF.Round(DepositAmt, 2))
            {
                string t = "";
                //foreach (BudgetDepositViewModel item in SubItems) t += "\n\t" + item.BudgetName + " " + item.DepositAmt;
                Debug.Print(BudgetName + " " + SubItems.Sum(item => item.DepositAmt) + " " + DepositAmt + " " + t);
                BudgetError();
            }
        }

        private Dictionary<BudgetDepositViewModel, float> DistributeAmtAmongBudgets(List<BudgetDepositViewModel> budgets, float amt, Func<BudgetDepositViewModel, float> ratioWeight, Func<BudgetDepositViewModel, float> preAllocatedAmt)
        {
            Dictionary<BudgetDepositViewModel, float> distributedAmts = budgets.ToDictionary(b => b, b => 0f);
            bool finishedAllocation = false;
            float totalUsed = 0;

            while (finishedAllocation == false)
            {
                finishedAllocation = true;
                float ratioTotal = budgets.Count();

                foreach (BudgetDepositViewModel budget in budgets)
                {
                    float calculatedAmt = (amt - totalUsed) * (ratioWeight(budget) / ratioTotal);

                    if (calculatedAmt + preAllocatedAmt(budget) >= budget.MaxAmt)
                    {
                        distributedAmts[budget] = budget.MaxAmt - preAllocatedAmt(budget);
                        totalUsed += budget.MaxAmt - preAllocatedAmt(budget);
                        budgets.Remove(budget);
                        finishedAllocation = false;
                        break;
                    }
                    else distributedAmts[budget] = calculatedAmt;
                }
            }

            return distributedAmts;
        }





        private Color _budgetDebugColor = Color.Gray;
        private Color budgetDebugColor { set { _budgetDebugColor = value; OnPropertyChanged(nameof(BudgetDebugColor)); } }
        public SolidColorBrush BudgetDebugColor { get => new SolidColorBrush(System.Windows.Media.Color.FromRgb(_budgetDebugColor.R, _budgetDebugColor.G, _budgetDebugColor.B)); }
        private void BudgetError() => budgetDebugColor = Color.Red;

    }
}
