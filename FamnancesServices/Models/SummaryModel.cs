namespace FamnancesServices.Models
{
    public class SummaryModel
    {
        public decimal PeriodBudget { get; set; }
        public decimal PeriodSpent { get; set; }
        public decimal Chequing { get; set; }
        public decimal Savings { get; set; }
        public decimal PeriodSavingsSpent { get; set; }
        public decimal HomeSavings { get; set; }

        public List<RoommateModel> Roommates { get; set; }
    }

    public class RoommateModel
    {
        public string Name { get; set; }
        public bool IsCurrentUser { get; set; }

        public List<SummaryFixedExpensesModel> SummaryFixedExpenses { get; set; }

        public List<SummaryBudgetModel> SummaryBudgets { get; set; }

        public List<SummaryPocketModel> SummaryPockets { get; set; }
    }

    public class SummaryFixedExpensesModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public bool WasPaid { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }

    public class SummaryBudgetModel
    {
        public Guid Id { get; set; }
        public Guid BudgetPeriodBalanceId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }

    public class SummaryPocketModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Spent { get; set; }
        public decimal Goal { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
