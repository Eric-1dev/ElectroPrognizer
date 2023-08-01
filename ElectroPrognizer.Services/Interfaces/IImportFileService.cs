using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IImportFileService
{
    void Import(List<UploadedFile> uploadedFIles, bool overrideExisting);
}
