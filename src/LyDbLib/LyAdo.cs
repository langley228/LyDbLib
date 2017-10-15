using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LyDbLib
{
    public class LyAdo : IDisposable
    {
        protected CommittableTransaction ct = null;
        protected string m_ConnectionString;
        protected SqlConnection m_Conn;
        protected SqlCommand m_Cmd;
        protected SqlDataReader m_Dr;
        protected List<SqlParameter> m_Parameters = new List<SqlParameter>();
        protected string m_SqlString;

        #region DbWork
        public LyAdo(string ConnectionString)
        {
            m_ConnectionString = ConnectionString;
            Init();
        }
        private void Init()
        {
            m_Conn = new SqlConnection(m_ConnectionString);
            m_Cmd = new SqlCommand();
            m_Cmd.Connection = m_Conn;
            m_Conn.Open();
        }
        #endregion
        

        #region Query
        /// <summary>查詢</summary>
        /// <typeparam name="TModel">資料型別</typeparam>
        /// <returns></returns>
        public IEnumerable<TModel> Query<TModel>()
        {
            m_Cmd.Parameters.Clear();
            m_Cmd.CommandText = this.m_SqlString;
            foreach (SqlParameter item in this.m_Parameters)
            {
                m_Cmd.Parameters.Add(item);
            }


            m_Dr = (SqlDataReader)m_Cmd.ExecuteReader();

            bool bRead = m_Dr.Read();
            bool bReturn = false;
            Type type = typeof(TModel);
            PropertyInfo[] ptops = type.GetProperties();
            TModel model = default(TModel);
            do
            {
                if (!bRead)
                    m_Dr.Close();

                if (bReturn)
                {
                    bReturn = false;
                    yield return model;
                }

                if (bRead)
                {
                    model = System.Activator.CreateInstance<TModel>();
                    for (int i = 0; i < m_Dr.FieldCount; i++)
                    {
                        PropertyInfo propertyInfo = ptops.FirstOrDefault(m => m.Name == m_Dr.GetName(i));
                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(model, m_Dr[propertyInfo.Name] == DBNull.Value ? null : m_Dr[propertyInfo.Name]);
                        }
                    }
                    bReturn = true;
                    bRead = m_Dr.Read();
                }
            } while (bRead || bReturn);
        }

        /// <summary>查詢</summary>
        /// <typeparam name="TModel">資料型別</typeparam>
        /// <param name="sql">SQL 查詢語法<</param>
        /// <returns></returns>
        public IEnumerable<TModel> Query<TModel>(string sql)
        {
            this.m_SqlString = sql;
            return Query<TModel>();
        }

        /// <summary>查詢</summary>
        /// <typeparam name="TModel">資料型別</typeparam>
        /// <param name="sql">SQL 查詢語法<</param>
        /// <param name="parameters">SQL 參數</param>
        /// <returns></returns>
        public IEnumerable<TModel> Query<TModel>(string sql, List<SqlParameter> parameters)
        {
            this.m_SqlString = sql;
            this.m_Parameters = parameters ?? new List<SqlParameter>();
            return Query<TModel>();
        }
        #endregion

        #region ExecuteCommand
        /// <summary>執行SQL Command</summary>
        /// <returns></returns>
        public int ExecuteCommand()
        {
            m_Cmd.Parameters.Clear();
            m_Cmd.CommandText = this.m_SqlString;
            foreach (SqlParameter item in this.m_Parameters)
            {
                m_Cmd.Parameters.Add(item);
            }
            return m_Cmd.ExecuteNonQuery();
        }

        /// <summary>執行SQL Command</summary>
        /// <param name="sql">SQL 查詢語法<</param>
        /// <returns></returns>
        public int ExecuteCommand(string sql)
        {
            this.m_SqlString = sql;
            return this.ExecuteCommand();
        }

        /// <summary>執行SQL Command</summary>
        /// <param name="sql">SQL 查詢語法<</param>
        /// <param name="parameters">SQL 參數</param>
        /// <returns></returns>
        public int ExecuteCommand(string sql, List<SqlParameter> parameters)
        {
            this.m_SqlString = sql;
            this.m_Parameters = parameters ?? new List<SqlParameter>();
            return this.ExecuteCommand();
        }
        #endregion

        #region Open
        /// <summary>開啟連線</summary>
        public void Open()
        {
            if (m_Conn == null) m_Conn = new SqlConnection(m_ConnectionString);
            if (m_Cmd == null)
            {
                m_Cmd = new SqlCommand();
                m_Cmd.Connection = m_Conn;
            }
            if (m_Conn != null && m_Conn.State == System.Data.ConnectionState.Closed)
                m_Conn.Open();
        }

        #endregion

        #region Close
        /// <summary>關閉連線</summary>
        public void Close()
        {
            if (m_Conn != null && m_Conn.State == System.Data.ConnectionState.Open)
            {
                m_Conn.Close();
                m_Conn.Dispose();
            }
        }

        #endregion

        #region Parameter
        /// <summary>設定SQL參數</summary>
        /// <param name="name">參數名稱</param>
        /// <param name="value">參數值</param>
        public void SetParameter(string name, object value)
        {
            SqlParameter parameter = this.m_Parameters.Where(m => m.ParameterName == name).FirstOrDefault();
            if (parameter != null)
                parameter.Value = value ?? DBNull.Value;
            else
                this.m_Parameters.Add(new SqlParameter(name, value ?? DBNull.Value));
        }

        /// <summary>清除所有SQL參數</summary>
        public void ClearParameter()
        {
            m_Parameters = new List<SqlParameter>();
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (m_Cmd != null)
                m_Cmd.Dispose();
            if (m_Conn != null)
                m_Conn.Dispose();
        }
        #endregion

        #region BeginTransaction

        public void BeginTransaction()
        {
            ct = new CommittableTransaction();

            try
            {
                m_Conn.EnlistTransaction(ct);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Commit

        public void Commit()
        {
            if (ct != null)
            {
                ct.Commit();
            }

        }

        #endregion

        #region Rollback
        /// <summary>
        /// Rollback
        /// </summary>
        public void Rollback()
        {

            if (ct != null)
            {
                ct.Rollback();
            }

        }

        #endregion

        #region GetModelString 取得用來建立 Model 的字串
        /// <summary>取得用來建立 Model 的字串</summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetModelString(string sql)
        {
            this.m_SqlString = sql;
            return GetModelString();
        }

        /// <summary>取得用來建立 Model 的字串</summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GetModelString(string sql, List<SqlParameter> parameters)
        {
            this.m_SqlString = sql;
            this.m_Parameters = parameters ?? new List<SqlParameter>();
            return GetModelString();
        }

        /// <summary>取得用來建立 Model 的字串</summary>
        /// <returns></returns>
        public string GetModelString()
        {
            StringBuilder sb = new StringBuilder();
            m_Cmd.Parameters.Clear();
            m_Cmd.Parameters.AddRange(this.m_Parameters.ToArray());
            m_Cmd.CommandType = CommandType.Text;
            m_Cmd.CommandText = this.m_SqlString;
            m_Dr = (SqlDataReader)m_Cmd.ExecuteReader();
            for (int i = 0; i < m_Dr.FieldCount; i++)
            {
                sb.AppendLine(string.Format("public {0} {1} {{ get; set; }}", m_Dr.GetFieldType(i).Name, m_Dr.GetName(i)));
            }
            return sb.ToString();
        }
        #endregion


        /// <summary>
        /// For SQL Log
        /// </summary>
        /// <returns></returns>
        public string GetCommandTextAndParameter()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.m_SqlString);
            if (m_Parameters != null && m_Parameters.Count > 0)
            {
                sb.AppendLine("parameter ==============");
                foreach (var parameter in m_Parameters)
                {
                    sb.AppendLine($"{parameter.ParameterName} = {parameter.Value}");
                }
            }
            return sb.ToString();
        }
    }
}
