namespace EqualFilesDetector
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
            this.rtbResults = new System.Windows.Forms.RichTextBox();
            this.btnStartFind = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbResults
            // 
            this.rtbResults.Location = new System.Drawing.Point(13, 13);
            this.rtbResults.Name = "rtbResults";
            this.rtbResults.Size = new System.Drawing.Size(779, 366);
            this.rtbResults.TabIndex = 0;
            this.rtbResults.Text = "";
            // 
            // btnStartFind
            // 
            this.btnStartFind.Location = new System.Drawing.Point(798, 13);
            this.btnStartFind.Name = "btnStartFind";
            this.btnStartFind.Size = new System.Drawing.Size(122, 22);
            this.btnStartFind.TabIndex = 1;
            this.btnStartFind.Text = "Start Find Equal";
            this.btnStartFind.UseVisualStyleBackColor = true;
            this.btnStartFind.Click += new System.EventHandler(this.BtnStartFindClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 391);
            this.Controls.Add(this.btnStartFind);
            this.Controls.Add(this.rtbResults);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbResults;
        private System.Windows.Forms.Button btnStartFind;
    }
}

