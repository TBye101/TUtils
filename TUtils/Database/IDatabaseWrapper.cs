using System;
using System.Collections.Generic;

namespace TUtils.Database
{
    ///<summary>
    ///Used to encapsulate database implementation details.
    ///<summary>
    public interface IDatabaseWrapper
    {
        ///<summary>
        ///Executes the specified query and utilizes the provided function to parse the data into useful objects.
        ///<summary>
        List<ExportedDataType> SelectData<ExportedDataType>(string selectQuery, Func<IDataRow, ExportedDataType> dataParser, params SQLParam[] parameters);

        ///<summary>
        ///Executes a non query script and returns the number of rows affected this way.
        ///The result validator recieves the number of rows affected and returns true if the number of affected rows was within expected parameters.
        ///Rolls the non-query back if the result was not deemed valid.
        ///<summary>
         bool AttemptNonQuery(string nonQuery, Func<int, bool> resultValidator, params SQLParam[] parameters);
    }
}