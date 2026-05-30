using Domain.Entities;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.Integrations.Icu;

namespace Api.Jobs;

public class IcuSyncJob : IJob
{
    private readonly DatabaseContext _context;
    private readonly IIcuClient _icuClient;
    private readonly ILogger<IcuSyncJob> _logger;

    public IcuSyncJob(
        DatabaseContext context,
        IIcuClient icuClient,
        ILogger<IcuSyncJob> logger)
    {
        _context = context;
        _icuClient = icuClient;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing {JobName}", nameof(IcuSyncJob));
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await SyncCommitteeMembers();
            await _context.SaveChangesAsync().ConfigureAwait(false);

            await SyncMembers();
            await _context.SaveChangesAsync().ConfigureAwait(false);

            _logger.LogInformation("{JobName} completed successfully.", nameof(IcuSyncJob));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during {JobName} execution.", nameof(IcuSyncJob));
            throw;
        }
    }

    public async Task SyncCommitteeMembers()
    {
        _logger.LogInformation("Starting committee members sync.");
        var committeeMembers = await _icuClient.GetCommitteeMembersFromIcuAsync();
        _logger.LogDebug("Fetched {Count} committee members from ICU.", committeeMembers.Count);

        var uniqueCommitteeMembers = committeeMembers
            .Where(m => !string.IsNullOrEmpty(m.Cid))
            .GroupBy(m => m.Cid)
            .Select(g => g.First())
            .ToList();

        var committeeMemberCids = uniqueCommitteeMembers.Select(m => m.Cid!).ToList();

        var existingCommitteeMembers = await _context.Users
            .Where(u => committeeMemberCids.Contains(u.Cid!))
            .GroupBy(u => u.Cid)
            .Select(g => g.First())
            .ToDictionaryAsync(u => u.Cid!);

        int updatedCount = 0;
        int addedCount = 0;

        foreach (var m in uniqueCommitteeMembers)
        {
            if (existingCommitteeMembers.TryGetValue(m.Cid!, out var existingCommitteeMember))
            {
                existingCommitteeMember.FullName = $"{m.FirstName} {m.Surname}";
                existingCommitteeMember.EndDate = m.EndDate;
                existingCommitteeMember.PostName = m.PostName;
                existingCommitteeMember.PhoneNo = m.PhoneNo;
                existingCommitteeMember.StartDate = m.StartDate;
                existingCommitteeMember.IsAdmin = true;
                updatedCount++;
            }
            else
            {
                _context.Users.Add(new User
                {
                    Cid = m.Cid,
                    FullName = $"{m.FirstName} {m.Surname}",
                    EndDate = m.EndDate,
                    PostName = m.PostName,
                    PhoneNo = m.PhoneNo,
                    StartDate = m.StartDate,
                    IsAdmin = true,
                });
                addedCount++;
            }
        }
        _logger.LogInformation("Committee members sync finished. Updated: {UpdatedCount}, Added: {AddedCount}", updatedCount, addedCount);
    }

    public async Task SyncMembers()
    {
        _logger.LogInformation("Starting members sync.");
        var members = await _icuClient.GetMembersFromIcuAsync();
        _logger.LogDebug("Fetched {Count} members from ICU.", members.Count);

        var uniqueMembers = members
            .Where(m => !string.IsNullOrEmpty(m.Cid))
            .GroupBy(m => m.Cid)
            .Select(g => g.First())
            .ToList();

        var memberCids = uniqueMembers.Select(m => m.Cid!).ToList();

        var existingMembersDict = await _context.Users
            .Where(u => memberCids.Contains(u.Cid!))
            .GroupBy(u => u.Cid)
            .Select(g => g.First())
            .ToDictionaryAsync(u => u.Cid!);

        int updatedCount = 0;
        int addedCount = 0;

        foreach (var m in uniqueMembers)
        {
            if (existingMembersDict.TryGetValue(m.Cid!, out var existingMember))
            {
                existingMember.FullName = $"{m.FirstName} {m.Surname}";
                existingMember.Email = m.Email;
                existingMember.Login = m.Login;
                existingMember.OrderNo = m.OrderNo;
                existingMember.MemberType = m.MemberType switch
                {
                    "Full" => MemberType.Full,
                    "Life / Associate" => MemberType.Life,
                    _ => null
                };
                updatedCount++;
            }
            else
            {
                _context.Users.Add(new User
                {
                    Cid = m.Cid,
                    FullName = $"{m.FirstName} {m.Surname}",
                    Email = m.Email,
                    Login = m.Login,
                    OrderNo = m.OrderNo,
                    MemberType = m.MemberType switch
                    {
                        "Full" => MemberType.Full,
                        "Life / Associate" => MemberType.Life,
                        _ => null
                    }
                });
                addedCount++;
            }
        }
        _logger.LogInformation("Members sync finished. Updated: {UpdatedCount}, Added: {AddedCount}", updatedCount, addedCount);
    }
}