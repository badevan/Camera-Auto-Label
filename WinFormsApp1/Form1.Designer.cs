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
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).BeginInit();
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
            lblStatus.Location = new Point(30, 160);
            lblStatus.MaximumSize = new Size(200, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(76, 15);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Camera ready.";
            // 
            // btnCapture
            // 
            btnCapture.Location = new Point(30, 105);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new Size(120, 32);
            btnCapture.TabIndex = 3;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 420);
            Controls.Add(lblFolderLabel);
            Controls.Add(btnCapture);
            Controls.Add(lblStatus);
            Controls.Add(txtLabel);
            Controls.Add(pictureBoxCamera);
            Name = "Form1";
            Text = "QA Camera Auto Label Software";
            ((System.ComponentModel.ISupportInitialize)pictureBoxCamera).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxCamera;
        private TextBox txtLabel;
        private Label lblStatus;
        private Button btnCapture;
        private Label lblFolderLabel;
    }
}