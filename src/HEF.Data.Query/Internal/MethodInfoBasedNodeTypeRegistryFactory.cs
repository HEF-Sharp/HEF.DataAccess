using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HEF.Data.Query.Internal
{
    public class MethodInfoBasedNodeTypeRegistryFactory : INodeTypeProviderFactory
    {
        private static readonly object _syncLock = new object();
        private static readonly MethodNameBasedNodeTypeRegistry _methodNameBasedNodeTypeRegistry
            = MethodNameBasedNodeTypeRegistry.CreateFromRelinqAssembly();

        private readonly MethodInfoBasedNodeTypeRegistry _methodInfoBasedNodeTypeRegistry;
        private volatile INodeTypeProvider[] _nodeTypeProviders;

        /// <summary>
        ///     Creates a new <see cref="MethodInfoBasedNodeTypeRegistryFactory" /> that will use the given
        ///     <see cref="MethodInfoBasedNodeTypeRegistry" />
        /// </summary>
        /// <param name="methodInfoBasedNodeTypeRegistry">The registry to use./></param>
        public MethodInfoBasedNodeTypeRegistryFactory(
            MethodInfoBasedNodeTypeRegistry methodInfoBasedNodeTypeRegistry)
        {
            _methodInfoBasedNodeTypeRegistry = methodInfoBasedNodeTypeRegistry
                ?? throw new ArgumentNullException(nameof(methodInfoBasedNodeTypeRegistry));            

            _nodeTypeProviders = new INodeTypeProvider[]
            {
                    _methodInfoBasedNodeTypeRegistry,
                    _methodNameBasedNodeTypeRegistry
            };
        }

        /// <summary>
        ///     Registers methods to be used with the <see cref="INodeTypeProvider" />.
        /// </summary>
        /// <param name="methods">The methods to register.</param>
        /// <param name="nodeType">The node type for these methods.</param>
        public virtual void RegisterMethods(IEnumerable<MethodInfo> methods, Type nodeType)
        {
            if (methods == null)
                throw new ArgumentNullException(nameof(methods));

            if (nodeType == null)
                throw new ArgumentNullException(nameof(nodeType));           

            lock (_syncLock)
            {
                _methodInfoBasedNodeTypeRegistry.Register(methods, nodeType);
                _nodeTypeProviders = new INodeTypeProvider[]
                {
                        _methodInfoBasedNodeTypeRegistry,
                        _methodNameBasedNodeTypeRegistry
                };
            }
        }

        /// <summary>
        ///     Creates a <see cref="INodeTypeProvider" />.
        /// </summary>
        /// <returns>The <see cref="INodeTypeProvider" />.</returns>
        public virtual INodeTypeProvider Create() => new CompoundNodeTypeProvider(_nodeTypeProviders);
    }
}
