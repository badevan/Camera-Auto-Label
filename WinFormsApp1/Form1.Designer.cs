namespace QA_Camera_Auto_Label
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pictureBoxCamera = new PictureBox();
            txtLabel = new TextBox();
            lblStatus = new Label();
            btnCapture = new Button();
            lblFolderLabel = new Label();
            lblConfidence = new Label();
            nudConfidence = new NumericUpDown();
            lblIou = new Label();
            nudIou = new NumericUpDown();
            lblPrompts = new Label();
            txtPrompts = new TextBox();
            splitContainer1 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudConfidence).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudIou).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBoxCamera
            // 
            pictureBoxCamera.Dock = DockStyle.Fill;
            pictureBoxCamera.Location = new Point(0, 0);
            pictureBoxCamera.Name = "pictureBoxCamera";
            pictureBoxCamera.Size = new Size(686, 489);
            pictureBoxCamera.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCamera.TabIndex = 0;
            pictureBoxCamera.TabStop = false;
            // 
            // txtLabel
            // 
            txtLabel.Location = new Point(31, 62);
            txtLabel.Name = "txtLabel";
            txtLabel.Size = new Size(180, 23);
            txtLabel.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(31, 433);
            lblStatus.MaximumSize = new Size(200, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(83, 15);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Camera ready.";
            // 
            // btnCapture
            // 
            btnCapture.Location = new Point(31, 388);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new Size(120, 32);
            btnCapture.TabIndex = 6;
            btnCapture.Text = "Capture / Save";
            btnCapture.UseVisualStyleBackColor = true;
            btnCapture.Click += btnCapture_Click;
            // 
            // lblFolderLabel
            // 
            lblFolderLabel.AutoSize = true;
            lblFolderLabel.Location = new Point(31, 42);
            lblFolderLabel.Name = "lblFolderLabel";
            lblFolderLabel.Size = new Size(82, 15);
            lblFolderLabel.TabIndex = 4;
            lblFolderLabel.Text = "Folder / Label:";
            // 
            // lblConfidence
            // 
            lblConfidence.AutoSize = true;
            lblConfidence.Location = new Point(31, 102);
            lblConfidence.Name = "lblConfidence";
            lblConfidence.Size = new Size(71, 15);
            lblConfidence.TabIndex = 5;
            lblConfidence.Text = "Confidence:";
            // 
            // nudConfidence
            // 
            nudConfidence.DecimalPlaces = 2;
            nudConfidence.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudConfidence.Location = new Point(31, 122);
            nudConfidence.Maximum = new decimal(new int[] { 100, 0, 0, 131072 });
            nudConfidence.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            nudConfidence.Name = "nudConfidence";
            nudConfidence.Size = new Size(120, 23);
            nudConfidence.TabIndex = 2;
            nudConfidence.Value = new decimal(new int[] { 20, 0, 0, 131072 });
            // 
            // lblIou
            // 
            lblIou.AutoSize = true;
            lblIou.Location = new Point(31, 152);
            lblIou.Name = "lblIou";
            lblIou.Size = new Size(28, 15);
            lblIou.TabIndex = 7;
            lblIou.Text = "IoU:";
            // 
            // nudIou
            // 
            nudIou.DecimalPlaces = 2;
            nudIou.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudIou.Location = new Point(31, 172);
            nudIou.Maximum = new decimal(new int[] { 100, 0, 0, 131072 });
            nudIou.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            nudIou.Name = "nudIou";
            nudIou.Size = new Size(120, 23);
            nudIou.TabIndex = 3;
            nudIou.Value = new decimal(new int[] { 80, 0, 0, 131072 });
            // 
            // lblPrompts
            // 
            lblPrompts.AutoSize = true;
            lblPrompts.Location = new Point(31, 207);
            lblPrompts.Name = "lblPrompts";
            lblPrompts.Size = new Size(55, 15);
            lblPrompts.TabIndex = 9;
            lblPrompts.Text = "Prompts:";
            // 
            // txtPrompts
            // 
            txtPrompts.Location = new Point(31, 225);
            txtPrompts.Multiline = true;
            txtPrompts.Name = "txtPrompts";
            txtPrompts.ScrollBars = ScrollBars.Vertical;
            txtPrompts.Size = new Size(294, 157);
            txtPrompts.TabIndex = 4;
            txtPrompts.Text = "phone, laptop, keyboard, mouse, book, notebook, pen, glasses, keys, wallet, bottle, jar, cup, coffee cup, ball, baseball, football";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lblFolderLabel);
            splitContainer1.Panel1.Controls.Add(txtPrompts);
            splitContainer1.Panel1.Controls.Add(txtLabel);
            splitContainer1.Panel1.Controls.Add(lblPrompts);
            splitContainer1.Panel1.Controls.Add(lblStatus);
            splitContainer1.Panel1.Controls.Add(nudIou);
            splitContainer1.Panel1.Controls.Add(btnCapture);
            splitContainer1.Panel1.Controls.Add(lblIou);
            splitContainer1.Panel1.Controls.Add(lblConfidence);
            splitContainer1.Panel1.Controls.Add(nudConfidence);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(pictureBoxCamera);
            splitContainer1.Size = new Size(1034, 489);
            splitContainer1.SplitterDistance = 344;
            splitContainer1.TabIndex = 12;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1034, 489);
            Controls.Add(splitContainer1);
            Name = "Form1";
            Text = "QA Camera Auto Label Software";
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudConfidence).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudIou).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBoxCamera;
        private TextBox txtLabel;
        private Label lblStatus;
        private Button btnCapture;
        private Label lblFolderLabel;
        private Label lblConfidence;
        private NumericUpDown nudConfidence;
        private Label lblIou;
        private NumericUpDown nudIou;
        private Label lblPrompts;
        private TextBox txtPrompts;
        private SplitContainer splitContainer1;
    }
}