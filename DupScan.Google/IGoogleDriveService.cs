using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace DupScan.Google;

public interface IGoogleDriveService
{
    Task<IList<GoogleFile>> ListFilesAsync();
}
