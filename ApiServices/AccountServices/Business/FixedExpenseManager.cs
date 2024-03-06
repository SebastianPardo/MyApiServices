﻿using AccountServices.Business.Interfaces;
using OhMyMoney.DataCore.Data;
using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business
{
    public class FixedExpenseManager : IFixedExpenseManager
    {
        DatabaseContext context;
        public FixedExpenseManager(DatabaseContext context)
        {
            this.context = context;
        }

        public FixedExpense Add(FixedExpense fixedExpense)
        {
            fixedExpense = context.FixedExpense.Add(fixedExpense).Entity;
            context.SaveChanges();
            return fixedExpense;
        }

        public bool Delete(FixedExpense fixedExpense)
        {
            context.FixedExpense.Remove(fixedExpense);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<FixedExpense> GetAllByUserId(Guid userId)
        {
            return context.FixedExpense.Where(fe => fe.UserId == userId);
        }

        public FixedExpense GetById(Guid id)
        {
            return context.FixedExpense.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(FixedExpense fixedExpense)
        {
            context.FixedExpense.Update(fixedExpense);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }
    }
}
