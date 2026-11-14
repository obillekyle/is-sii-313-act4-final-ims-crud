namespace Obille_Try.Dialogs
{
    partial class UserDialog
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
            footer = new Kit.DialogFooter();
            saveButton = new Kit.KitButton();
            cancelButton = new Kit.KitButton();
            fields = new TableLayoutPanel();
            fullNameField = new Kit.Field();
            fullNameBox = new Kit.TextInput();
            usernameField = new Kit.Field();
            usernameBox = new Kit.TextInput();
            roleField = new Kit.Field();
            roleBox = new Kit.Select();
            passwordField = new Kit.Field();
            passwordBox = new Kit.TextInput();
            confirmField = new Kit.Field();
            confirmBox = new Kit.TextInput();
            statusField = new Kit.Field();
            activeBox = new Kit.Checkbox();
            footer.SuspendLayout();
            fields.SuspendLayout();
            fullNameField.SuspendLayout();
            usernameField.SuspendLayout();
            roleField.SuspendLayout();
            passwordField.SuspendLayout();
            confirmField.SuspendLayout();
            statusField.SuspendLayout();
            SuspendLayout();
            //
            // footer
            //
            footer.Controls.Add(saveButton);
            footer.Controls.Add(cancelButton);
            footer.Location = new Point(20, 360);
            footer.Name = "footer";
            footer.Size = new Size(528, 56);
            footer.TabIndex = 1;
            //
            // saveButton
            //
            saveButton.AutoSize = true;
            saveButton.Location = new Point(424, 12);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(104, 32);
            saveButton.TabIndex = 0;
            saveButton.Text = "Add user";
            saveButton.Variant = Kit.ButtonVariant.Primary;
            saveButton.Click += saveButton_Click;
            //
            // cancelButton
            //
            cancelButton.AutoSize = true;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(334, 12);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(82, 32);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            //
            // fields
            //
            fields.AutoSize = true;
            fields.ColumnCount = 2;
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            fields.Controls.Add(fullNameField, 0, 0);
            fields.Controls.Add(usernameField, 0, 1);
            fields.Controls.Add(roleField, 1, 1);
            fields.Controls.Add(passwordField, 0, 2);
            fields.Controls.Add(confirmField, 1, 2);
            fields.Controls.Add(statusField, 0, 3);
            fields.Dock = DockStyle.Top;
            fields.Location = new Point(20, 56);
            fields.Name = "fields";
            fields.RowCount = 4;
            fields.RowStyles.Add(new RowStyle());
            fields.RowStyles.Add(new RowStyle());
            fields.RowStyles.Add(new RowStyle());
            fields.RowStyles.Add(new RowStyle());
            fields.Size = new Size(528, 288);
            fields.TabIndex = 0;
            //
            // fullNameField
            //
            fields.SetColumnSpan(fullNameField, 2);
            fullNameField.Controls.Add(fullNameBox);
            fullNameField.Dock = DockStyle.Fill;
            fullNameField.Label = "Full name";
            fullNameField.Location = new Point(0, 0);
            fullNameField.Margin = new Padding(0, 0, 0, 12);
            fullNameField.Name = "fullNameField";
            fullNameField.Size = new Size(528, 64);
            fullNameField.TabIndex = 0;
            //
            // fullNameBox
            //
            fullNameBox.Location = new Point(0, 26);
            fullNameBox.Name = "fullNameBox";
            fullNameBox.Placeholder = "Maria Santos";
            fullNameBox.Size = new Size(528, 32);
            fullNameBox.TabIndex = 0;
            //
            // usernameField
            //
            usernameField.Controls.Add(usernameBox);
            usernameField.Dock = DockStyle.Fill;
            usernameField.Label = "Username";
            usernameField.Location = new Point(0, 76);
            usernameField.Margin = new Padding(0, 0, 8, 12);
            usernameField.Name = "usernameField";
            usernameField.Size = new Size(256, 64);
            usernameField.TabIndex = 1;
            //
            // usernameBox
            //
            usernameBox.Location = new Point(0, 26);
            usernameBox.Name = "usernameBox";
            usernameBox.Placeholder = "msantos";
            usernameBox.Size = new Size(256, 32);
            usernameBox.TabIndex = 0;
            //
            // roleField
            //
            roleField.Controls.Add(roleBox);
            roleField.Dock = DockStyle.Fill;
            roleField.Label = "Role";
            roleField.Location = new Point(272, 76);
            roleField.Margin = new Padding(8, 0, 0, 12);
            roleField.Name = "roleField";
            roleField.Size = new Size(256, 64);
            roleField.TabIndex = 2;
            //
            // roleBox
            //
            roleBox.Location = new Point(0, 26);
            roleBox.Name = "roleBox";
            roleBox.Placeholder = "Choose a role";
            roleBox.Size = new Size(256, 32);
            roleBox.TabIndex = 0;
            //
            // passwordField
            //
            passwordField.Controls.Add(passwordBox);
            passwordField.Dock = DockStyle.Fill;
            passwordField.Label = "Password";
            passwordField.Location = new Point(0, 152);
            passwordField.Margin = new Padding(0, 0, 8, 12);
            passwordField.Name = "passwordField";
            passwordField.Size = new Size(256, 64);
            passwordField.TabIndex = 3;
            //
            // passwordBox
            //
            passwordBox.Location = new Point(0, 26);
            passwordBox.Name = "passwordBox";
            passwordBox.Size = new Size(256, 32);
            passwordBox.TabIndex = 0;
            passwordBox.Type = Kit.InputType.Password;
            //
            // confirmField
            //
            confirmField.Controls.Add(confirmBox);
            confirmField.Dock = DockStyle.Fill;
            confirmField.Label = "Confirm password";
            confirmField.Location = new Point(272, 152);
            confirmField.Margin = new Padding(8, 0, 0, 12);
            confirmField.Name = "confirmField";
            confirmField.Size = new Size(256, 64);
            confirmField.TabIndex = 4;
            //
            // confirmBox
            //
            confirmBox.Location = new Point(0, 26);
            confirmBox.Name = "confirmBox";
            confirmBox.Size = new Size(256, 32);
            confirmBox.TabIndex = 0;
            confirmBox.Type = Kit.InputType.Password;
            //
            // statusField
            //
            statusField.Controls.Add(activeBox);
            statusField.Dock = DockStyle.Fill;
            statusField.Label = "Status";
            statusField.Optional = true;
            statusField.Location = new Point(0, 228);
            statusField.Margin = new Padding(0, 0, 8, 0);
            statusField.Name = "statusField";
            statusField.Size = new Size(256, 56);
            statusField.TabIndex = 5;
            //
            // activeBox
            //
            activeBox.Checked = true;
            activeBox.Location = new Point(0, 26);
            activeBox.Name = "activeBox";
            activeBox.Size = new Size(256, 24);
            activeBox.TabIndex = 0;
            activeBox.Text = "Active, can sign in";
            //
            // UserDialog
            //
            AcceptButton = saveButton;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = cancelButton;
            ClientSize = new Size(560, 420);
            Controls.Add(fields);
            Controls.Add(footer);
            Name = "UserDialog";
            Text = "New user";
            Title = "New user";
            footer.ResumeLayout(false);
            footer.PerformLayout();
            fields.ResumeLayout(false);
            fullNameField.ResumeLayout(false);
            usernameField.ResumeLayout(false);
            roleField.ResumeLayout(false);
            passwordField.ResumeLayout(false);
            confirmField.ResumeLayout(false);
            statusField.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Kit.DialogFooter footer;
        private Kit.KitButton saveButton;
        private Kit.KitButton cancelButton;
        private TableLayoutPanel fields;
        private Kit.Field fullNameField;
        private Kit.TextInput fullNameBox;
        private Kit.Field usernameField;
        private Kit.TextInput usernameBox;
        private Kit.Field roleField;
        private Kit.Select roleBox;
        private Kit.Field passwordField;
        private Kit.TextInput passwordBox;
        private Kit.Field confirmField;
        private Kit.TextInput confirmBox;
        private Kit.Field statusField;
        private Kit.Checkbox activeBox;
    }
}
