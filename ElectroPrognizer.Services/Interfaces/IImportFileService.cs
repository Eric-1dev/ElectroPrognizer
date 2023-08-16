using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IImportFileService
{
    void Import(IEnumerable<FileData> uploadedFIles, bool overrideExisting);
}
