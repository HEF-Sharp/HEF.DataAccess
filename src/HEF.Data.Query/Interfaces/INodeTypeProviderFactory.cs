using Remotion.Linq.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HEF.Data.Query
{
    public interface INodeTypeProviderFactory
    {
        /// <summary>
        ///     Creates a <see cref="INodeTypeProvider" />.
        /// </summary>
        /// <returns>The <see cref="INodeTypeProvider" />.</returns>
        INodeTypeProvider Create();

        /// <summary>
        ///     Registers methods to be used with the <see cref="INodeTypeProvider" />.
        /// </summary>
        /// <param name="methods">The methods to register.</param>
        /// <param name="nodeType">The node type for these methods.</param>
        void RegisterMethods(IEnumerable<MethodInfo> methods, Type nodeType);
    }
}
