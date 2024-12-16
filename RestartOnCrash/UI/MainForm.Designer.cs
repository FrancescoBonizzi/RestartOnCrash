namespace RestartOnCrash.UI
{
    /// <summary>
    /// Main menu fo work with application.
    /// </summary>
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            startServiceButton = new System.Windows.Forms.Button();
            selectFileButton = new System.Windows.Forms.Button();
            listOfAddedPrograms = new System.Windows.Forms.ListBox();
            selectProgramToCheck = new System.Windows.Forms.OpenFileDialog();
            stopServiceButton = new System.Windows.Forms.Button();
            waitBeforeRestart = new System.Windows.Forms.CheckBox();
            notifyIcon = new System.Windows.Forms.NotifyIcon(components);
            timeTextBox = new System.Windows.Forms.MaskedTextBox();
            label1 = new System.Windows.Forms.Label();
            toolTipCheckbox = new System.Windows.Forms.ToolTip(components);
            removeFileButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // startServiceButton
            // 
            startServiceButton.Location = new System.Drawing.Point(138, 12);
            startServiceButton.Name = "startServiceButton";
            startServiceButton.Size = new System.Drawing.Size(80, 23);
            startServiceButton.TabIndex = 0;
            startServiceButton.Text = "Start service";
            startServiceButton.UseVisualStyleBackColor = true;
            startServiceButton.Click += startServiceButton_Click;
            // 
            // selectFileButton
            // 
            selectFileButton.Location = new System.Drawing.Point(138, 70);
            selectFileButton.Name = "selectFileButton";
            selectFileButton.Size = new System.Drawing.Size(80, 20);
            selectFileButton.TabIndex = 1;
            selectFileButton.Text = "Add exe";
            selectFileButton.UseVisualStyleBackColor = true;
            selectFileButton.Click += selectFileButton_Click;
            // 
            // listOfAddedPrograms
            // 
            listOfAddedPrograms.BackColor = System.Drawing.SystemColors.ActiveBorder;
            listOfAddedPrograms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            listOfAddedPrograms.FormattingEnabled = true;
            listOfAddedPrograms.ItemHeight = 15;
            listOfAddedPrograms.Location = new System.Drawing.Point(10, 10);
            listOfAddedPrograms.Margin = new System.Windows.Forms.Padding(0);
            listOfAddedPrograms.Name = "listOfAddedPrograms";
            listOfAddedPrograms.Size = new System.Drawing.Size(120, 225);
            listOfAddedPrograms.TabIndex = 2;
            // 
            // selectProgramToCheck
            // 
            selectProgramToCheck.FileName = "openFileDialog1";
            // 
            // stopServiceButton
            // 
            stopServiceButton.Location = new System.Drawing.Point(138, 41);
            stopServiceButton.Name = "stopServiceButton";
            stopServiceButton.Size = new System.Drawing.Size(80, 23);
            stopServiceButton.TabIndex = 3;
            stopServiceButton.Text = "Stop service";
            stopServiceButton.UseVisualStyleBackColor = true;
            stopServiceButton.Click += StopServiceThread_Click;
            // 
            // waitBeforeRestart
            // 
            waitBeforeRestart.AutoSize = true;
            waitBeforeRestart.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            waitBeforeRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            waitBeforeRestart.Location = new System.Drawing.Point(138, 186);
            waitBeforeRestart.Name = "waitBeforeRestart";
            waitBeforeRestart.Size = new System.Drawing.Size(69, 49);
            waitBeforeRestart.TabIndex = 4;
            waitBeforeRestart.Text = "Wait\r\nfor\r\nfirst start";
            waitBeforeRestart.UseVisualStyleBackColor = true;
            waitBeforeRestart.CheckedChanged += waitBeforeRestart_CheckedChanged;
            // 
            // notifyIcon
            // 
            notifyIcon.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "Restart on crash";
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            // 
            // timeTextBox
            // 
            timeTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            timeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            timeTextBox.Location = new System.Drawing.Point(138, 164);
            timeTextBox.Mask = "00:00:00";
            timeTextBox.Name = "timeTextBox";
            timeTextBox.Size = new System.Drawing.Size(80, 16);
            timeTextBox.TabIndex = 6;
            timeTextBox.Text = "000000";
            timeTextBox.TextChanged += timeTextBox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(138, 146);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(80, 15);
            label1.TabIndex = 7;
            label1.Text = "Restart period";
            // 
            // toolTipCheckbox
            // 
            toolTipCheckbox.BackColor = System.Drawing.SystemColors.ControlDark;
            // 
            // removeFileButton
            // 
            removeFileButton.Location = new System.Drawing.Point(138, 96);
            removeFileButton.Name = "removeFileButton";
            removeFileButton.Size = new System.Drawing.Size(80, 20);
            removeFileButton.TabIndex = 8;
            removeFileButton.Text = "Del exe";
            removeFileButton.UseVisualStyleBackColor = true;
            removeFileButton.Click += removeFileButton_Click;
            // 
            // MainForm
            // 
            AcceptButton = startServiceButton;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            BackColor = System.Drawing.SystemColors.ControlDark;
            CancelButton = stopServiceButton;
            ClientSize = new System.Drawing.Size(234, 241);
            Controls.Add(removeFileButton);
            Controls.Add(label1);
            Controls.Add(timeTextBox);
            Controls.Add(waitBeforeRestart);
            Controls.Add(stopServiceButton);
            Controls.Add(listOfAddedPrograms);
            Controls.Add(selectFileButton);
            Controls.Add(startServiceButton);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Restart on crash";
            Resize += MainForm_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button startServiceButton;
        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.ListBox listOfAddedPrograms;
        private System.Windows.Forms.OpenFileDialog selectProgramToCheck;
        private System.Windows.Forms.Button stopServiceButton;
        private System.Windows.Forms.CheckBox waitBeforeRestart;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.MaskedTextBox timeTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTipCheckbox;
        private System.Windows.Forms.Button removeFileButton;
    }
}