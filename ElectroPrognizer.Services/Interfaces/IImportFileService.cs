using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IImportFileService
{
    OperationResult Import(List<UploadedFile> uploadedFIles, bool overrideExisting);
}
