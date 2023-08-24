using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ElectroPrognizer.Services.Hubs;

public class StatusHub : Hub
{
    public IUploadService UploadService { get; set; }

    public override async Task OnConnectedAsync()
    {
        UploadService.SendStatus();

        await base.OnConnectedAsync();
    }

    public void CancelUpload()
    {
        UploadService.Cancel();
    }
}
