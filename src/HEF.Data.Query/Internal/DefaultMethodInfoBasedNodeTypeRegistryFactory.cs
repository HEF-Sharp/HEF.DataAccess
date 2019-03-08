using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace HEF.Data.Query.Internal
{
    public class DefaultMethodInfoBasedNodeTypeRegistryFactory : MethodInfoBasedNodeTypeRegistryFactory
    {
        public DefaultMethodInfoBasedNodeTypeRegistryFactory()
            : base(MethodInfoBasedNodeTypeRegistry.CreateFromRelinqAssembly())
        { }
    }
}
