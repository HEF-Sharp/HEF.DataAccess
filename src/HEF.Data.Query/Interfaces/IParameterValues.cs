using System.Collections.Generic;

namespace HEF.Data.Query
{
    public interface IParameterValues
    {
        IReadOnlyDictionary<string, object> ParameterValues { get; }
        
        void AddParameter(string name, object value);
       
        object RemoveParameter(string name);

        void SetParameter(string name, object value);
    }
}
