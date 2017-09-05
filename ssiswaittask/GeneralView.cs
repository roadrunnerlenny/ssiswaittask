using System;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using Microsoft.DataTransformationServices.Controls;

namespace ALE.WaitTask
{
    public partial class GeneralView : UserControl, IDTSTaskUIView
    {
        private GeneralViewNode generalNode;
        internal GeneralViewNode GeneralNode
        {
            get { return generalNode; }
        }

        public GeneralView()
        {           
            InitializeComponent();  
        }

        #region IDTSTaskUIView Members

        public void OnCommit(object taskHost)
        {
            TaskHost host = taskHost as TaskHost;
            if (host == null)
                throw new ArgumentException("Arugment is not a TaskHost.", "taskHost");

            WaitTaskMain task = host.InnerObject as WaitTaskMain;
            if (task == null)
            {
                throw new ArgumentException("Arugment is not a A.LE WaitTask.", "taskHost");
            }

            host.Name = generalNode.Name;
            host.Description = generalNode.Description;

            // Task properties
            task.WaitUntilTimeInternal = generalNode.WaitUntilTimeInternal;
            task.SleepTimeInMinutes = generalNode.SleepTimeInMinutes;
            task.DoNothingAndContinue = generalNode.DoNothingAndContinue;
            task.SQLRepetitionFrequencyInMinutes = generalNode.SQLRepetitionFrequencyInMinutes;
            task.SQLCheckStatement = generalNode.SQLCheckStatement;
            task.SQLConnectionID = generalNode.SQLConnectionID;
            task.MaximumSQLWaitTime = generalNode.MaximumSQLWaitTime;            
        }

        

        public void OnInitialize(IDTSTaskUIHost treeHost, TreeNode viewNode, object taskHost, object connections)
        {
            this.generalNode = new GeneralViewNode(taskHost as TaskHost, connections as IDtsConnectionService);
            this.propertyGrid.SelectedObject = generalNode;
        }

        public void OnLoseSelection(ref bool bCanLeaveView, ref string reason)
        {
        }

        public void OnSelection()
        {
        }

        public void OnValidate(ref bool bViewIsValid, ref string reason)
        {
        }

        #endregion
    }

    
}
