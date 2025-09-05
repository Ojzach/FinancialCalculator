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
    public abstract class BudgetDepositViewModel : ViewModelBase
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
        protected DepositStore _deposit;


        public string BudgetName { get => _budget.Name; }
        public string BudgetType { get => _budget.BudgetType; }
        public float BudgetRecommendedAmtPerMonth { get => _budget.GetRecommendedMonthlyDepositAmt(_deposit.GetDepositAmount(_budget.AssociatedFinancialAccount.isPreTaxAccount)); }


        protected AmtPct depositAmtPct;
        private bool isUsrSet = false;
        public bool IsUsrSet { get => isUsrSet; set { isUsrSet = value; OnPropertyChanged(nameof(isUsrSet)); } }
        public bool IsSetByAmt { get => depositAmtPct.IsSetByAmt; }

        public float UsrDepositPct { get => DepositPct; set { IsUsrSet = true; DepositPct = value; OnPropertyChanged(nameof(IsSetByAmt)); } }
        public float DepositPct { 
            get => depositAmtPct.DepositPct; 
            set {
                float startAmt = depositAmtPct.DepositAmt;
                depositAmtPct.SetPct(value, TotalDepositAmt); 
                OnPropertyChanged(nameof(UsrDepositPct));
                OnPropertyChanged(nameof(UsrDepositAmt));
                
                if(startAmt != depositAmtPct.DepositAmt) ValueChanged();
            } 
        }
        public float UsrDepositAmt { get => DepositAmt; set { IsUsrSet = true; DepositAmt = value; OnPropertyChanged(nameof(IsSetByAmt)); } }
        public float DepositAmt { 
            get => depositAmtPct.DepositAmt; 
            set {
                float startAmt = depositAmtPct.DepositAmt;
                depositAmtPct.SetAmt(value, TotalDepositAmt);
                OnPropertyChanged(nameof(UsrDepositPct));
                OnPropertyChanged(nameof(UsrDepositAmt));
                if (startAmt != depositAmtPct.DepositAmt) ValueChanged();
            } 
        }
        public void TotatlDepositAmtChanged()
        {
            depositAmtPct.UpdateSumAmt(TotalDepositAmt);
            OnPropertyChanged(nameof(UsrDepositPct));
            OnPropertyChanged(nameof(UsrDepositAmt));
            ValueChanged();
            budgetDebugColor = Color.Black;
        }


        protected float TotalDepositAmt { get => _budget.AssociatedFinancialAccount.isPreTaxAccount ? _deposit.DepositAmount : _deposit.TakeHomeAmount; }
        public abstract float MaxAmt { get; }
        public virtual float MinAmt { get; }



        private Color _budgetDebugColor = Color.Gray;
        private Color budgetDebugColor { set { _budgetDebugColor = value; OnPropertyChanged(nameof(BudgetDebugColor)); } }
        public SolidColorBrush BudgetDebugColor { get => new SolidColorBrush(System.Windows.Media.Color.FromRgb(_budgetDebugColor.R, _budgetDebugColor.G, _budgetDebugColor.B)); }
        private void BudgetError() => budgetDebugColor = Color.Red;

        public ICommand EditBudgetCommand { get; set; }

        public BudgetDepositViewModel(Budget _budget, DepositStore _depositStore)
        {
            this._budget = _budget;
            _deposit = _depositStore;

            depositAmtPct = new AmtPct();


            foreach (Budget childBudget in this._budget.ChildBudgets)
            {
                if (childBudget is FixedBudget) SubItems.Add(new FixedBudgetDepositViewModel(childBudget as FixedBudget, _depositStore));
                else SubItems.Add(new FlexiblebudgetDepositViewModel(childBudget as FlexibleBudget, _depositStore));

                //SubItems[SubItems.Count - 1].BudgetValueChanged += SubItemValueChanged;
                //SubItems[SubItems.Count - 1].EditBudgetAction += (ViewModelBase viewModel) => EditBudgetAction?.Invoke(viewModel);
            }


            EditBudgetCommand = new RelayCommand(execute => EditBudgetAction?.Invoke(this._budget.ToViewModel()));
        }

        public Action<BudgetDepositViewModel> BudgetValueChanged;
        public Action<ViewModelBase> EditBudgetAction;

        public void ValueChanged()
        {
            //Debug.Print(BudgetName);

            if (SubItems.Count > 0) RebalanceSubItems();

            if (IsUsrSet) BudgetValueChanged?.Invoke(this);

        }


        //Only Applies To Fixed And Flexible Budgets
        private void SubItemValueChanged(BudgetDepositViewModel changedBudget)
        {

            /*budgetDebugColor = Color.Gray;

            List<DepositCalculatorBudgetViewModel> usrSetValueBudgets = SubItems.Where(item => item.DepositAmtPct.IsUsrSetValue).ToList();

            float UsrSetSum = usrSetValueBudgets.Sum(item => item.DepositAmtPct.DepositAmount);
            float OtherItemsMinSum = SubItems.Except(usrSetValueBudgets).Sum(item => item.MinAmt);

            if (UsrSetSum + OtherItemsMinSum > budget.MaxMonthlyDeposit)
            {
                if (UsrSetSum + OtherItemsMinSum - changedBudget.DepositAmtPct.DepositAmount > budget.MaxMonthlyDeposit) BudgetError();
                else
                {
                    budgetDebugColor = Color.Orange;
                    changedBudget.DepositAmtPct.SilentSetAmountAndPercent(budget.MaxMonthlyDeposit - (UsrSetSum + OtherItemsMinSum - changedBudget.DepositAmtPct.DepositAmount));
                    RebalanceSubItems();
                }
            }
            else
            {
                if(budget is FlexibleBudget)
                {
                    budgetDebugColor = Color.Purple;
                    if (!DepositAmtPct.IsUsrSetValue)
                    {
                        DepositAmtPct.SilentSetAmountAndPercent(amount: UsrSetSum + OtherItemsMinSum);
                        BudgetValueChanged?.Invoke(this);
                    }
                    else BudgetError();
                }
                else
                {
                    budgetDebugColor = Color.Pink;
                    RebalanceSubItems();
                }

            }*/

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
                budget.TotatlDepositAmtChanged();
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
                availableSum = 0;
            }


            //Distribute Felexible Budgets

            List<BudgetDepositViewModel> savingBudgets = unAssignedBudgets.Where(item => item._budget is SavingsBudget).ToList();
            List<BudgetDepositViewModel> flexibleBudgets = unAssignedBudgets.Where(item => item._budget is FlexibleBudget).ToList();


            if(availableSum >= savingBudgets.Sum(item => item.MaxAmt) + flexibleBudgets.Sum(item => item.MaxAmt))
            {
                budgetDebugColor = Color.Blue;
                availableSum = availableSum - savingBudgets.Sum(item => item.MaxAmt) + flexibleBudgets.Sum(item => item.MaxAmt);
                foreach (BudgetDepositViewModel budget in savingBudgets) { budget.DepositAmt = budget.MaxAmt; unAssignedBudgets.Remove(budget); }
                foreach (BudgetDepositViewModel budget in flexibleBudgets) { budget.DepositAmt = budget.MaxAmt; unAssignedBudgets.Remove(budget); }
            }
            else
            {



                //Put Min Into All Flexible Accounts
                if (availableSum >= savingBudgets.Sum(item => item.MinAmt) + flexibleBudgets.Sum(item => item.MinAmt))
                {
                    budgetDebugColor = Color.Purple;

                    availableSum = availableSum - savingBudgets.Sum(item => item.MinAmt) + flexibleBudgets.Sum(item => item.MinAmt);

                    //Max Out Savings
                    if (availableSum >= 0)
                    {

                        foreach (KeyValuePair<BudgetDepositViewModel, float> keyValue in DistributeAmtAmongBudgets(unAssignedBudgets.Where(budget => budget._budget is SavingsBudget).ToList(), availableSum, (budget) => 1))
                        {
                            keyValue.Key.DepositAmt = keyValue.Key.MinAmt + keyValue.Value;
                            availableSum = availableSum - keyValue.Value;
                            unAssignedBudgets.Remove(keyValue.Key);
                        }
                    }


                    //Max Out Flexible Budget
                    if (availableSum >= 0)
                    {
                        foreach (KeyValuePair<BudgetDepositViewModel, float> keyValue in DistributeAmtAmongBudgets(unAssignedBudgets.Where(budget => budget._budget is FlexibleBudget).ToList(), availableSum, (budget) => 1))
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



            foreach (BudgetDepositViewModel budget in unAssignedBudgets)
            {
                budget.DepositAmt = 0;
                budget.budgetDebugColor = Color.DarkRed;
            }

        }

        private Dictionary<BudgetDepositViewModel, float> DistributeAmtAmongBudgets(List<BudgetDepositViewModel> budgets, float amt, Func<BudgetDepositViewModel, float> ratioWeight)
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

                    if (calculatedAmt + budget.MinAmt >= budget.MaxAmt)
                    {
                        distributedAmts[budget] = budget.MaxAmt - budget.MinAmt;
                        totalUsed += budget.MaxAmt - budget.MinAmt;
                        budgets.Remove(budget);
                        finishedAllocation = false;
                        break;
                    }
                    else distributedAmts[budget] = calculatedAmt;
                }
            }

            return distributedAmts;
        }





    }


    public class AmtPct
    {

        private float depositAmount = 0;
        private float depositPercent = 0;

        public float DepositAmt { get => depositAmount; }
        public float DepositPct { get => depositPercent; }

        private bool isSetByAmt = true;
        public bool IsSetByAmt { get => isSetByAmt; private set { isSetByAmt = value; } }

        public void SetAmt(float amt, float sumAmt)
        {
            IsSetByAmt = true;
            depositAmount = MathF.Max(0, amt);
            depositPercent = depositAmount / sumAmt;
        }

        public void SetPct(float pct, float sumAmt)
        {
            IsSetByAmt = false;
            depositPercent = MathF.Max(0, MathF.Min(1, pct));
            depositAmount = depositPercent * sumAmt;
        }

        public void UpdateSumAmt(float newSum)
        {
            if (isSetByAmt) depositPercent = depositAmount / newSum;
            else depositAmount = depositPercent * newSum;
        }

    }
}
