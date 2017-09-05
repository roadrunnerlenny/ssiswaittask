using System;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

//how to create a default UI for SSIS Task: http://blogs.msdn.com/b/mattm/archive/2008/07/18/creating-a-custom-task-with-a-default-ui.aspx

namespace ALE.WaitTask
{
    public class WaitTaskUI : IDtsTaskUI
    {

        private TaskHost _taskHost = null;
        private IDtsConnectionService _connectionService = null;

        #region IDtsTaskUI Members

        public void Delete(IWin32Window parentWindow)
        {
        }

        public ContainerControl GetView()
        {
            return new WaitTaskMainWnd(_taskHost, _connectionService);
        }

        public void Initialize(TaskHost taskHost, IServiceProvider serviceProvider)
        {
            this._taskHost = taskHost;
            this._connectionService = serviceProvider.GetService(typeof(IDtsConnectionService)) as IDtsConnectionService;
        }

        public void New(IWin32Window parentWindow)
        {
        }

        #endregion

    }
}
