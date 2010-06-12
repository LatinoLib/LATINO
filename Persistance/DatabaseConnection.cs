/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    DatabaseConnection.cs
 *  Desc:    Database connection object
 *  Created: Mar-2007
 * 
 *  Authors: Miha Grcar
 * 
 ***************************************************************************/

using System;
using System.Data;
using System.Data.OleDb;

namespace Latino.Persistance
{
    /* .-----------------------------------------------------------------------
       |
       |  Enum DatabaseType
       |
       '-----------------------------------------------------------------------
    */
    public enum DatabaseType
    {
        SqlServer7,
        SqlServer2000,
        SqlServer2005,
        SqlServer2008
    }

    /* .-----------------------------------------------------------------------
       |
       |  Class DatabaseConnection
       |
       '-----------------------------------------------------------------------
    */
    public class DatabaseConnection
    {
        private OleDbConnection mConnection
            = null;
        private OleDbTransaction mTransaction
            = null;
        private string mConnectionString
            = "Provider=SQLOLEDB;Data Source=${server};User ID=${username};Password=${password};Initial Catalog=${database}";
        private string mUsername
            = "sa";
        private string mPassword
            = "";
        private string mDatabase
            = "";
        private string mServer
            = "(local)";

        public string ConnectionString
        {
            get { return mConnectionString; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("ConnectionString") : null);
                mConnectionString = value;
            }
        }

        public string Username
        {
            get { return mUsername; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Username") : null);
                mUsername = value;
            }
        }

        public string Password
        {
            get { return mPassword; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Password") : null);
                mPassword = value;
            }
        }

        public string Database
        {
            get { return mDatabase; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Database") : null);
                mDatabase = value;
            }
        }

        public string Server
        {
            get { return mServer; }
            set
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Server") : null);
                mServer = value;
            }
        }

        public void SetConnectionString(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer7:
                case DatabaseType.SqlServer2000:
                    mConnectionString = "Provider=SQLOLEDB;Data Source=${server};User ID=${username};Password=${password};Initial Catalog=${database}";
                    break;
                case DatabaseType.SqlServer2005:
                    mConnectionString = "Provider=SQLNCLI;Server=${server};Database=${database};Uid=${username};Pwd=${password}";
                    break;
                case DatabaseType.SqlServer2008:
                    mConnectionString = "Provider=SQLNCLI10;Server=${server};Database=${database};Uid=${username};Pwd=${password}";
                    break;
                default:
                    throw new ArgumentNotSupportedException("dbType");
            }            
        }

        public void Connect()
        {
            Utils.ThrowException(mConnection != null ? new InvalidOperationException() : null);
            string connectionString = mConnectionString.Replace("${username}", mUsername).Replace("${password}", mPassword)
                .Replace("${database}", mDatabase).Replace("${server}", mServer);
            Utils.VerboseLine("Connecting with {0} ...", connectionString);
            try
            {
                mConnection = new OleDbConnection(connectionString);
                //mConnection.StateChange += new StateChangeEventHandler(ConnectionStateChange);
                mConnection.Open(); // throws OleDbException
            }
            catch (Exception e)
            {
                Utils.VerboseLine("Not successful: {0}", e);
                throw; 
            }
            Utils.VerboseLine("Success.");
        }

        public void Disconnect()
        {
            Utils.ThrowException(mConnection == null ? new InvalidOperationException() : null);
            Utils.VerboseLine("Disconnecting ...");
            mConnection.Close();
            mConnection = null;
            Utils.VerboseLine("Success.");
        }

        //private void ConnectionStateChange(object sender, StateChangeEventArgs args)
        //{
        //    Utils.VerboseLine("New state: " + args.CurrentState);
        //}

        public void StartTransaction()
        {
            Utils.ThrowException((mConnection == null || mTransaction != null) ? new InvalidOperationException() : null);
            Utils.VerboseLine("Starting transaction.");
            mTransaction = mConnection.BeginTransaction();
        }

        public void Commit()
        {
            Utils.ThrowException((mConnection == null || mTransaction == null) ? new InvalidOperationException() : null);
            Utils.VerboseLine("Committing transaction.");
            mTransaction.Commit(); 
            mTransaction = null;
        }

        public void Rollback()
        {
            Utils.ThrowException((mConnection == null || mTransaction == null) ? new InvalidOperationException() : null);
            Utils.VerboseLine("Rollbacking transaction.");
            mTransaction.Rollback();
            mTransaction = null;
        }

        private static void AssignParamsToCommand(OleDbCommand command, object[] args)
        {
            foreach (object arg in args)
            {
                OleDbParameter param = command.CreateParameter();
                param.Value = arg == null ? System.DBNull.Value : arg;
                command.Parameters.Add(param);
            }
        }

        public bool ExecuteNonQuery(string sqlStatement, params object[] args)
        {
            Utils.ThrowException(mConnection == null ? new InvalidOperationException() : null);
            Utils.ThrowException(sqlStatement == null ? new ArgumentNullException("sqlStatement") : null);
            Utils.ThrowException(args == null ? new ArgumentNullException("args") : null);
            Utils.VerboseLine("Executing non-query ...");
            try
            {
                OleDbCommand command = new OleDbCommand(sqlStatement, mConnection);
                if (mTransaction != null) { command.Transaction = mTransaction; }
                AssignParamsToCommand(command, args);
                int rowsAffected = command.ExecuteNonQuery();
                Utils.VerboseLine("Success. {0} rows affected.", rowsAffected);
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                Utils.VerboseLine("Not successful: {0}", e);
                throw;
            }
        }

        public DataTable ExecuteQuery(string sqlSelectStatement, params object[] args)
        {
            Utils.ThrowException(mConnection == null ? new InvalidOperationException() : null);
            Utils.ThrowException(sqlSelectStatement == null ? new ArgumentNullException("sqlSelectStatement") : null);
            Utils.ThrowException(args == null ? new ArgumentNullException("args") : null);
            Utils.VerboseLine("Executing query ...");
            try
            {
                OleDbCommand command = new OleDbCommand(sqlSelectStatement, mConnection);
                if (mTransaction != null) { command.Transaction = mTransaction; }
                AssignParamsToCommand(command, args);
                DataTable dataTable;
                new OleDbDataAdapter(command).Fill(dataTable = new DataTable());
                Utils.VerboseLine("Success.");
                return dataTable;
            }
            catch (Exception e)
            {
                Utils.VerboseLine("Not successful: {0}", e);
                throw;
            }
        }
    }
}
