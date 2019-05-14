namespace FRC_FencingResultConverter
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.groupBoxMulti = new System.Windows.Forms.GroupBox();
            this.radioButtonOnlyFiles = new System.Windows.Forms.RadioButton();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.radioButtonBundleAndFiles = new System.Windows.Forms.RadioButton();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.listBoxFileName = new System.Windows.Forms.ListBox();
            this.radioButtonOnlyBundle = new System.Windows.Forms.RadioButton();
            this.groupBoxMulti.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(112, 254);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "Copyright © 2015 Robin Kase";
            // 
            // buttonConvert
            // 
            this.buttonConvert.Enabled = false;
            this.buttonConvert.Location = new System.Drawing.Point(115, 201);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(128, 42);
            this.buttonConvert.TabIndex = 13;
            this.buttonConvert.Text = "Convert";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // groupBoxMulti
            // 
            this.groupBoxMulti.Controls.Add(this.radioButtonOnlyFiles);
            this.groupBoxMulti.Controls.Add(this.buttonRemove);
            this.groupBoxMulti.Controls.Add(this.radioButtonBundleAndFiles);
            this.groupBoxMulti.Controls.Add(this.buttonOpen);
            this.groupBoxMulti.Controls.Add(this.listBoxFileName);
            this.groupBoxMulti.Controls.Add(this.radioButtonOnlyBundle);
            this.groupBoxMulti.Location = new System.Drawing.Point(53, 12);
            this.groupBoxMulti.Name = "groupBoxMulti";
            this.groupBoxMulti.Size = new System.Drawing.Size(278, 183);
            this.groupBoxMulti.TabIndex = 17;
            this.groupBoxMulti.TabStop = false;
            // 
            // radioButtonOnlyFiles
            // 
            this.radioButtonOnlyFiles.AutoSize = true;
            this.radioButtonOnlyFiles.Location = new System.Drawing.Point(19, 161);
            this.radioButtonOnlyFiles.Name = "radioButtonOnlyFiles";
            this.radioButtonOnlyFiles.Size = new System.Drawing.Size(181, 16);
            this.radioButtonOnlyFiles.TabIndex = 21;
            this.radioButtonOnlyFiles.TabStop = true;
            this.radioButtonOnlyFiles.Text = "Only files separately as output";
            this.radioButtonOnlyFiles.UseVisualStyleBackColor = true;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(7, 66);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(61, 29);
            this.buttonRemove.TabIndex = 20;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // radioButtonBundleAndFiles
            // 
            this.radioButtonBundleAndFiles.AutoSize = true;
            this.radioButtonBundleAndFiles.Checked = true;
            this.radioButtonBundleAndFiles.Location = new System.Drawing.Point(19, 117);
            this.radioButtonBundleAndFiles.Name = "radioButtonBundleAndFiles";
            this.radioButtonBundleAndFiles.Size = new System.Drawing.Size(253, 16);
            this.radioButtonBundleAndFiles.TabIndex = 17;
            this.radioButtonBundleAndFiles.TabStop = true;
            this.radioButtonBundleAndFiles.Text = "One bundle and all files separately as output";
            this.radioButtonBundleAndFiles.UseVisualStyleBackColor = true;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(19, 22);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(49, 29);
            this.buttonOpen.TabIndex = 14;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // listBoxFileName
            // 
            this.listBoxFileName.FormattingEnabled = true;
            this.listBoxFileName.ItemHeight = 12;
            this.listBoxFileName.Location = new System.Drawing.Point(74, 12);
            this.listBoxFileName.Name = "listBoxFileName";
            this.listBoxFileName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.listBoxFileName.Size = new System.Drawing.Size(198, 100);
            this.listBoxFileName.TabIndex = 15;
            // 
            // radioButtonOnlyBundle
            // 
            this.radioButtonOnlyBundle.AutoSize = true;
            this.radioButtonOnlyBundle.Location = new System.Drawing.Point(19, 139);
            this.radioButtonOnlyBundle.Name = "radioButtonOnlyBundle";
            this.radioButtonOnlyBundle.Size = new System.Drawing.Size(157, 16);
            this.radioButtonOnlyBundle.TabIndex = 16;
            this.radioButtonOnlyBundle.Text = "Only one bundle as output";
            this.radioButtonOnlyBundle.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 278);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.groupBoxMulti);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "FRC - Fencing Result Converter 2.5";
            this.groupBoxMulti.ResumeLayout(false);
            this.groupBoxMulti.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.GroupBox groupBoxMulti;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.RadioButton radioButtonBundleAndFiles;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.ListBox listBoxFileName;
        private System.Windows.Forms.RadioButton radioButtonOnlyBundle;
        private System.Windows.Forms.RadioButton radioButtonOnlyFiles;
    }
}