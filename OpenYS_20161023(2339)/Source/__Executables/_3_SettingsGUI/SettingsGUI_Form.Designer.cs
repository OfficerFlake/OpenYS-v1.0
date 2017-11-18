namespace OpenYS
{
    public partial class SettingsGUI_Form
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.Button Reload;
            System.Windows.Forms.Button Save;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsGUI_Form));
            this.DataGridObject = new System.Windows.Forms.DataGridView();
            Reload = new System.Windows.Forms.Button();
            Save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridObject)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridObject
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(13)))), ((int)(((byte)(13)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("BankGothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Empty;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Empty;
            this.DataGridObject.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridObject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridObject.BackgroundColor = System.Drawing.Color.Black;
            this.DataGridObject.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridObject.Location = new System.Drawing.Point(13, 36);
            this.DataGridObject.Name = "DataGridObject";
            this.DataGridObject.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("BankGothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            this.DataGridObject.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridObject.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.DataGridObject.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("BankGothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DataGridObject.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.DataGridObject.Size = new System.Drawing.Size(599, 394);
            this.DataGridObject.TabIndex = 0;
            // 
            // Reload
            // 
            Reload.Location = new System.Drawing.Point(12, 7);
            Reload.Name = "Reload";
            Reload.Size = new System.Drawing.Size(296, 23);
            Reload.TabIndex = 1;
            Reload.Text = "Reload";
            Reload.UseCompatibleTextRendering = true;
            Reload.UseVisualStyleBackColor = true;
            Reload.Click += new System.EventHandler(this.Reload_Click);
            // 
            // Save
            // 
            Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            Save.Location = new System.Drawing.Point(315, 7);
            Save.Name = "Save";
            Save.Size = new System.Drawing.Size(297, 23);
            Save.TabIndex = 2;
            Save.Text = "Save";
            Save.UseCompatibleTextRendering = true;
            Save.UseVisualStyleBackColor = true;
            Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // SettingsGUI_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(Save);
            this.Controls.Add(Reload);
            this.Controls.Add(this.DataGridObject);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsGUI_Form";
            this.Text = "OpenYS Settings GUI";
            ((System.ComponentModel.ISupportInitialize)(this.DataGridObject)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView DataGridObject;
    }
}

