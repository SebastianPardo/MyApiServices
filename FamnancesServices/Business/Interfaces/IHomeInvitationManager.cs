using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IHomeInvitationManager
    {
        List<HomeInvitation>? GetRequests(Guid homeId);
        HomeInvitation GetById(Guid id);
        List<HomeInvitation>? GetInvitations(string email);
        bool Add(HomeInvitation homeInvitation);
        HomeInvitation Update(HomeInvitation homeInvitation);
        bool Delete(HomeInvitation homeInvitation);
        HomeInvitation Accept(Guid invitationId);
    }
}
