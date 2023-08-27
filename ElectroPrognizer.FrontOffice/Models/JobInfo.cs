namespace ElectroPrognizer.FrontOffice.Models;

public class JobInfo
{
    public Type JobType { get; }

    public string JobDescription { get; }

    public JobInfo(Type jobType, string description)
    {
        JobType = jobType;
        JobDescription = description;
    }
}
