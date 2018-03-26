using System;

namespace HEF.Data
{
    public interface IDbContextOptionsModule
    {
        Type ServiceType { get; }

        object Instance { get; }
    }
}
