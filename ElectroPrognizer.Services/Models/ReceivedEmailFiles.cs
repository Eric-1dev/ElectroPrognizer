using MailKit;

namespace ElectroPrognizer.Services.Models;

public class ReceivedEmailFiles
{
    public UniqueId MailId { get; set; }

    public FileData[] FileDatas { get; set; }
}
