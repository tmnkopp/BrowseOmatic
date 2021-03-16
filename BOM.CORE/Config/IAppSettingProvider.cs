using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BOM.CORE
{
    public interface IAppSettingProvider<T>
    {
        IEnumerable<T> Items { get; } 
    }
}
