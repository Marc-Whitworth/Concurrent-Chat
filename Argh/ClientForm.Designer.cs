using System;

namespace ClientClasses
{
    partial class ClientForm
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
            this.components = new System.ComponentModel.Container();
            this.MessageWindow = new System.Windows.Forms.TextBox();
            this.SubmitButton = new System.Windows.Forms.Button();
            this.InputField = new System.Windows.Forms.TextBox();
            this.ActionList = new System.Windows.Forms.ComboBox();
            this.ClientList = new System.Windows.Forms.ListBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.clientNamePacketBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.clientNamePacketBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // MessageWindow
            // 
            this.MessageWindow.Enabled = false;
            this.MessageWindow.Location = new System.Drawing.Point(12, 12);
            this.MessageWindow.Multiline = true;
            this.MessageWindow.Name = "MessageWindow";
            this.MessageWindow.ReadOnly = true;
            this.MessageWindow.Size = new System.Drawing.Size(569, 367);
            this.MessageWindow.TabIndex = 0;
            this.MessageWindow.Visible = false;
            // 
            // SubmitButton
            // 
            this.SubmitButton.Enabled = false;
            this.SubmitButton.Location = new System.Drawing.Point(629, 397);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(154, 22);
            this.SubmitButton.TabIndex = 1;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Visible = false;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // InputField
            // 
            this.InputField.Enabled = false;
            this.InputField.Location = new System.Drawing.Point(12, 397);
            this.InputField.Name = "InputField";
            this.InputField.Size = new System.Drawing.Size(569, 22);
            this.InputField.TabIndex = 2;
            this.InputField.Visible = false;
            // 
            // ActionList
            // 
            this.ActionList.Enabled = false;
            this.ActionList.FormattingEnabled = true;
            this.ActionList.Items.AddRange(new object[] {
            "Send Global",
            "Send Private",
            "Change Name"});
            this.ActionList.Location = new System.Drawing.Point(629, 315);
            this.ActionList.Name = "ActionList";
            this.ActionList.Size = new System.Drawing.Size(154, 24);
            this.ActionList.TabIndex = 3;
            this.ActionList.Visible = false;
            this.ActionList.SelectionChangeCommitted += new System.EventHandler(this.ActionList_SelectionChangeCommitted);
            // 
            // ClientList
            // 
            this.ClientList.Cursor = System.Windows.Forms.Cursors.Default;
            this.ClientList.Enabled = false;
            this.ClientList.FormattingEnabled = true;
            this.ClientList.ItemHeight = 16;
            this.ClientList.Location = new System.Drawing.Point(629, 65);
            this.ClientList.Name = "ClientList";
            this.ClientList.Size = new System.Drawing.Size(154, 244);
            this.ClientList.Sorted = true;
            this.ClientList.TabIndex = 4;
            this.ClientList.Visible = false;
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(629, 13);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(154, 46);
            this.loginButton.TabIndex = 5;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // clientNamePacketBindingSource
            // 
            this.clientNamePacketBindingSource.DataSource = typeof(Packets.ClientNamePacket);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.ClientList);
            this.Controls.Add(this.ActionList);
            this.Controls.Add(this.InputField);
            this.Controls.Add(this.SubmitButton);
            this.Controls.Add(this.MessageWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ClientForm";
            this.Text = "Unconnected";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.clientNamePacketBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.TextBox MessageWindow;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.TextBox InputField;
        private System.Windows.Forms.ComboBox ActionList;
        private System.Windows.Forms.ListBox ClientList;
        private System.Windows.Forms.BindingSource clientNamePacketBindingSource;
        private System.Windows.Forms.Button loginButton;
    }
}