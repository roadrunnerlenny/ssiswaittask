using System;
using System.Drawing;
using System.Windows.Forms;

namespace ALE.WaitTask.Helper
{
    public partial class SqlEditor : Form
    {

        public string InitialTextBoxValue;

        public string TextBoxValue
        {
            get
            {
                return this.SqlTextBox.Text;
            }
            private set
            {
                if (value != null)
                    this.SqlTextBox.Text = value;
                else
                    this.SqlTextBox.Text = string.Empty;
            }
        }
        public SqlEditor()
        {
            InitializeComponent();
        }
                
        private void SqlEditor_Load(object sender, EventArgs e)
        {
            this.TextBoxValue = InitialTextBoxValue;
            this.Icon = new Icon(typeof(WaitTaskMain).Assembly.GetManifestResourceStream("ALE.WaitTask.clock.ico"));
        }

    }
}
