namespace ALE.WaitTask.Helper
{
    partial class SqlEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SqlTextBox = new System.Windows.Forms.TextBox();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButtonPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(166, 12);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(247, 12);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // SqlTextBox
            // 
            this.SqlTextBox.AcceptsReturn = true;
            this.SqlTextBox.AcceptsTab = true;
            this.SqlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SqlTextBox.Location = new System.Drawing.Point(10, 10);
            this.SqlTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.SqlTextBox.MinimumSize = new System.Drawing.Size(314, 246);
            this.SqlTextBox.Multiline = true;
            this.SqlTextBox.Name = "SqlTextBox";
            this.SqlTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SqlTextBox.Size = new System.Drawing.Size(314, 252);
            this.SqlTextBox.TabIndex = 1;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Controls.Add(this.CancelButton);
            this.ButtonPanel.Controls.Add(this.OKButton);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 272);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(334, 40);
            this.ButtonPanel.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SqlTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(334, 272);
            this.panel1.TabIndex = 5;
            // 
            // SqlEditor
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 312);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ButtonPanel);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "SqlEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter SQL Query";
            this.Load += new System.EventHandler(this.SqlEditor_Load);
            this.ButtonPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.TextBox SqlTextBox;
        private System.Windows.Forms.Panel ButtonPanel;
        private System.Windows.Forms.Panel panel1;
    }
}