using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Models
{
    public class FixedBudget : Budget
    {
        public override string BudgetType { get => "Fixed"; }

        public bool IsSetByAmount { get => isSetByAmount; set => isSetByAmount = value; }
        public float SetAmount { get => setAmount; set => setAmount = value; }
        public float SetPercent { get => setPercent; set => setPercent = value; }


        private bool isSetByAmount = true;
        private float setAmount = 0;
        private float setPercent = 0;


        public FixedBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, float setAmt = 0f, float setPct = 0.0f, List<int>? childBudgets = null) : base(id, name, priority, associatedFinancialAccount, childBudgets)
        {
            isSetByAmount = setPct != 0.0f ? false : true;
            setAmount = setAmt;
            setPercent = setPct;
        }

        public float GetSetAmount(float referenceAmount) => isSetByAmount ? setAmount : setPercent * referenceAmount;

        public override float MinDepositAmount(float referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override float MaxDepositAmount(float referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override float RecommendedDepositAmount(float referenceDeposit, int numMonths = 1) => GetSetAmount(referenceDeposit);

        public override ViewModelBase ToViewModel() => new FixedBudgetViewModel(this);
    }
}
