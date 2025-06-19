using DupScan.Core.Models;
using DupScan.Core.Services;

namespace DupScan.Orchestration;

public class Orchestrator
{
    private readonly IEnumerable<ScanProvider> _providers;
    private readonly DuplicateDetector _detector;

    public Orchestrator(IEnumerable<ScanProvider> providers, DuplicateDetector detector)
    {
        _providers = providers.ToList();
        _detector = detector;
    }

    public async Task<IReadOnlyList<DuplicateGroup>> RunAsync(bool link)
    {
        var allFiles = new List<FileItem>();
        var map = new Dictionary<FileItem, ScanProvider>();

        foreach (var provider in _providers)
        {
            var files = await provider.Scanner.ScanAsync();
            foreach (var file in files)
            {
                allFiles.Add(file);
                map[file] = provider;
            }
        }

        var groups = _detector.FindDuplicates(allFiles);

        if (link)
        {
            foreach (var group in groups)
            {
                var provider = map[group.Files[0]];
                if (provider.LinkService == null) continue;
                if (group.Files.All(f => map[f] == provider))
                {
                    await provider.LinkService.LinkAsync(group);
                }
            }
        }

        return groups;
    }
}
