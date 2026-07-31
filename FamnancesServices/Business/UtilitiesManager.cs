using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class UtilitiesManager : IUtilitiesManager
    {
        DatabaseContext _context;

        public UtilitiesManager(DatabaseContext context)
        {
            _context = context;
        }

        public (DateTime, DateTime) GetPeriodDates(Guid periodId, int dayStart, DateTime? date = null)
        {
            date = date?? DateTimeEast.Now;

            Period period = _context.Period.Single(e => e.Id == periodId);
            
            DateTime dateStart = DateTimeEast.Now.AddMonths(-1);
            if (dateStart.Month == 2 && dayStart == 30)
                dayStart = 28;
            dateStart = DateTime.Parse($"{dateStart.Year}/{dateStart.Month}/{dayStart}");
            
            DateTime? dateEnd = null;

            while (dateEnd == null || !(dateStart <= date&& dateEnd >= date))
            {
                switch (period.Code)
                {
                    case "MON":
                        dateStart = dateEnd == null? dateStart : dateEnd.Value.AddMinutes(1);                        
                        dateEnd = dateStart.AddMonths(1).AddMinutes(-1);
                        break;
                    case "SMON":
                        dateStart = dateEnd == null ? dateStart : dateEnd.Value.AddMinutes(1);
                        dateEnd = DateTime.DaysInMonth(DateTimeEast.Now.Year, DateTimeEast.Now.Month) < 31? 
                            dateStart.AddMonths(1).AddDays(-14).AddMinutes(-1) : 
                            dateStart.AddMonths(1).AddDays(-15).AddMinutes(-1);
                        break;
                    case "BWEEK":
                        dateStart = dateEnd == null ? dateStart : dateEnd.Value.AddDays(1);
                        dateEnd = dateStart.AddDays(15).AddMinutes(-1);
                        break;
                    case "WEEK":
                        dateStart = dateEnd == null ? dateStart : dateEnd.Value.AddDays(1);
                        dateEnd = dateStart.AddDays(8).AddMinutes(-1);
                        break;
                    case "DAY":
                        dateStart = dateEnd == null ? dateStart : dateEnd.Value.AddDays(1).AddMinutes(-1);
                        dateEnd = dateStart.AddDays(1);
                        break;
                }
            }
            return (dateStart, dateEnd.Value);
        }
    }
}
