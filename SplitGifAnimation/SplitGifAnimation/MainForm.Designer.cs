namespace SplitGifAnimation
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.dropPointLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dropPointLabel
            // 
            this.dropPointLabel.AllowDrop = true;
            this.dropPointLabel.Image = ((System.Drawing.Image)(resources.GetObject("dropPointLabel.Image")));
            this.dropPointLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.dropPointLabel.Location = new System.Drawing.Point(2, -2);
            this.dropPointLabel.Name = "dropPointLabel";
            this.dropPointLabel.Size = new System.Drawing.Size(289, 168);
            this.dropPointLabel.TabIndex = 5;
            this.dropPointLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.dropPointLabel_DragDrop);
            this.dropPointLabel.DragEnter += new System.Windows.Forms.DragEventHandler(this.dropPointLabel_DragEnter);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(292, 166);
            this.Controls.Add(this.dropPointLabel);
            this.Name = "MainForm";
            this.Text = "SplitGifAnimation";
            this.ResumeLayout(false);

        }

        #endregion

        private Label dropPointLabel;
    }
}