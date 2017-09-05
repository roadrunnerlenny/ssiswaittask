using System;
using System.ComponentModel;
using System.Threading;
using ALE.WaitTask.Helper;
using Microsoft.SqlServer.Dts.Runtime;
using System.Xml;
using System.Data.SqlClient;
using DtsWrap = Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Data.OleDb;
using System.Data;

namespace ALE.WaitTask
{
#if VS2008
    [DtsTask(
        DisplayName = "A.LE Wait Task",
        Description = "Waits until a specific time or a data event",
        TaskContact = "A.LE Wait Task (ssis.andreaslennartz.de)",
        IconResource = "ALE.WaitTask.clock_2008.ico",
        UITypeName = "ALE.WaitTask.WaitTaskUI, ALE.WaitTask, Version=1.0.0.2008, Culture=neutral, PublicKeyToken=8b31d41c0e44001c"
    )]
#else
    [DtsTask(
        DisplayName = "A.LE Wait Task",
        Description = "Waits until a specific time or a data event",
        TaskContact = "A.LE Wait Task (ssis.andreaslennartz.de)",
        IconResource = "ALE.WaitTask.clock.ico",
        UITypeName = "ALE.WaitTask.WaitTaskUI, ALE.WaitTask, Version=1.0.0.2012, Culture=neutral, PublicKeyToken=8b31d41c0e44001c"
    )]
#endif
    public class WaitTaskMain : Task, IDTSComponentPersist
    {
        #region Properties

        internal Time _waitUntilTimeInternal;
        internal Time WaitUntilTimeInternal
        {
            get
            {
                return _waitUntilTimeInternal;
            }
            set
            {
                _waitUntilTimeInternal = value;
            }
        }

        internal bool HasWaitUntilTime
        {
            get
            {
                return WaitUntilTimeInternal != null;
            }
        }

        [Category("Task state")]
        public bool DoNothingAndContinue
        {
            get;
            set;
        }

        [Category("Wait Time")]
        public string WaitUntilTime
        {
            get
            {
                if (WaitUntilTimeInternal != null)
                    return WaitUntilTimeInternal.ToString();
                else
                    return String.Empty;
            }
            set
            {
                WaitUntilTimeInternal = Time.ParseHourMinutesTimeString(value.Trim());
            }
        }

        internal bool HasSleepTime
        {
            get
            {
                return SleepTimeInMinutes != null;
            }
        }

        [Category("Wait Time")]
        public int? SleepTimeInMinutes
        {
            get;
            set;
        }

        internal bool HasSQLCheckStatement
        {
            get
            {
                if (String.IsNullOrEmpty(SQLCheckStatement) || SQLCheckStatement.Trim().Length == 0)
                    return false;
                else
                    return true;
            }
        }

        [Category("SQL")]
        public string SQLCheckStatement
        {
            get;
            set;
        }

        internal bool HasSQLConnection
        {
            get
            {
                if (String.IsNullOrEmpty(SQLConnectionID) || SQLConnectionID.Trim().Length == 0)
                    return false;
                else
                    return true;

            }
        }

        internal string SQLConnectionID
        {
            get;
            set;
        }

        [Category("SQL")]
        public string SQLConnection
        {
            get
            {
                if (HasSQLConnection && Conns != null)
                {
                    return GetConnectionManager(Conns).ConnectionString;
                }
                else
                    return String.Empty;
            }
        }

        internal bool HasSQLRepetitionFrequencyInMinutes
        {
            get
            {
                return SQLRepetitionFrequencyInMinutes != null && SQLRepetitionFrequencyInMinutes > 0;
            }
        }

        [Category("SQL")]
        public int? SQLRepetitionFrequencyInMinutes
        {
            get;
            set;
        }

        internal bool HasMaximumSQLWaitTime
        {
            get
            {
                return MaximumSQLWaitTime != null && MaximumSQLWaitTime > 0;
            }
        }

        [Category("SQL")]
        public int? MaximumSQLWaitTime
        {
            get;
            set;
        }

        #endregion

        public WaitTaskMain()
        { }

        internal IDTSComponentEvents Events { get; set; }
        internal Connections Conns { get; set; }
        internal object Transaction { get; set; }

        #region Task overrides

        public override void InitializeTask(Connections connections, VariableDispenser variableDispenser, IDTSInfoEvents events, IDTSLogging log, EventInfos eventInfos, LogEntryInfos logEntryInfos, ObjectReferenceTracker refTracker)
        {
            Conns = connections;

            base.InitializeTask(connections, variableDispenser, events, log, eventInfos, logEntryInfos, refTracker);
        }

        public override DTSExecResult Validate(Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log)
        {
            Events = componentEvents;
            Conns = connections;

            DTSExecResult execResult = base.Validate(connections, variableDispenser, componentEvents, log);
            if (execResult == DTSExecResult.Success)
            {
                if (!DoNothingAndContinue)
                {
                    if (HasSQLCheckStatement && !HasSQLConnection)
                    {
                        FireError("An Sql check statement is defined, aber no connection exists. Please select a valid ADO.NET or OLEDB connection or leave the sql check statement blank.");
                    }
                    if (HasSQLConnection && HasSQLCheckStatement && !HasMaximumSQLWaitTime)
                    {
                        FireWarning("A Sql check statement and connection exists, but no maximum wait time was entered. The default of 24h is used.");
                    }
                    if (HasSQLConnection && HasSQLCheckStatement && !HasSQLRepetitionFrequencyInMinutes)
                    {
                        FireWarning("A Sql check statement and connection exists, but no Sql repetition frequency was entered. The SQL statement will be fired on the database every second. This can cause performance issues.");
                    }
                }
            }

            return execResult;
        }

        public override DTSExecResult Execute(Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log, object transaction)
        {
            Events = componentEvents;
            Conns = connections;
            Transaction = transaction;

            DTSExecResult execResult = DTSExecResult.Success;

            if (!DoNothingAndContinue)
            {
                try
                {
                    SleepUntilWaitTime();
                    SleepSomeMinutes();
                    WaitForSQL();
                }
                catch
                {
                    execResult = DTSExecResult.Failure;
                }
            }
            else
            {
                FireInformation("DoNothingAndContinueExecution is set to true - task will do nothing and continue.");
            }
            return execResult;
        }

        #endregion

        private ConnectionManager GetConnectionManager(Connections connections)
        {
            ConnectionManager connMan = null;
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].ID.Equals(SQLConnectionID))
                    connMan = connections[i];
            }
            if (connMan == null)
            {
                FireError("Cannot find connection " + SQLConnection + " with ID " + SQLConnectionID + ".");
            }
            return connMan;
        }

        private void SleepUntilWaitTime()
        {
            if (HasWaitUntilTime)
            {
                FireInformation("Execution is suspended until " + WaitUntilTime + "h (24-h format)...");
                Thread.Sleep(Time.CalculateTimeToWait(WaitUntilTimeInternal));
            }
        }

        private void SleepSomeMinutes()
        {
            if (HasSleepTime)
            {
                FireInformation("Execution is suspended for " + SleepTimeInMinutes + " minutes...");
                Thread.Sleep(TimeSpan.FromMinutes(SleepTimeInMinutes ?? 0));
            }
        }


        private void WaitForSQL()
        {
            if (HasSQLCheckStatement && HasSQLConnection)
            {
                ConnectionManager connMan = GetConnectionManager(Conns);

                if (connMan.CreationName.StartsWith("ADO.NET"))
                {
                    SqlCommand cmd = GetSqlCommand(connMan);
                    QueryDatabaseFrequently(cmd);
                }
                else if (connMan.CreationName.StartsWith("OLEDB"))
                {
                    OleDbCommand cmd = GetOleDbCommand(connMan);
                    QueryDatabaseFrequently(cmd);
                }
            }
        }

        private SqlCommand GetSqlCommand(ConnectionManager connMan)
        {
            SqlConnection conn = null;
            conn = connMan.AcquireConnection(Transaction) as SqlConnection;

            if (conn == null)
                FireError("Failed to aquire ADO.NET connection");

            SqlCommand cmd = new SqlCommand(SQLCheckStatement, conn);
            return cmd;
        }

        private OleDbCommand GetOleDbCommand(ConnectionManager connMan)
        {
            DtsWrap.IDTSConnectionManagerDatabaseParameters100 cmParams = connMan.InnerObject as DtsWrap.IDTSConnectionManagerDatabaseParameters100;
            OleDbConnection conn = cmParams.GetConnectionForSchema() as OleDbConnection;

            if (conn == null)
                FireError("Failed to aquire OLEDB connection");

            OleDbCommand cmd = new OleDbCommand(SQLCheckStatement, conn);
            return cmd;
        }

        private void QueryDatabaseFrequently(IDbCommand cmd)
        {
            FireInformation("Starting query database with sql check statement: " + cmd.CommandText);
            DateTime endTime = DateTime.Now;
            if (HasMaximumSQLWaitTime)
            {
                endTime = DateTime.Now.AddMinutes(MaximumSQLWaitTime ?? 0);
                FireInformation("Execution will be repeated until" + endTime.ToShortTimeString());
            }
            DateTime maxTime = DateTime.Now.AddDays(1);

            bool continueWaiting = true;

            do
            {
                var result = ExecuteSql(cmd);                
                if (IsResultTrue(result))
                {
                    FireInformation("Sql query returns 1, execution is continued.");
                    continueWaiting = false;
                }
                else if (IsResultFalse(result))
                {
                    FireInformation("Sql query returns 0, execution is delayed.");
                    if (HasMaximumSQLWaitTime)
                    {
                        if (DateTime.Now >= endTime)
                        {
                            continueWaiting = false;
                            FireError("Maximun wait time reached - task will abort execution.");
                        }
                        else
                        {
                            continueWaiting = true;
                        }
                    }
                    else
                    {
                        continueWaiting = true;
                    }
                }
                else
                {
                    FireError("The sql check statement return a value different from 1 or 0. Please adjust the query. The return value was "+ (result == null ? "Null" : result.ToString()));
                    break;
                }

                if (continueWaiting)
                {
                    if (DateTime.Now >= maxTime)
                    {
                        FireError("Maximum wait time of 24h reached - execution stopped!");
                        break;
                    }

                    if (HasSQLRepetitionFrequencyInMinutes)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(SQLRepetitionFrequencyInMinutes ?? 0));
                        FireInformation("Execution is delayed, task will sleep for " + (SQLRepetitionFrequencyInMinutes ?? 0) + " minutes...");
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        FireInformation("Execution is delayed, task will sleep for 1 second...");
                    }
                }
            } while (continueWaiting);
        }

        private bool IsResultTrue(object result)
        {
            if (result != null && (result.ToString().Trim().Equals("1") || result.ToString().Trim().ToLower().Equals("true")))
                return true;
            else
                return false;
        }

        private bool IsResultFalse(object result)
        {
            if (result != null && (result.ToString().Trim().Equals("0") || result.ToString().Trim().ToLower().Equals("false")))
                return true;
            else
                return false;
        }

        private object ExecuteSql(IDbCommand cmd)
        {
            try
            {
                var result = cmd.ExecuteScalar();
                return result;
            }
            catch (Exception e)
            {
                FireError("Error while executing Sql: " + e.Message.ToString());
                return String.Empty;
            }

        }

        private void FireError(string text)
        {
            if (Events != null)
                Events.FireError(0, "WaitTask", text, "See more on ssis.andreaslennartz.de", 0);
            throw new ApplicationException(text);
        }

        private void FireInformation(string text)
        {
            bool fireAgain = false;
            Events.FireInformation(0, "WaitTask", text, "See more on ssis.andreaslennartz.de", 0, ref fireAgain);
        }

        private void FireWarning(string text)
        {
            Events.FireWarning(1, this.GetType().Name, text, string.Empty, 0);
        }

        #region IDTSComponentPersist

        public void LoadFromXML(XmlElement node, IDTSInfoEvents infoEvents)
        {
            XmlModificator modificator = new XmlModificator(infoEvents, this);
            modificator.LoadXml(node);
        }

        public void SaveToXML(XmlDocument doc, IDTSInfoEvents infoEvents)
        {
            XmlModificator modificator = new XmlModificator(infoEvents, this);
            modificator.SaveXml(doc);
        }

        #endregion
    }


}
