using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Najafi_Test.Interfaces
{
    public interface IBaseEntity
    {
        Guid Guid { get; set; }
        bool? IsDisabled { get; set; }
    }

    public interface IBaseEntity<T> : IBaseEntity
        where T : IBaseEntity
    {
        bool Validate(ref List<string> Errors);
    }
}
