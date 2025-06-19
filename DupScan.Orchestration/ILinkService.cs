namespace DupScan.Orchestration;

using DupScan.Core.Models;

public interface ILinkService
{
    Task LinkAsync(DuplicateGroup group);
}
