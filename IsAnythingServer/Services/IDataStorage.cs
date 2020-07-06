namespace IsAnythingServer.Services
{
    public interface IDataStorage
    {
        bool? ReadRecord(string subject, string predicate);
        bool WriteRecord(string subject, string predicate, bool value);
    }
}
