using HEF.Entity.Mapper;
using HEF.Sql;
using HEF.Sql.Entity;
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
        public ExpressionSqlResolveExecutor(IEntityMapperProvider mapperProvider,
            IEntitySqlFormatter sqlFormatter, Expression expression)
        {
            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));

            if (sqlFormatter == null)
                throw new ArgumentNullException(nameof(sqlFormatter));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            MapperProvider = mapperProvider;
            SqlFormatter = sqlFormatter;
            ToResolveExpression = expression;

            SqlStrings = new StringBuilder();
            SqlParameters = new List<SqlParameter>();
            ResolveSqlSentence = Execute(ToResolveExpression);
        }

        public IEntityMapperProvider MapperProvider { get; }

        public IEntitySqlFormatter SqlFormatter { get; }

        public Expression ToResolveExpression { get; }

        #region Resolve Result
        protected StringBuilder SqlStrings { get; }

        protected IList<SqlParameter> SqlParameters { get; }

        public SqlSentence ResolveSqlSentence { get; }
        #endregion

        protected virtual SqlSentence Execute(Expression expression)
        {
            Visit(expression);

            return new SqlSentence(SqlStrings.ToString(), SqlParameters.ToArray());
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
            return expression.IsLambda() ||
                expression.IsMethodCall() ||
                expression.IsMemberAccess() ||
                expression.IsConstant() ||
                expression.IsParameter() ||
                expression.IsLogicOperation() ||
                expression.IsCompareOperation() ||
                expression.IsMathOperation();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var binaryOperator = GetBinaryOperator(node);

            Expression leftExpr = node.Left;
            Expression rightExpr = node.Right;

            return base.VisitBinary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
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
            return base.VisitConstant(node);
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
                    return "and";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "or";
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

        protected virtual void Write(object value)
        {
            SqlStrings.Append(value);
        }
        #endregion

        #endregion
    }
}
