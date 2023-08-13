using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IImportFileService
{
    void Import(List<FileData> uploadedFIles, bool overrideExisting);
}
