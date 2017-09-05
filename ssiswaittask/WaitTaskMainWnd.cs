using System.Drawing;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;

namespace ALE.WaitTask
{
    public partial class WaitTaskMainWnd : DTSBaseTaskUI
    {
        private const string Title = "A.LE Wait Task";
        private const string Description = "Waits until a specific time or a data event.";
        //private static Icon TaskIcon = new Icon(typeof(WaitTaskMain), "clock.ico");
        private static Icon TaskIcon = new Icon(typeof(WaitTaskMain).Assembly.GetManifestResourceStream("ALE.WaitTask.clock.ico"));


        private GeneralView generalView;
        public GeneralView GeneralView
        {
            get { return generalView; }
        }

        public WaitTaskMainWnd(TaskHost taskHost, object connections) : base(Title, TaskIcon, Description, taskHost, connections)
        {
            InitializeComponent();

            // Setup our views
            generalView = new GeneralView();
            this.DTSTaskUIHost.FastLoad = false;
            this.DTSTaskUIHost.AddView("General", generalView, null);
            this.DTSTaskUIHost.FastLoad = true;
        }

    }
}
