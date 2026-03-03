using Famnances.DataCore.Entities;
using System;
using System.Net.Sockets;

namespace FamnancesServices.Models
{
    public class SummaryModel
    {
        public bool ToBeClosed { get; set; }
        public DateTime PeriodStartDate { get; set; } = DateTime.Now;
        public DateTime PeriodEndDate { get; set; } = DateTime.Now;
        public decimal PeriodBudget { get; set; } = 0;
        public decimal PeriodBalance { get; set; } = 0;
        public decimal PeriodExpenses { get; set; } = 0;
        public decimal HomeSavings { get; set; } = 0;
        public decimal Chequing { get; set; } = 0;
        public decimal Savings { get; set; } = 0;
        public decimal PeriodSavingsExpeses { get; set; } = 0;

        public List<RoommateModel> Roommates { get; set; }
    }

    public class RoommateModel
    {
        public string Name { get; set; }
        public bool IsCurrentUser { get; set; }

        public List<SummaryBudgetModel> SummaryBudgets { get; set; }
        public List<SummaryPocketModel> SummaryPockets { get; set; }
        public List<SummaryFixedExpensesModel> SummaryFixedExpenses { get; set; }
    }

    public class SummaryBudgetModel
    {
        public Guid Id { get; set; }
        public Guid BudgetBalanceId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
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

    public class SummaryPocketModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Spent { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
