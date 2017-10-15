
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LyDbLib
{
    internal class SqlBuilder<TModel> :DbExtensions.SqlBuilder where TModel : class
    {
        private Dictionary<string, object> m_Parameters = new Dictionary<string, object>();

        public Dictionary<string, object> Parameters
        {
            get { return m_Parameters; }
        }

        private string m_PName = "EX";
        public SqlBuilder() : base() { }
        public SqlBuilder(string PName) : base() { m_PName = PName; }
        public SqlBuilder<TModel> OrderBy<TKey>(Expression<Func<TModel, TKey>> order)
        {
            this.ORDER_BY(string.Join(", ", GetExpressionName(order.Body)));
            return this;
        }
        public SqlBuilder<TModel> OrderBy<TKey, TDesc>(Expression<Func<TModel, TKey>> order, Expression<Func<TModel, TDesc>> setdesc)
        {
            IEnumerable<string> setdescx = GetExpressionName(setdesc.Body);
            IEnumerable<string> orderx = GetExpressionName(order.Body).Select(m =>
            {
                if (setdescx.Any(c => c == m))
                    return $"{m} Desc";
                else
                    return m;
            });
            this.ORDER_BY(string.Join(", ", orderx));
            return this;
        }
        public SqlBuilder<TModel> Delete()
        {
            this.DELETE_FROM(typeof(TModel).Name);
            return this;
        }

        public SqlBuilder<TModel> Delete<TSelect>() where TSelect : class
        {
            this.DELETE_FROM(typeof(TSelect).Name);
            return this;
        }
        public SqlBuilder<TModel> Update()
        {
            Type type = typeof(TModel);
            PropertyInfo[] props = type.GetProperties();
            this.UPDATE($"{typeof(TModel).Name}");
            return this;
        }
        public SqlBuilder<TModel> Set(TModel v)
        {
            int iCount1 = this.m_Parameters.Count;
            int iCount2 = this.m_Parameters.Count;
            Type type = typeof(TModel);
            PropertyInfo[] props = type.GetProperties();
            foreach (var item in props)
            {
                this.m_Parameters.Add($"{m_PName}{iCount1++}", item.GetValue(v));
            }
            this.AppendLine()
                .Append($"Set {string.Join($", ", props.Select(m => $"{m.Name}=@{m_PName}{iCount2++}" ))}");
            return this;
        }
        public SqlBuilder<TModel> Set<TSet>(TSet setValue) where TSet : class
        {
            int iCount1 = this.m_Parameters.Count;
            int iCount2 = this.m_Parameters.Count;
            Type typeM = typeof(TModel);
            Type typeS = typeof(TSet);
            PropertyInfo[] propsM = typeM.GetProperties();
            PropertyInfo[] propsS = typeS.GetProperties().Where(m => propsM.Any(x => x.Name == m.Name)).ToArray();
            foreach (var item in propsS)
            {
                this.m_Parameters.Add($"{m_PName}{iCount1++}", item.GetValue(setValue));
            }
            this.AppendLine()
                .Append($"Set {string.Join($", ", propsS.Select(m => $"{m.Name}=@{m_PName}{iCount2++}"))}");
            return this;
        }
        public SqlBuilder<TModel> Insert()
        {
            Type type = typeof(TModel);
            PropertyInfo[] props = type.GetProperties();
            this.INSERT_INTO($"{typeof(TModel).Name} ({string.Join(", ", props.Select(m => m.Name))})");
            return this;
        }
        public SqlBuilder<TModel> Output()
        {
            this.AppendLine()
                .Append($"OUTPUT INSERTED.* ");
            return this;
        }

        public SqlBuilder<TModel> Values<TSet>(TSet setValue) where TSet : class
        {
            int iCount1 = this.m_Parameters.Count;
            int iCount2 = this.m_Parameters.Count;
            Type typeM = typeof(TModel);
            Type typeS = typeof(TSet);
            PropertyInfo[] propsM = typeM.GetProperties();
            PropertyInfo[] propsS = typeS.GetProperties().Where(m => propsM.Any(x => x.Name == m.Name)).ToArray();
            foreach (var item in propsS)
            {
                this.m_Parameters.Add($"{m_PName}{iCount1++}", item.GetValue(setValue));
            }
            this.AppendLine()
                .Append($"VALUES (@{m_PName}{string.Join($", @{m_PName}", propsS.Select(m => iCount2++))})");
            return this;
        }
        public SqlBuilder<TModel> Values(TModel v)
        {
            int iCount1 = this.m_Parameters.Count;
            int iCount2 = this.m_Parameters.Count;
            Type type = typeof(TModel);
            PropertyInfo[] props = type.GetProperties();
            foreach (var item in props)
            {
                this.m_Parameters.Add($"{m_PName}{iCount1++}", item.GetValue(v));
            }
            this.AppendLine()
                .Append($"VALUES (@{m_PName}{string.Join($", @{m_PName}", props.Select(m => iCount2++))})");
            return this;
        }

        public SqlBuilder<TModel> Select<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            string s = string.Join(", ", GetExpressionName(keySelector.Body));
            this.SELECT(s);
            return this;
        }
        public SqlBuilder<TModel> Select()
        {
            Type type = typeof(TModel);
            string s = string.Join(", ", type.GetProperties().Select(m => m.Name));
            this.SELECT(s);
            return this;
        }
        public SqlBuilder<TModel> Select<TSelect>() where TSelect : class
        {
            Type type = typeof(TSelect);
            string s = string.Join(", ", type.GetProperties().Select(m => m.Name));
            this.SELECT(s);
            return this;
        }
        public SqlBuilder<TModel> From()
        {
            this.FROM(typeof(TModel).Name);
            return this;
        }
        public SqlBuilder<TModel> From<TSelect>()
        {
            this.FROM(typeof(TSelect).Name);
            return this;
        }

        public SqlBuilder<TModel> Where(Expression<Func<TModel, bool>> predicate)
        {
            this.WHERE(GetExpressionWhere(predicate.Body));
            return this;
        }

        private IEnumerable<string> GetExpressionName(Expression expression)
        {
            if (expression is NewExpression)
            {
                NewExpression newExpression = expression as NewExpression;
                foreach (var item in newExpression.Members)
                {
                    yield return typeof(TModel).GetProperty(item.Name).Name;
                }
            }
            else if (expression is ParameterExpression)
            {
                foreach (var item in expression.Type.GetProperties())
                {
                    //欄位
                    yield return typeof(TModel).GetProperty(item.Name).Name;
                }
            }
            else
            {

            }
        }
        private string GetExpressionSet(Expression expression)
        {
            if (expression is BinaryExpression)
            {
                //還是比較 And OR
                string oper, left, right;
                BinaryExpression binaryExpression = expression as BinaryExpression;
                left = this.GetExpressionSet(binaryExpression.Left as Expression);
                right = this.GetExpressionSet(binaryExpression.Right as Expression);

                //=NULL 換成 IS NULL !=NULL 換成 IS NOT NULL
                if (expression.NodeType == ExpressionType.Equal && right == "NULL")
                {
                    oper = " IS ";
                }
                else if (expression.NodeType == ExpressionType.NotEqual && right == "NULL")
                {
                    oper = " IS NOT ";
                }
                else
                {
                    oper = GetOperator(expression.NodeType);
                }
                return string.Format("({0}{1}{2})", left, oper, right);
            }
            else if (expression is MemberExpression)
            {
                MemberExpression memberExpression = expression as MemberExpression;

                if (memberExpression.Expression is ParameterExpression)
                {
                    //欄位
                    return typeof(TModel).GetProperty(memberExpression.Member.Name).Name;
                }
            }
            return GetExpressionValue(expression);
        }
        private string GetExpressionWhere(Expression expression)
        {
            if (expression is BinaryExpression)
            {
                //還是比較 And OR
                string oper, left, right;
                BinaryExpression binaryExpression = expression as BinaryExpression;
                left = this.GetExpressionWhere(binaryExpression.Left as Expression);
                right = this.GetExpressionWhere(binaryExpression.Right as Expression);

                //=NULL 換成 IS NULL !=NULL 換成 IS NOT NULL
                if (expression.NodeType == ExpressionType.Equal && right == "NULL")
                {
                    oper = " IS ";
                }
                else if (expression.NodeType == ExpressionType.NotEqual && right == "NULL")
                {
                    oper = " IS NOT ";
                }
                else
                {
                    oper = GetOperator(expression.NodeType);
                }
                return string.Format("({0}{1}{2})", left, oper, right);
            }
            else if (expression is MemberExpression)
            {
                MemberExpression memberExpression = expression as MemberExpression;

                if (memberExpression.Expression is ParameterExpression)
                {
                    //欄位
                    return typeof(TModel).GetProperty(memberExpression.Member.Name).Name;
                }
            }
            return GetExpressionValue(expression);
        }
        private string GetExpressionValue(Expression expression)
        {
            int iCount = this.m_Parameters.Count;
            if (expression is ConstantExpression)
            {
                //直接設值的Expression
                var ce = expression as ConstantExpression;
                this.m_Parameters.Add($"{m_PName}{iCount}", ce.Value);
                return $"@{ this.m_Parameters.Last().Key}";
            }
            else if (expression is UnaryExpression)
            {
                //表示有一元 (Unary) 運算子的運算式
                UnaryExpression ue = expression as UnaryExpression;

                if (ue.Operand is MemberExpression)
                {
                    //取屬性值
                    MemberExpression me = ue.Operand as MemberExpression;
                    if (me.Type == typeof(DateTime))
                    {
                        //DateTime.Now 直接用SQL的語法
                        if (me.Member.Name == "Now")
                        {
                            return $"GETDATE()";
                        }
                        else if (me.Member.Name == "UtcNow")
                        {
                            return $"GETUTCDATE()";
                        }
                    }
                    this.m_Parameters.Add($"{m_PName}{iCount}", Expression.Lambda(me).Compile().DynamicInvoke());
                    return $"@{ this.m_Parameters.Last().Key}";
                }
                else
                {
                    this.m_Parameters.Add($"{m_PName}{iCount}", Expression.Lambda(ue.Operand).Compile().DynamicInvoke());
                    return $"@{ this.m_Parameters.Last().Key}";
                }
            }
            this.m_Parameters.Add($"{m_PName}{iCount}", Expression.Lambda(expression).Compile().DynamicInvoke());
            return $"@{ this.m_Parameters.Last().Key}";
        }


        /// <summary>
        /// 取得操作子
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.OrElse:
                    return " OR ";
                default:
                    throw new ArgumentException("不支援的Where操作");
            }
        }
    }
}
