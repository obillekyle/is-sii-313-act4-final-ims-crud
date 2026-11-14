namespace Obille_Try.Dialogs
{
    partial class ConfirmDialog
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
            actionButton = new Kit.KitButton();
            cancelButton = new Kit.KitButton();
            footer.SuspendLayout();
            SuspendLayout();
            //
            // footer
            //
            footer.Controls.Add(actionButton);
            footer.Controls.Add(cancelButton);
            footer.Location = new Point(20, 90);
            footer.Name = "footer";
            footer.Size = new Size(368, 56);
            footer.TabIndex = 0;
            //
            // actionButton
            //
            actionButton.AutoSize = true;
            actionButton.DialogResult = DialogResult.OK;
            actionButton.Location = new Point(262, 12);
            actionButton.Name = "actionButton";
            actionButton.Size = new Size(106, 32);
            actionButton.TabIndex = 1;
            actionButton.Text = "Delete";
            actionButton.Variant = Kit.ButtonVariant.Danger;
            //
            // cancelButton
            //
            cancelButton.AutoSize = true;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(172, 12);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(82, 32);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "Cancel";
            //
            // ConfirmDialog
            //
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = cancelButton;
            ClientSize = new Size(400, 146);
            Controls.Add(footer);
            Description = "This can't be undone.";
            DialogSize = Kit.DialogSize.Small;
            Name = "ConfirmDialog";
            Text = "Delete?";
            Title = "Delete?";
            footer.ResumeLayout(false);
            footer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Kit.DialogFooter footer;
        private Kit.KitButton actionButton;
        private Kit.KitButton cancelButton;
    }
}
