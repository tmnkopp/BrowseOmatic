using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BOM.CORE
{
    public interface IAppSettingsProvider<T>
    {
        IEnumerable<T> Items { get; } 
    }
    public interface ISettingProvider<T>
    {
        T Get(string ItemName);
    }
}
