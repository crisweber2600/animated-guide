namespace DupScan.Orchestration;

public record ScanProvider(IScanner Scanner, ILinkService? LinkService);
