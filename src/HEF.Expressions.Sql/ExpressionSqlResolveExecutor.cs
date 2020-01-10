using HEF.Entity.Mapper;
using HEF.Sql;
using HEF.Sql.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HEF.Expressions.Sql
{
    public class ExpressionSqlResolveExecutor : ExpressionVisitor
    {
        private static string paramNamePrefix = "p";
        private int paramCount = 0;

        private IDictionary<TypeAndValue, SqlParameter> sqlParamMap = new Dictionary<TypeAndValue, SqlParameter>();

        #region 私有结构定义
        struct TypeAndValue : IEquatable<TypeAndValue>
        {
            readonly Type type;
            readonly object value;
            readonly int hash;

            public TypeAndValue(Type type, object value)
            {
                this.type = type;
                this.value = value;
                hash = type.GetHashCode() + (value != null ? value.GetHashCode() : 0);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TypeAndValue))
                    return false;

                return Equals((TypeAndValue)obj);
            }

            public bool Equals(TypeAndValue vt)
            {
                return vt.type == type && Equals(vt.value, value);
            }

            public override int GetHashCode()
            {
                return hash;
            }
        }
        #endregion   

        public ExpressionSqlResolveExecutor(IEntityMapperProvider mapperProvider,
            IEntitySqlFormatter sqlFormatter,
            IMethodCallSqlResolver methodCallSqlResolver,
            Expression expression)
        {
            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            MethodCallSqlResolver = methodCallSqlResolver ?? throw new ArgumentNullException(nameof(methodCallSqlResolver));

            ToResolveExpression = expression ?? throw new ArgumentNullException(nameof(expression));

            SqlStrings = new StringBuilder();
            ResolveSqlSentence = Execute(ToResolveExpression);
        }

        public IEntityMapperProvider MapperProvider { get; }

        public IEntitySqlFormatter SqlFormatter { get; }

        public IMethodCallSqlResolver MethodCallSqlResolver { get; }

        public Expression ToResolveExpression { get; }

        #region Resolve Result
        protected StringBuilder SqlStrings { get; }

        protected SqlParameter[] SqlParameters => sqlParamMap.Select(m => m.Value).ToArray();

        public SqlSentence ResolveSqlSentence { get; }
        #endregion

        protected virtual SqlSentence Execute(Expression expression)
        {
            expression = PartialEvaluator.Eval(expression);

            Visit(expression);

            return new SqlSentence(SqlStrings.ToString(), SqlParameters);
        }

        #region Visit Expression
        public override Expression Visit(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (IsResolveNodeType(expression))
                return base.Visit(expression);

            throw new NotSupportedException($"The LINQ expression node of type {expression.NodeType} is not supported");
        }

        protected virtual bool IsResolveNodeType(Expression expression)
        {
            return expression.IsLambda()
                || expression.IsMethodCall()
                || expression.IsMemberAccess()
                || expression.IsConstant()
                || expression.IsParameter()
                || expression.IsQuote()
                || expression.IsLogicOperation()
                || expression.IsCompareOperation()
                || expression.IsMathOperation();
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.IsQuote())
            {
                Visit(node.Operand);
                return node;
            }

            throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", node.NodeType));
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var binaryOperator = GetBinaryOperator(node);

            Expression leftExpr = node.Left;
            Expression rightExpr = node.Right;

            Write("(");
            switch (node.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    goto case ExpressionType.Add;
                case ExpressionType.Equal:
                    if (IsNullConstant(rightExpr))
                    {
                        Visit(leftExpr);
                        Write(" is null");
                        break;
                    }
                    else if (IsNullConstant(leftExpr))
                    {
                        Visit(rightExpr);
                        Write(" is null");
                        break;
                    }
                    goto case ExpressionType.LessThan;
                case ExpressionType.NotEqual:
                    if (IsNullConstant(rightExpr))
                    {
                        Visit(leftExpr);
                        Write(" is not null");
                        break;
                    }
                    else if (IsNullConstant(leftExpr))
                    {
                        Visit(rightExpr);
                        Write(" is not null");
                        break;
                    }
                    goto case ExpressionType.LessThan;
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    // check for special x.CompareTo(y) && type.Compare(x,y)
                    if (leftExpr.NodeType == ExpressionType.Call && IsZeroConstant(rightExpr))
                    {
                        MethodCallExpression mc = (MethodCallExpression)leftExpr;                        
                        if (IsIComparableMethod(mc))  //x.CompareTo(y)
                        {
                            leftExpr = mc.Object;
                            rightExpr = mc.Arguments[0];
                        }
                        else if (IsStaticCompareMethod(mc))  //type.Compare(x,y)
                        {
                            leftExpr = mc.Arguments[0];
                            rightExpr = mc.Arguments[1];
                        }
                    }
                    goto case ExpressionType.Add;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                    Visit(leftExpr);
                    Write(" ");
                    Write(binaryOperator);
                    Write(" ");
                    Visit(rightExpr);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", node.NodeType));
            }
            Write(")");

            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (MethodCallSqlResolver.VisitMethodCall(node, Write, Visit))
                return node;
            
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", node.Method.Name));
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            //判断是否引用表达式参数对象的属性
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                var propertyMap = GetPropertyMap(node.Member);
                var columnName = SqlFormatter.ColumnName(propertyMap); //获取实体属性对应column名称

                Write(columnName);
                return node;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            WriteConstant(node.Type, node.Value);
            return node;
        }

        #region Helper Functions
        protected virtual IPropertyMap GetPropertyMap(MemberInfo member)
        {
            var entityMapper = MapperProvider.GetEntityMapper(member.DeclaringType);

            return entityMapper.Properties.Single(m => string.Compare(m.PropertyInfo.Name, member.Name) == 0);
        }

        protected virtual string GetBinaryOperator(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return (IsBoolean(expression.Left.Type)) ? "and" : "&";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return (IsBoolean(expression.Left.Type) ? "or" : "|");
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                default:
                    return "";
            }
        }

        protected virtual bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        protected virtual bool IsPredicate(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return IsBoolean(((BinaryExpression)expr).Type);
                case ExpressionType.Not:
                    return IsBoolean(((UnaryExpression)expr).Type);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return true;
                case ExpressionType.Call:
                    return IsBoolean(((MethodCallExpression)expr).Type);
                default:
                    return false;
            }
        }

        protected virtual bool IsNullConstant(Expression expr)
        {
            return expr.NodeType == ExpressionType.Constant && ((ConstantExpression)expr).Value == null;
        }

        protected virtual bool IsZeroConstant(Expression expr)
        {
            if (expr.NodeType != ExpressionType.Constant)
                return false;

            var constExpr = (ConstantExpression)expr;

            return constExpr.Value != null && constExpr.Value.GetType() == typeof(int) && ((int)constExpr.Value) == 0;
        }

        protected virtual bool IsIComparableMethod(MethodCallExpression expr)
        {
            return expr.Method.Name == "CompareTo" && !expr.Method.IsStatic && expr.Arguments.Count == 1;
        }

        protected virtual bool IsStaticCompareMethod(MethodCallExpression expr)
        {
            return (expr.Method.DeclaringType == typeof(string) || expr.Method.DeclaringType == typeof(decimal))
                && expr.Method.Name == "Compare" && expr.Method.IsStatic && expr.Arguments.Count == 2;
        }

        protected virtual void Write(object value)
        {
            SqlStrings.Append(value);
        }

        protected virtual void WriteConstant(Type constantType, object value)
        {
            if (value == null)
            {
                Write("null");
                return;
            }

            var valueType = value.GetType();
            if (valueType.IsEnum)
            {
                Write(Convert.ChangeType(value, Enum.GetUnderlyingType(valueType)));
                return;
            }
            
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Boolean:
                    Write(((bool)value) ? 1 : 0);
                    break;
                case TypeCode.Single:
                case TypeCode.Double:
                    Write(Convert.ToDouble(value).ToString("0.0"));
                    break;
                case TypeCode.DateTime:
                case TypeCode.String:
                case TypeCode.Object:
                    SqlParameter sqlParam;
                    TypeAndValue typeValue = new TypeAndValue(constantType, value);
                    if (!sqlParamMap.TryGetValue(typeValue, out sqlParam))
                    {   // re-use same name-value if same type & value
                        string name = paramNamePrefix + (paramCount++);
                        sqlParam = new SqlParameter(name, value);
                        sqlParamMap.Add(typeValue, sqlParam);
                    }
                    Write(SqlFormatter.Parameter(sqlParam.ParameterName));   //写入参数名
                    break;
                default:
                    Write(value);
                    break;
            }
        }
        #endregion

        #endregion
    }
}
