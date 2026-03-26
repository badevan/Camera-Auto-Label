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
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudConfidence).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudIou).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxCamera
            // 
            pictureBoxCamera.Location = new Point(250, 20);
            pictureBoxCamera.Name = "pictureBoxCamera";
            pictureBoxCamera.Size = new Size(520, 360);
            pictureBoxCamera.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCamera.TabIndex = 0;
            pictureBoxCamera.TabStop = false;
            // 
            // txtLabel
            // 
            txtLabel.Location = new Point(30, 55);
            txtLabel.Name = "txtLabel";
            txtLabel.Size = new Size(180, 23);
            txtLabel.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(30, 250);
            lblStatus.MaximumSize = new Size(200, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(76, 15);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Camera ready.";
            // 
            // btnCapture
            // 
            btnCapture.Location = new Point(30, 195);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new Size(120, 32);
            btnCapture.TabIndex = 5;
            btnCapture.Text = "Capture / Save";
            btnCapture.UseVisualStyleBackColor = true;
            btnCapture.Click += btnCapture_Click;
            // 
            // lblFolderLabel
            // 
            lblFolderLabel.AutoSize = true;
            lblFolderLabel.Location = new Point(30, 35);
            lblFolderLabel.Name = "lblFolderLabel";
            lblFolderLabel.Size = new Size(92, 15);
            lblFolderLabel.TabIndex = 4;
            lblFolderLabel.Text = "Folder / Label:";
            // 
            // lblConfidence
            // 
            lblConfidence.AutoSize = true;
            lblConfidence.Location = new Point(30, 95);
            lblConfidence.Name = "lblConfidence";
            lblConfidence.Size = new Size(67, 15);
            lblConfidence.TabIndex = 5;
            lblConfidence.Text = "Confidence:";
            // 
            // nudConfidence
            // 
            nudConfidence.DecimalPlaces = 2;
            nudConfidence.Increment = 0.01M;
            nudConfidence.Location = new Point(30, 115);
            nudConfidence.Maximum = 1.00M;
            nudConfidence.Minimum = 0.01M;
            nudConfidence.Name = "nudConfidence";
            nudConfidence.Size = new Size(120, 23);
            nudConfidence.TabIndex = 2;
            nudConfidence.Value = 0.40M;
            // 
            // lblIou
            // 
            lblIou.AutoSize = true;
            lblIou.Location = new Point(30, 145);
            lblIou.Name = "lblIou";
            lblIou.Size = new Size(28, 15);
            lblIou.TabIndex = 7;
            lblIou.Text = "IoU:";
            // 
            // nudIou
            // 
            nudIou.DecimalPlaces = 2;
            nudIou.Increment = 0.01M;
            nudIou.Location = new Point(30, 165);
            nudIou.Maximum = 1.00M;
            nudIou.Minimum = 0.01M;
            nudIou.Name = "nudIou";
            nudIou.Size = new Size(120, 23);
            nudIou.TabIndex = 3;
            nudIou.Value = 0.80M;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 420);
            Controls.Add(nudIou);
            Controls.Add(lblIou);
            Controls.Add(nudConfidence);
            Controls.Add(lblConfidence);
            Controls.Add(lblFolderLabel);
            Controls.Add(btnCapture);
            Controls.Add(lblStatus);
            Controls.Add(txtLabel);
            Controls.Add(pictureBoxCamera);
            Name = "Form1";
            Text = "QA Camera Auto Label Software";
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudConfidence).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudIou).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
    }
}