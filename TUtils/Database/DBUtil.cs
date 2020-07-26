using System;
using System.IO;
using System.Reflection;

namespace TUtils.Database
{
    /// <summary>
    /// Some common utilities for working with the database.
    /// </summary>
    public static class DBUtil
    {
        /// <summary>
        /// Executes a SQL script from the specified resource path as a non query.
        /// Resource paths should follow this form: 'AssemblyName.Namespacename.Filename.Extension'
        /// </summary>
        /// <param name="scriptResourcePath"></param>
        /// <returns></returns>
        public static void LaunchScript(string scriptResourcePath)
        {
            string script = ReadScript(scriptResourcePath);
            DBSingleton.Wrapper.AttemptNonQuery(script, (rowsAffected) => true);
        }

        /// <summary>
        /// Reads a sql script into a string.
        /// </summary>
        /// <param name="scriptResourcePath"></param>
        /// <returns></returns>
        public static string ReadScript(string scriptResourcePath)
        {
            Assembly tbotAssembly = Assembly.GetExecutingAssembly();
            Stream scriptStream = tbotAssembly.GetManifestResourceStream(scriptResourcePath);
            StreamReader reader = new StreamReader(scriptStream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Converts the contents of the data row via casting to their proper objects.
        /// </summary>
        public static T1 CastResults<T1>(IDataRow row)
        {
            return row.GetByColumnIndex<T1>(0);
        }
        
        /// <summary>
        /// Converts the contents of the data row via casting to their proper objects.
        /// </summary>
        public static Tuple<T1, T2> CastResults<T1, T2>(IDataRow row)
        {
            return new Tuple<T1, T2>(row.GetByColumnIndex<T1>(0), row.GetByColumnIndex<T2>(1));
        }

        /// <summary>
        /// Converts the contents of the data row via casting to their proper objects.
        /// </summary>
        public static Tuple<T1, T2, T3> CastResults<T1, T2, T3>(IDataRow row)
        {
            return new Tuple<T1, T2, T3>(row.GetByColumnIndex<T1>(0), row.GetByColumnIndex<T2>(1), row.GetByColumnIndex<T3>(2));
        }
    }
}