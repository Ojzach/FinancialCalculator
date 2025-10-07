using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Models
{
    public class FixedBudget : Budget
    {
        public override string BudgetType { get => "Fixed"; }

        private AmountPercentModel setAmountPercent = new AmountPercentModel();
        public AmountPercentModel SetAmountPercent { get => setAmountPercent; }


        public FixedBudget(int id, string name, FinancialAccount associatedFinancialAccount, float setAmt = 0f, float setPct = 0.0f) : base(id, name, associatedFinancialAccount)
        {
            if(setAmt != 0.0f) setAmountPercent.Amount = setAmt;
            else if(setPct != 0.0f) setAmountPercent.Percent = setPct;
        }

        public override float MinDepositAmount(float referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override float MaxDepositAmount(float referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override float RecommendedDepositAmount(float referenceDeposit, int numMonths = 1) => SetAmountPercent.GetAmount(referenceDeposit) * numMonths;

        public override ViewModelBase ToViewModel() => new FixedBudgetViewModel(this);
    }
}
