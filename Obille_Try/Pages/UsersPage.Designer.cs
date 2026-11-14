namespace Obille_Try.Pages
{
    partial class UsersPage
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toaster = new Kit.Toaster(components);
            headerRow = new Panel();
            pageHeader = new Kit.PageHeader();
            newButton = new Kit.KitButton();
            tableRow = new Panel();
            userGrid = new Obille_Try.UI.UserGrid();
            personColumn = new DataGridViewTextBoxColumn();
            roleColumn = new DataGridViewTextBoxColumn();
            statusColumn = new DataGridViewTextBoxColumn();
            createdColumn = new DataGridViewTextBoxColumn();
            actionsColumn = new DataGridViewTextBoxColumn();
            headerRow.SuspendLayout();
            pageHeader.SuspendLayout();
            tableRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)userGrid).BeginInit();
            SuspendLayout();
            //
            // toaster
            //
            toaster.ContainerControl = this;
            //
            // headerRow
            //
            headerRow.Controls.Add(pageHeader);
            headerRow.Dock = DockStyle.Top;
            headerRow.Location = new Point(18, 8);
            headerRow.Name = "headerRow";
            headerRow.Padding = new Padding(6, 0, 6, 8);
            headerRow.Size = new Size(958, 74);
            headerRow.TabIndex = 0;
            //
            // pageHeader
            //
            pageHeader.Controls.Add(newButton);
            pageHeader.Description = "3 accounts can sign in to the inventory";
            pageHeader.Dock = DockStyle.Top;
            pageHeader.Location = new Point(6, 0);
            pageHeader.Name = "pageHeader";
            pageHeader.Size = new Size(946, 66);
            pageHeader.TabIndex = 0;
            pageHeader.Title = "Users";
            //
            // newButton
            //
            newButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            newButton.AutoSize = true;
            newButton.Icon = "plus";
            newButton.Location = new Point(840, 2);
            newButton.Name = "newButton";
            newButton.Size = new Size(106, 32);
            newButton.TabIndex = 0;
            newButton.Text = "New user";
            newButton.UseVisualStyleBackColor = false;
            newButton.Variant = Kit.ButtonVariant.Primary;
            //
            // tableRow
            //
            tableRow.Controls.Add(userGrid);
            tableRow.Dock = DockStyle.Fill;
            tableRow.Location = new Point(18, 82);
            tableRow.Name = "tableRow";
            tableRow.Padding = new Padding(6, 0, 6, 0);
            tableRow.Size = new Size(958, 548);
            tableRow.TabIndex = 1;
            //
            // userGrid
            //
            userGrid.AccessibleName = "Users";
            userGrid.AllowUserToAddRows = false;
            userGrid.AllowUserToDeleteRows = false;
            userGrid.AllowUserToResizeColumns = false;
            userGrid.AllowUserToResizeRows = false;
            userGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            userGrid.Columns.AddRange(new DataGridViewColumn[] { personColumn, roleColumn, statusColumn, createdColumn, actionsColumn });
            userGrid.Dock = DockStyle.Fill;
            userGrid.EmptyText = "No accounts yet.";
            userGrid.Location = new Point(6, 0);
            userGrid.MultiSelect = false;
            userGrid.Name = "userGrid";
            userGrid.ReadOnly = true;
            userGrid.RowHeadersVisible = false;
            userGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            userGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            userGrid.Size = new Size(946, 548);
            userGrid.StandardTab = true;
            userGrid.TabIndex = 0;
            //
            // personColumn
            //
            personColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            personColumn.DataPropertyName = "Person";
            personColumn.HeaderText = "Name";
            personColumn.Name = "personColumn";
            personColumn.ReadOnly = true;
            personColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            //
            // roleColumn
            //
            roleColumn.DataPropertyName = "Role";
            roleColumn.HeaderText = "Role";
            roleColumn.Name = "roleColumn";
            roleColumn.ReadOnly = true;
            roleColumn.Resizable = DataGridViewTriState.False;
            roleColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            roleColumn.Width = 140;
            //
            // statusColumn
            //
            statusColumn.DataPropertyName = "Status";
            statusColumn.HeaderText = "Status";
            statusColumn.Name = "statusColumn";
            statusColumn.ReadOnly = true;
            statusColumn.Resizable = DataGridViewTriState.False;
            statusColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            statusColumn.Width = 140;
            //
            // createdColumn
            //
            createdColumn.DataPropertyName = "Created";
            createdColumn.HeaderText = "Created";
            createdColumn.Name = "createdColumn";
            createdColumn.ReadOnly = true;
            createdColumn.Resizable = DataGridViewTriState.False;
            createdColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            createdColumn.Width = 150;
            //
            // actionsColumn
            //
            actionsColumn.DataPropertyName = "Actions";
            actionsColumn.HeaderText = "";
            actionsColumn.Name = "actionsColumn";
            actionsColumn.ReadOnly = true;
            actionsColumn.Resizable = DataGridViewTriState.False;
            actionsColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            actionsColumn.ToolTipText = "User actions";
            actionsColumn.Width = 64;
            //
            // UsersPage
            //
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(tableRow);
            Controls.Add(headerRow);
            Name = "UsersPage";
            Padding = new Padding(18, 8, 18, 14);
            Size = new Size(994, 644);
            headerRow.ResumeLayout(false);
            pageHeader.ResumeLayout(false);
            pageHeader.PerformLayout();
            tableRow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)userGrid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Kit.Toaster toaster;
        private Panel headerRow;
        private Kit.PageHeader pageHeader;
        private Kit.KitButton newButton;
        private Panel tableRow;
        private UI.UserGrid userGrid;
        private DataGridViewTextBoxColumn personColumn;
        private DataGridViewTextBoxColumn roleColumn;
        private DataGridViewTextBoxColumn statusColumn;
        private DataGridViewTextBoxColumn createdColumn;
        private DataGridViewTextBoxColumn actionsColumn;
    }
}
