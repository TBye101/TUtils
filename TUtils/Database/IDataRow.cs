namespace TUtils.Database
{
    /// <summary>
    /// Used to abstract the concept of a row of data between database implementations.
    /// </summary>
    public interface IDataRow
    {
        T GetByColumnIndex<T>(int columnIndex);
    }
}