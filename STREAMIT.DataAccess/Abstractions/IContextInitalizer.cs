using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Abstractions
{
    public interface IContextInitalizer
    {
        Task InitDatabaseAsync();
    }
}
