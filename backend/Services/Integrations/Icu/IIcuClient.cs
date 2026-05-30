using Services.Integrations.Icu.Models;

namespace Services.Integrations.Icu;

/// <summary>
/// The interface for the Imperial College Union (ICU) client.
/// </summary>
public interface IIcuClient
{
    /// <summary>
    /// Gets the ICMC committee members from the ICU client.
    /// </summary>
    Task<List<IcuCommitteeMember>> GetCommitteeMembersFromIcuAsync();

    /// <summary>
    /// Gets the ICMC members from the ICU client.
    /// </summary>
    Task<List<IcuMember>> GetMembersFromIcuAsync();
}