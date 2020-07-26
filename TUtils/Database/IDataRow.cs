namespace TUtils.Database
{
    /// <summary>
    /// Used to abstract the concept of a row of data between database implementations.
    /// </summary>
    internal interface IDataRow
    {
        T GetByColumnIndex<T>(int columnIndex);
    }
}