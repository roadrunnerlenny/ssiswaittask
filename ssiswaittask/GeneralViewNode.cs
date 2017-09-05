using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ALE.WaitTask.Helper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace ALE.WaitTask
{
    internal class GeneralViewNode
    {
        internal TaskHost taskHost = null;
        internal IDtsConnectionService connectionService = null;

        internal GeneralViewNode(TaskHost taskHost, IDtsConnectionService connectionService)
        {
            this.taskHost = taskHost;
            this.connectionService = connectionService;

            // Extract common values from the Task Host
            name = taskHost.Name;
            description = taskHost.Description;

            // Extract values from the task object
            WaitTaskMain task = taskHost.InnerObject as WaitTaskMain;
            if (task == null) throw new ArgumentException("Type mismatch for taskHost inner object.");

            AvailableConnections = LoadDBConnections();

            WaitUntilTimeInternal = task.WaitUntilTimeInternal;
            SleepTimeInMinutes = task.SleepTimeInMinutes;
            DoNothingAndContinue = task.DoNothingAndContinue;
            SQLRepetitionFrequencyInMinutes = task.SQLRepetitionFrequencyInMinutes;
            SQLCheckStatement = task.SQLCheckStatement;
            
            SQLConnectionID = task.SQLConnectionID;
            var connection = GetConnectionById(task.SQLConnectionID);
            if (connection != null)
                _sqlConnection = connection.Name;
            
            MaximumSQLWaitTime = task.MaximumSQLWaitTime;
        }

        internal List<ConnectionManager> LoadDBConnections()
        {
            List<ConnectionManager> result = new List<ConnectionManager>();
            foreach (ConnectionManager connectionManager in connectionService.GetConnections())
            {
                if (connectionManager.CreationName.StartsWith("ADO.NET") || connectionManager.CreationName.StartsWith("OLEDB"))
                {
                    result.Add(connectionManager);
                }
            }
            return result;
        }

        internal ConnectionManager GetConnectionById(string connectionId)
        {
            foreach (ConnectionManager connectionManager in connectionService.GetConnections())
            {
                if (connectionManager.ID.Equals(connectionId))
                    return connectionManager;
            }
            return null;
        }

        #region Properties

        private string name = string.Empty;
        [Category("1. General"), Description("Specifies the name for this task.")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Trim().Length == 0)
                    throw new ApplicationException("Task name cannot be empty");
                name = value;
            }
        }

        private string description = string.Empty;
        [Category("1. General"), Description("Describes the task.")]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value.Trim();
            }
        }

        [Category("2. Task state")]
        [Description("If set to true, the execution will continue without any waiting. ")]
        [TypeConverter(typeof(BooleanConverter))]
        public bool DoNothingAndContinue
        {
            get;
            set;
        }

        internal Time WaitUntilTimeInternal { get; set; }

        [Category("3. Wait Time")]
        [Description("Time until the wait task sleeps (hh:MM) - 24h format. Subsequently the evaluation of the sql check statement is started.")]
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

        [Category("3. Wait Time")]
        [Description("Amount of minutes this task will sleep before execution is continued. Subsequently the evaluation of the sql check statement is started.")]
        public int? SleepTimeInMinutes
        {
            get;
            set;
        }

        [Category("4. SQL (wait time passed)")]
        [Description("An SQL statement that either returns 1 (true) or 0. Wait task delays execution until the statement returns 1.")]
        [Editor(typeof(SqlEditorWrapper), typeof(UITypeEditor))]
        //    [TypeConverter(typeof(ExpandableObjectConverter))]
        public string SQLCheckStatement
        {
            get;
            set;

        }

        internal string SQLConnectionID
        { get; set; }

        private string _sqlConnection;
        [Category("4. SQL (wait time passed)")]
        [Description("OLEDB oder ADO.NET Connection on which the SQL check statement is executed.")]
        [TypeConverter(typeof(ConnectionsConverter))]
        public string SQLConnection
        {
            get
            {
                return _sqlConnection;
            }
            set
            {
                if (value != null)
                    SetSQLConnectionID(value);                    
                this._sqlConnection = value;
            }
        }

        private void SetSQLConnectionID(string value)
        {
            SQLConnectionID = null;
            foreach (ConnectionManager connMan in AvailableConnections)
                if (connMan.Name.Equals(value))
                    SQLConnectionID = connMan.ID;
            //Linq: SQLConnectionID = AvailableConnections.FirstOrDefault(match => match.Name.Equals(value)).ID;
        }

        [Category("4. SQL (wait time passed)")]
        [Description("Time period in minutes during the SQL will be evaluated frequently and execution is delayed. " +
            "If within this period the statement returns 1 the execution is continued," +
            "otherwise the execution will be delayed.")]
        public int? MaximumSQLWaitTime
        {
            get;
            set;
        }

        [Category("4. SQL (wait time passed)")]
        [Description("Pause in minutes between the execution of the SQL check statements. If left empty, the execution frequency will be 1 second.")]
        public int? SQLRepetitionFrequencyInMinutes
        {
            get;
            set;
        }

        public static List<ConnectionManager> AvailableConnections { get; set; }

        public class ConnectionsConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                List<string> result = new List<string>();
                foreach (ConnectionManager connMan in AvailableConnections)
                    result.Add(connMan.Name);
                return new StandardValuesCollection(result);
                //LINQ: return new StandardValuesCollection(AvailableConnections.Select(conn => conn.Name).ToList());
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        #endregion
    }

    class SqlEditorWrapper : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string input = value as string;
            if (svc != null)
            {
                using (SqlEditor form = new SqlEditor())
                {
                    form.InitialTextBoxValue = input;
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        input = form.TextBoxValue;
                    }
                }
            }
            return input; 
        }
    }
}
