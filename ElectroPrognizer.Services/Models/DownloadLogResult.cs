using ElectroPrognizer.DataModel.Entities;

namespace ElectroPrognizer.Services.Models;

public class DownloadLogResult
{
    public int TotalPages { get; set; }

    public DownloadLogEntity[] Entities { get; set; }
}
