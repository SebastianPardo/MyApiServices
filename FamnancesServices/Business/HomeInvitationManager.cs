using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class HomeInvitationManager : IHomeInvitationManager
    {
        DatabaseContext context;
        public HomeInvitationManager(DatabaseContext context)
        {
            this.context = context;
        }

        public List<HomeInvitation>? GetInvitations(string email)
        {
            return context.HomeInvitation.Include(e => e.Guest).ThenInclude(ee => ee.User).Where(i => i.Guest.Email == email && i.IsInvitation == true && i.InvitedAceptedDate == null).ToList();
        }

        public List<HomeInvitation>? GetRequests(Guid homeId)
        {
            return context.HomeInvitation.Include(e => e.Guest).ThenInclude(ee => ee.User).Where(i => i.HomeId == homeId && i.IsInvitation == false && i.InvitedAceptedDate == null).ToList();
        }

        public HomeInvitation GetById(Guid id)
        {
            return context.HomeInvitation.FirstOrDefault(i => i.Id == id);
        }

        public bool Add(HomeInvitation homeInvitation)
        {
            context.HomeInvitation.Add(homeInvitation);
            return context.SaveChanges() > 0;
        }

        public HomeInvitation Update(HomeInvitation homeInvitation)
        {
            homeInvitation = context.HomeInvitation.Update(homeInvitation).Entity;
            context.SaveChanges();
            return homeInvitation;
        }

        public bool Delete(HomeInvitation homeInvitation)
        {
            context.HomeInvitation.Remove(homeInvitation);
            return context.SaveChanges() > 0;
        }

        public HomeInvitation Accept(Guid invitationId)
        {
            var invitation = context.HomeInvitation.FirstOrDefault(e => e.Id == invitationId);
            invitation.InvitedAceptedDate = DateTime.Now;

            context.HomeInvitation.Update(invitation);

            context.HomeInvitation.RemoveRange(context.HomeInvitation.Where(e => e.InvitedAceptedDate == null));
            context.SaveChanges();
            return invitation;
        }
    }
}