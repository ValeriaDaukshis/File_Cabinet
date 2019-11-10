using FileCabinetApp.Records;

namespace FileCabinetApp.Service
{
    public interface IRecordIterator<T>
    {
        T Current();

        bool MoveNext();
    }
}