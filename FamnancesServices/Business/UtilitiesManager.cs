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

            while ((dateStart == null && dateEnd == null!) || !(dateStart <= DateTime.Now && dateEnd >= DateTime.Now))
            {
                switch (period.Code)
                {
                    case "MON":
                        dateStart = DateTime.Parse($"{DateTime.Now.Year}/{DateTime.Now.Month}/{dayStart}");
                        dateEnd = dateStart.Value.AddMonths(1).AddMinutes(-1);
                        break;
                    case "SMON":
                        dateStart = dateStart == null ?
                            DateTime.Parse($"{DateTime.Now.Year}/{DateTime.Now.Month}/{dayStart}")
                        : dateEnd.Value.AddMinutes(1);

                        dateEnd = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) < 31? 
                            dateStart.Value.AddMonths(1).AddDays(-14).AddMinutes(-1) : 
                            dateStart.Value.AddMonths(1).AddDays(-15).AddMinutes(-1);
                        break;
                    case "BWEEK":
                        dateStart = dateStart == null ?
                            DateTime.Parse($"{DateTime.Now.Year}/{DateTime.Now.Month}/{dayStart}")
                        : dateEnd.Value.AddDays(1);

                        dateEnd = dateStart.Value.AddDays(15).AddMinutes(-1);
                        break;
                    case "WEEK":
                        dateStart = dateStart == null ?
                            DateTime.Parse($"{DateTime.Now.Year}/{DateTime.Now.Month}/{dayStart}")
                        : dateEnd.Value.AddDays(1);

                        dateEnd = dateStart.Value.AddDays(8).AddMinutes(-1);
                        break;
                    case "DAY":
                        dateStart = dateStart == null ?
                            DateTime.Parse($"{dayStart}/{DateTime.Now.Month}/{DateTime.Now.Year}")
                        : dateEnd.Value.AddDays(1).AddMinutes(-1);

                        dateEnd = dateStart.Value.AddDays(1);
                        break;
                }
            }
            return (dateStart.Value, dateEnd.Value);
        }
    }
}
