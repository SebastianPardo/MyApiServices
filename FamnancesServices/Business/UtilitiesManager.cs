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

        public (DateTime, DateTime) GetPeriodDates(Guid periodId, int dayStart)
        {
            Period period = _context.Period.Single(e => e.Id == periodId);
            DateTime? dateStart = null;
            DateTime? dateEnd = null;
            int dateMonth = 1;
            int dateYear = 1;

            while ((dateStart == null && dateEnd == null) || !(dateStart <= DateTimeEast.Now && dateEnd >= DateTimeEast.Now))
            {
                switch (period.Code)
                {
                    case "MON":
                        dateMonth = DateTimeEast.Now.AddMonths(-1).Month;
                        dateYear = DateTimeEast.Now.Month > dateMonth? DateTimeEast.Now.Year : DateTimeEast.Now.AddYears(-1).Year;
                        
                        dateStart = dateStart == null?
                            DateTime.Parse($"{dateYear}/{dateMonth}/{dayStart}"): dateEnd.Value.AddMinutes(1);
                        
                        dateEnd = dateStart.Value.AddMonths(1).AddMinutes(-1);
                        break;
                    case "SMON":
                        dateMonth = DateTimeEast.Now.AddMonths(-1).Month;
                        dateYear = DateTimeEast.Now.Month > dateMonth ? DateTimeEast.Now.Year : DateTimeEast.Now.AddYears(-1).Year;
                        
                        dateStart = dateStart == null ?
                            DateTime.Parse($"{dateYear}/{dateMonth}/{dayStart}")
                        : dateEnd.Value.AddMinutes(1);

                        dateEnd = DateTime.DaysInMonth(DateTimeEast.Now.Year, DateTimeEast.Now.Month) < 31? 
                            dateStart.Value.AddMonths(1).AddDays(-14).AddMinutes(-1) : 
                            dateStart.Value.AddMonths(1).AddDays(-15).AddMinutes(-1);
                        break;
                    case "BWEEK":
                        dateMonth = DateTimeEast.Now.AddMonths(-1).Month;
                        dateYear = DateTimeEast.Now.Month > dateMonth ? DateTimeEast.Now.Year : DateTimeEast.Now.AddYears(-1).Year;
                        
                        dateStart = dateStart == null ?
                            DateTime.Parse($"{dateYear}/{dateMonth}/{dayStart}")
                        : dateEnd.Value.AddDays(1);

                        dateEnd = dateStart.Value.AddDays(15).AddMinutes(-1);
                        break;
                    case "WEEK":
                        dateMonth = DateTimeEast.Now.AddMonths(-1).Month;
                        dateYear = DateTimeEast.Now.Month > dateMonth ? DateTimeEast.Now.Year : DateTimeEast.Now.AddYears(-1).Year;

                        dateStart = dateStart == null ?
                            DateTime.Parse($"{dateYear}/{dateMonth}/{dayStart}")
                        : dateEnd.Value.AddDays(1);

                        dateEnd = dateStart.Value.AddDays(8).AddMinutes(-1);
                        break;
                    case "DAY":
                        dateMonth = DateTimeEast.Now.AddMonths(-1).Month;
                        dateYear = DateTimeEast.Now.Month > dateMonth ? DateTimeEast.Now.Year : DateTimeEast.Now.AddYears(-1).Year;

                        dateStart = dateStart == null ?
                            DateTime.Parse($"{dateYear}/{dateMonth}/{dayStart}")
                        : dateEnd.Value.AddDays(1).AddMinutes(-1);

                        dateEnd = dateStart.Value.AddDays(1);
                        break;
                }
            }
            return (dateStart.Value, dateEnd.Value);
        }
    }
}
