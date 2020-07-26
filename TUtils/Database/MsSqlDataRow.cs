using System.Data;

namespace TUtils.Database
{
    /// <summary>
    /// Wraps the concept of a single row of data.
    /// </summary>
    public  class MsSqlDataRow : IDataRow
    {
        private readonly object[] Items;

        public MsSqlDataRow(DataRow row)
        {
            this.Items = row.ItemArray;
        }

        public T GetByColumnIndex<T>(int columnIndex)
        {
            return (T)this.Items[columnIndex];
        }
    }
}