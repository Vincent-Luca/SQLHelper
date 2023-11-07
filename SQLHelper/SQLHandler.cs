using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.CodeDom;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;

namespace SQLHelper
{
    public class SQLHandler : IDisposable
    {
        private string _connectionString;
        public string ConnectionString { get { return _connectionString; } set { _connectionString = value; } }

        private readonly bool _resetConString;
        public bool ResetConString => _resetConString;

        private  SqlConnection _connection;
        private SqlCommand _command;
        private SqlDataReader _reader;

        public SQLHandler()
        {
            _resetConString = false;
        }
        public SQLHandler(bool resetConnectionString)
        {
            _resetConString = resetConnectionString;
        }

        public SQLHandler(SqlConnection Connection)
        {
            _connection = Connection;
            _resetConString = false;
        }

        public SQLHandler(string ConnectionString) 
        {
            this.ConnectionString = ConnectionString;
            _resetConString = false;
        }

        private void ResetVals()
        {
            if (_resetConString)
            {
                _connectionString = string.Empty;
            }

            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }

            _connection = null;
            _command = null;
            _reader = null;
        }

        private void SetVals(string conString, string sqlCommand, CommandType commandType)
        {
            try
            {
                _connectionString = conString;
                _connection = new SqlConnection(_connectionString);
                _command = new SqlCommand();
                _command.Connection = _connection;
                _command.CommandType = commandType;
                _command.CommandText = sqlCommand;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            
        }

        private void SetParams(Dictionary<string, dynamic> parameters)
        {
            if (_command == null)
            {
                throw new NullReferenceException("Commander was null");
            }
            if (_command.Connection.State == ConnectionState.Open)
            {
                throw new Exception();
            }
            if (parameters.Count == 0)
            {
                return;
            }


            foreach (KeyValuePair<string,dynamic> pair in parameters)
            {
                _command.Parameters.AddWithValue(pair.Key, pair.Value);
            }
        }

        public DataTable SqlGetData(string conString, string sqlCommand, Dictionary<string,dynamic> parameters, CommandType commandType)
        {
            if (!SubServices.ValidateString(conString))
            {
                throw new ArgumentNullException(conString, "Given Connectionstring was empty or null");
            }
            if (!SubServices.ValidateString(sqlCommand))
            {
                throw new ArgumentNullException(sqlCommand, "Given sqlCommand was empty or null");
            }

            try
            {
                SetVals(conString, sqlCommand, commandType);

                if (_command.Connection.State == ConnectionState.Open)
                {
                    throw new Exception("Connection to sql Server was still open");
                }

                if (parameters != null)
                {
                    SetParams(parameters);
                }
                return InteralRead();

            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        private DataTable InteralRead()
        {
            try
            {
                DataTable result = new DataTable();
                
                _connection.Open();

                using (_reader = _command.ExecuteReader())
                {
                    for (int i = 0; i < _reader.FieldCount; i++)
                    {
                        Type t = _reader.GetFieldType(i);
                        string fieldName = _reader.GetName(i);

                        DataColumn column = new DataColumn(fieldName, t);
                        result.Columns.Add(column);
                    }

                    while (_reader.Read())
                    {
                        DataRow dr = result.NewRow();
                        for (int i = 0; i < _reader.FieldCount; i++)
                        {
                            dr[result.Columns[i].ColumnName] = _reader.GetValue(i);
                        }
                        result.Rows.Add(dr);
                    }
                }

                return result;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            finally
            {
                _connection.Close();
                ResetVals();
            }
        }

        public DataTable SqlGetData(string sqlCommand, Dictionary<string, dynamic> parameters, CommandType commandType)
        {
            return SqlGetData(_connectionString, sqlCommand, parameters, commandType);
        }

        public DataTable SqlGetData(string conString, string sqlCommand, Dictionary<string, dynamic> parameters)
        {
            return SqlGetData(conString, sqlCommand, parameters, CommandType.Text);
        }

        public DataTable SqlGetData(string sqlCommand, Dictionary<string, dynamic> parameters)
        {
            return SqlGetData(_connectionString, sqlCommand, parameters, CommandType.Text);
        }

        public DataTable SqlGetData(string conString, string sqlCommand, CommandType commandType)
        {
            return SqlGetData(conString, sqlCommand, null, commandType);
        }

        public DataTable SqlGetData(string sqlCommand, CommandType commandType)
        {
            return SqlGetData(_connectionString, sqlCommand, null, commandType);
        }

        public DataTable SqlGetData(string conString, string sqlCommand)
        {
            return SqlGetData(conString, sqlCommand, null, CommandType.Text);
        }

        public DataTable SqlGetData(string sqlCommand)
        {
            return SqlGetData(_connectionString, sqlCommand, null, CommandType.Text);
        }

        public T GetSingleResult<T>(string conString, string sqlCommand, Dictionary<string, dynamic> parameters, CommandType commandType)
        {
            if (!SubServices.ValidateString(conString))
            {
                throw new ArgumentNullException(conString, "Given Connectionstring was empty or null");
            }
            if (!SubServices.ValidateString(sqlCommand))
            {
                throw new ArgumentNullException(sqlCommand, "Given sqlCommand was empty or null");
            }

            try
            {
                SetVals(conString, sqlCommand, commandType);

                if (_connection.State == ConnectionState.Open)
                {
                    throw new Exception("Connection to sql Server was still open");
                }
                if (parameters != null)
                {
                    SetParams(parameters);
                }
                return InteralRead<T>();

            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        private T InteralRead<T>()
        {
            try
            {
                DataTable result = new DataTable();

                _connection.Open();

                object returnVal = _command.ExecuteScalar();
                if (returnVal == null)
                {
                    return default;
                }
                else
                {
                    return (T)returnVal;
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            finally
            {
                _connection.Close();
                ResetVals();
            }
        }

        public T GetSingleResult<T>(string sqlCommand, Dictionary<string, dynamic> parameters, CommandType commandType)
        {
            return GetSingleResult<T>(_connectionString, sqlCommand, parameters, commandType);
        }

        public T GetSingleResult<T>(string conString, string sqlCommand, Dictionary<string, dynamic> parameters)
        {
            return GetSingleResult<T>(conString, sqlCommand, parameters, CommandType.Text);
        }

        public T GetSingleResult<T>(string sqlCommand, Dictionary<string, dynamic> parameters)
        {
            return GetSingleResult<T>(_connectionString, sqlCommand, parameters, CommandType.Text);
        }

        public T GetSingleResult<T>(string conString, string sqlCommand, CommandType commandType)
        {
            return GetSingleResult<T>(conString, sqlCommand, null, commandType);
        }

        public T GetSingleResult<T>(string sqlCommand, CommandType commandType)
        {
            return GetSingleResult<T>(_connectionString, sqlCommand, null, commandType);
        }

        public T GetSingleResult<T>(string conString, string sqlCommand)
        {
            return GetSingleResult<T>(conString, sqlCommand, null, CommandType.Text);
        }

        public T GetSingleResult<T>(string sqlCommand)
        {
            return GetSingleResult<T>(_connectionString, sqlCommand, null, CommandType.Text);
        }

        public void Dispose()
        {
            ResetVals();

            _command.Dispose();
            _connection.Dispose();
            _reader.Close();
        }

        public void ExecuteNonQuerySql(string conString, string sqlCommand, Dictionary<string, dynamic> parameters, CommandType commandType)
        {
            if (!SubServices.ValidateString(conString))
            {
                throw new ArgumentNullException(conString, "Given Connectionstring was empty or null");
            }
            if (!SubServices.ValidateString(sqlCommand))
            {
                throw new ArgumentNullException(sqlCommand, "Given sqlCommand was empty or null");
            }
            

            try
            {
                SetVals(conString, sqlCommand, commandType);

                if (_connection.State == ConnectionState.Open)
                {
                    throw new Exception("Connection to sql Server was still open");
                }

                if (parameters != null)
                {
                    SetParams(parameters);
                }
                internalNonQuery();

            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        private void internalNonQuery()
        {
            try
            {
                _connection.Open();
                _command.ExecuteReader();
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            finally 
            {
                _connection.Close();
                ResetVals();
            }
        }

        public void ExecuteNonQuerySql(string sqlCommand, Dictionary<string, dynamic> parameters, CommandType commandType)
        {
            ExecuteNonQuerySql(_connectionString, sqlCommand, parameters, commandType);
        }

        public void ExecuteNonQuerySql(string conString, string sqlCommand, CommandType commandType)
        {
            ExecuteNonQuerySql(conString, sqlCommand, null, commandType);
        }

        public void ExecuteNonQuerySql(string conString, string sqlCommand, Dictionary<string, dynamic> parameters)
        {
            ExecuteNonQuerySql(conString, sqlCommand, parameters, CommandType.Text);
        }

        public void ExecuteNonQuerySql(string sqlCommand, CommandType commandType)
        {
            ExecuteNonQuerySql(_connectionString, sqlCommand, null, commandType);
        }

        public void ExecuteNonQuerySql(string sqlCommand, Dictionary<string, dynamic> parameters)
        {
            ExecuteNonQuerySql(_connectionString, sqlCommand, parameters, CommandType.Text);
        }

        public void ExecuteNonQuerySql(string conString, string sqlCommand)
        {
            ExecuteNonQuerySql(conString, sqlCommand, null, CommandType.Text);
        }

        public void ExecuteNonQuerySql(string sqlCommand)
        {
            ExecuteNonQuerySql(_connectionString, sqlCommand, null, CommandType.Text);
        }
    }
}
