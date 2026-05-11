using FinancialCalculator.ViewModels;

namespace FinancialCalculator.Models
{
    public class FixedBudget : Budget
    {
        public override string BudgetType { get => "Fixed"; }

        public bool IsSetByAmount { get => isSetByAmount; set => isSetByAmount = value; }
        public decimal SetAmount { get => setAmount; set => setAmount = value; }
        public decimal SetPercent { get => setPercent; set => setPercent = value; }


        private bool isSetByAmount = true;
        private decimal setAmount = 0;
        private decimal setPercent = 0;


        public FixedBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, decimal setAmt = 0m, decimal setPct = 0m, List<int>? childBudgets = null) : base(id, name, priority, associatedFinancialAccount, childBudgets)
        {
            isSetByAmount = setPct != 0.0m ? false : true;
            setAmount = setAmt;
            setPercent = setPct;
        }

        public decimal GetSetAmount(decimal referenceAmount) => isSetByAmount ? setAmount : setPercent * referenceAmount;

        public override decimal MinDepositAmount(decimal referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override decimal MaxDepositAmount(decimal referenceDeposit, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override decimal RecommendedDepositAmount(decimal referenceDeposit, int numMonths = 1) => GetSetAmount(referenceDeposit);

        public override ViewModelBase ToViewModel() => new FixedBudgetViewModel(this);
    }
}
