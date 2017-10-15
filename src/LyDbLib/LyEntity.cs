using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LyDbLib
{
    public class LyEntity<TModel> : IDisposable where TModel : class
    {
        internal SqlBuilder<TModel> m_SqlSb = new SqlBuilder<TModel>();
        /// <summary>
        /// 自動Reset
        /// </summary>
        public bool ResetAuto { get; set; } = true;
        protected LyAdo m_db = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        public LyEntity(string ConnectionString)
        {
            m_db = new LyAdo(ConnectionString);
        }
        /// <summary>
        /// Insert [Output] Values 
        /// </summary>
        public LyEntity<TModel> Insert()
        {
            m_SqlSb.Insert();
            return this;
        }
        /// <summary>
        /// Insert [Output] Values 
        /// </summary>
        public LyEntity<TModel> Values(TModel v)
        {
            m_SqlSb.Values(v);
            return this;
        }
        /// <summary>
        /// Insert [Output] Values 
        /// </summary>
        public LyEntity<TModel> Values<TSet>(TSet v) where TSet : class
        {
            m_SqlSb.Values(v);
            return this;
        }

        /// <summary>
        /// Update Set [Output] [Where] 
        /// </summary>
        public LyEntity<TModel> Update()
        {
            m_SqlSb.Update();
            return this;
        }
        /// <summary>
        /// Update Set [Output] [Where] 
        /// </summary>
        public LyEntity<TModel> Set(TModel v)
        {
            m_SqlSb.Set(v);
            return this;
        }
        /// <summary>
        /// Update Set [Output] [Where] 
        /// </summary>
        public LyEntity<TModel> Set<TSet>(TSet v) where TSet : class
        {
            m_SqlSb.Set(v);
            return this;
        }
        /// <summary>
        /// Insert [Output] Values 
        /// Update Set [Output] [Where] 
        /// </summary>
        public LyEntity<TModel> Output()
        {
            m_SqlSb.Output();
            return this;
        }
        /// <summary>
        /// Delete [Where] 
        /// </summary>
        public LyEntity<TModel> Delete()
        {
            m_SqlSb.Delete();
            return this;
        }
        /// <summary>
        /// Delete [Where] 
        /// </summary>
        public LyEntity<TModel> Delete(Expression<Func<TModel, bool>> predicate)
        {
            m_SqlSb.Delete().Where(predicate);
            return this;
        }
        /// <summary>
        /// Select [Where] [OrderBy]
        /// </summary>
        public LyEntity<TModel> Select()
        {
            m_SqlSb.Select().From();
            return this;
        }
        /// <summary>
        /// Select [Where] [OrderBy]
        /// Update Set [Output] [Where] 
        /// Delete [Where] 
        /// </summary>
        public LyEntity<TModel> Select<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            m_SqlSb.Select(keySelector).From();
            return this;
        }
        /// <summary>
        /// Select [Where] [OrderBy]
        /// </summary>
        public LyEntity<TModel> Where(Expression<Func<TModel, bool>> predicate)
        {
            m_SqlSb.Where(predicate);
            return this;
        }
        /// <summary>
        /// Select [Where] [OrderBy]
        /// </summary>
        public LyEntity<TModel> OrderBy<TKey>(Expression<Func<TModel, TKey>> order)
        {
            m_SqlSb.OrderBy(order);
            return this;
        }

        /// <summary>
        /// Select [Where] [OrderBy]
        /// </summary>
        public LyEntity<TModel> OrderBy<TKey, TDesc>(Expression<Func<TModel, TKey>> order, Expression<Func<TModel, TDesc>> setdesc)
        {
            m_SqlSb.OrderBy(order, setdesc);
            return this;
        }


        /// <summary>查詢</summary>
        public IEnumerable<QModel> Query<QModel>() where QModel : class
        {
            if (ResetAuto)
                ClearParameter();

            if (m_SqlSb.Parameters.Count > 0)
            {
                foreach (var item in m_SqlSb.Parameters)
                {
                    m_db.SetParameter($"{item.Key}", item.Value);
                }
            }
            string sql = m_SqlSb.ToString();
            if (ResetAuto)
                ClearSql();
            return m_db.Query<QModel>(sql);
        }
        /// <summary>查詢</summary>
        public IEnumerable<TModel> Query()
        {
            if (ResetAuto)
                ClearParameter();
            if (m_SqlSb.Parameters.Count > 0)
            {
                foreach (var item in m_SqlSb.Parameters)
                {
                    m_db.SetParameter($"{item.Key}", item.Value);
                }
            }
            string sql = m_SqlSb.ToString();
            if (ResetAuto)
                ClearSql();
            return m_db.Query<TModel>(sql);
        }
        /// <summary>執行SQL Command</summary>
        /// <returns></returns>
        public int Execute()
        {
            if (ResetAuto)
                ClearParameter();
            if (m_SqlSb.Parameters.Count > 0)
            {
                foreach (var item in m_SqlSb.Parameters)
                {
                    m_db.SetParameter($"{item.Key}", item.Value);
                }
            }

            string sql = m_SqlSb.ToString();
            if (ResetAuto)
                ClearSql();
            return m_db.ExecuteCommand(sql);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            m_db.Dispose();
        }

        /// <summary>
        /// For SQL Log
        /// </summary>
        /// <returns></returns>
        public string GetCommandTextAndParameter()
        {
            return m_db.GetCommandTextAndParameter();
        }

        /// <summary>
        /// ClearParameter
        /// </summary>
        public void ClearParameter()
        {
            m_db.ClearParameter();
        }
        /// <summary>
        /// ClearSql
        /// </summary>
        public void ClearSql()
        {
            m_SqlSb.Buffer.Clear();
            m_SqlSb.Parameters.Clear();
        }
    }
}
