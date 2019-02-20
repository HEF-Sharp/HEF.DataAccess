using HEF.Data.Query.Internal;
using System.Linq.Expressions;
using System.Reflection;

namespace HEF.Data
{
    public static class ExpressionExtensions
    {
        public static bool IsEntityQueryable(this ConstantExpression constantExpression)
            => constantExpression.Type.GetTypeInfo().IsGenericType
               && constantExpression.Type.GetGenericTypeDefinition() == typeof(EntityQueryable<>);
    }
}
