using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteAdapter
{
    public enum SQLiteSpecificErrors
    {
        SQLITE_OK = 0,   /* Successful result */
        /* beginning-of-error-codes */
        SQLITE_ERROR = 1,   /* Generic error */
        //SQLite recerved codes 0-101 in ver 3
        SQLITE_BADSTATE = 1000
    }
}
