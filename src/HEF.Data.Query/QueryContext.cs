using System;
using System.Collections.Generic;

namespace HEF.Data.Query
{
    public class QueryContext : IDisposable, IParameterValues
    {
        private readonly IDictionary<string, object> _parameterValues = new Dictionary<string, object>();

        public QueryContext(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual DbContext DbContext { get; }

        public virtual IReadOnlyDictionary<string, object> ParameterValues
            => (IReadOnlyDictionary<string, object>)_parameterValues;

        /// <summary>
        ///     Adds a parameter.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="value"> The value. </param>
        public virtual void AddParameter(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            _parameterValues.Add(name, value);
        }

        /// <summary>
        ///     Sets a parameter value.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="value"> The value. </param>
        public virtual void SetParameter(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            _parameterValues[name] = value;
        }

        /// <summary>
        ///     Removes a parameter by name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The parameter value.
        /// </returns>
        public virtual object RemoveParameter(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var value = _parameterValues[name];

            _parameterValues.Remove(name);

            return value;
        }

        public virtual void Dispose()
        {

        }
    }
}
