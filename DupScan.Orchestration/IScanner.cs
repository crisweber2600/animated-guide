namespace DupScan.Orchestration;

using DupScan.Core.Models;

public interface IScanner
{
    Task<IReadOnlyList<FileItem>> ScanAsync();
}
