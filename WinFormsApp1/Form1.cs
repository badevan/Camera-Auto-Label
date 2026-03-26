using OpenCvSharp;
using OpenCvSharp.Extensions;
using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.ExecutionProvider.Cpu;
using YoloDotNet.Models;
using Point = OpenCvSharp.Point;
using Size = System.Drawing.Size;

namespace QA_Camera_Auto_Label
{
    public partial class Form1 : Form
    {
        private VideoCapture? capture;
        private System.Windows.Forms.Timer? timer;
        private Mat? lastFrame;
        private Yolo? detector;

        public Form1()
        {
            InitializeComponent();
            Text = "QA Camera Auto Label Software";
            FormBorderStyle = FormBorderStyle.FixedSingle;

            SetupCamera();
        }

        private static SKBitmap BitmapToSkBitmap(Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;
            return SKBitmap.Decode(ms);
        }

        private void SetupCamera()
        {
            try
            {
                capture = new VideoCapture(0);
                capture.Set(VideoCaptureProperties.FrameWidth, 1280);
                capture.Set(VideoCaptureProperties.FrameHeight, 720);

                timer = new System.Windows.Forms.Timer { Interval = 33 };
                timer.Tick += Timer_Tick;
                timer.Start();

                lblStatus.Text = "Camera ready.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open camera:\n" + ex.Message);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (capture == null || !capture.IsOpened())
                return;

            using var frame = new Mat();
            if (capture.Read(frame) && !frame.Empty())
            {
                lastFrame?.Dispose();
                lastFrame = frame.Clone();

                pictureBoxCamera.Image?.Dispose();
                pictureBoxCamera.Image = BitmapConverter.ToBitmap(frame);
            }
        }

        private void EnsureDetectorLoaded()
        {
            if (detector != null)
                return;

            try
            {
                string modelPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Models",
                    "yolo26n.onnx");

                if (!File.Exists(modelPath))
                {
                    MessageBox.Show($"Model not found at:\n{modelPath}\n\nPlease place your ONNX file in the Models folder.");
                    return;
                }

                detector = new Yolo(new YoloOptions
                {
                    ExecutionProvider = new CpuExecutionProvider(modelPath),
                    ImageResize = ImageResize.Proportional
                });

                lblStatus.Text = "YOLO detector loaded.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load YOLO detector:\n" + ex.Message);
            }
        }

        private string GetCurrentLabel()
        {
            return txtLabel.Text.Trim();
        }

        private string GetSessionFolder(string label)
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string folder = Path.Combine(exeDir, label);
            Directory.CreateDirectory(folder);
            return folder;
        }

        private int GetNextImageNumber(string folder, string label)
        {
            return Directory.GetFiles(folder, $"{label}*.jpg").Length + 1;
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            string label = GetCurrentLabel();
            if (string.IsNullOrWhiteSpace(label))
            {
                MessageBox.Show("Please enter a label name first.");
                return;
            }

            if (lastFrame == null)
            {
                MessageBox.Show("No camera frame available.");
                return;
            }

            EnsureDetectorLoaded();
            if (detector == null)
            {
                PerformSave(label, "");
                return;
            }

            using var bmp = BitmapConverter.ToBitmap(lastFrame);
            using var skBitmap = BitmapToSkBitmap(bmp);

            var results = detector.RunObjectDetection(skBitmap, confidence: 0.7f, iou: 0.9f);

            var detectedClasses = results
                .Where(r => r.Confidence > 0.35)
                .Select(r => r.Label.Name)
                .Distinct()
                .Take(4)
                .ToList();

            string extraSuffix = detectedClasses.Any()
                ? "_" + string.Join("_", detectedClasses)
                : "";

            ShowDetectionPreview(lastFrame, results, label, extraSuffix);
        }

        private void ShowDetectionPreview(Mat originalFrame, IReadOnlyList<ObjectDetection> detections, string label, string extraSuffix)
        {
            using var previewForm = new Form
            {
                Text = "Detection Preview - Save or Cancel?",
                Size = new Size(950, 750),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var previewPic = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            using var displayMat = originalFrame.Clone();

            foreach (var det in detections.Where(d => d.Confidence > 0.35))
            {
                var rect = new Rect(
                    det.BoundingBox.Left,
                    det.BoundingBox.Top,
                    det.BoundingBox.Width,
                    det.BoundingBox.Height);

                Cv2.Rectangle(displayMat, rect, Scalar.Red, 3);
                Cv2.PutText(
                    displayMat,
                    $"{det.Label.Name} {det.Confidence:P0}",
                    new Point(rect.X, Math.Max(20, rect.Y - 10)),
                    HersheyFonts.HersheySimplex,
                    0.8,
                    Scalar.Yellow,
                    2);
            }

            previewPic.Image = BitmapConverter.ToBitmap(displayMat);

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.RightToLeft
            };

            var btnSave = new Button
            {
                Text = "Save Image",
                Width = 140,
                Height = 50
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Width = 120,
                Height = 50
            };

            btnSave.Click += (s, ev) =>
            {
                PerformSave(label, extraSuffix);
                previewForm.Close();
            };

            btnCancel.Click += (s, ev) => previewForm.Close();

            panel.Controls.Add(btnCancel);
            panel.Controls.Add(btnSave);

            previewForm.Controls.Add(previewPic);
            previewForm.Controls.Add(panel);

            previewForm.ShowDialog(this);
        }

        private void PerformSave(string label, string extraSuffix)
        {
            if (lastFrame == null)
            {
                MessageBox.Show("No image available to save.");
                return;
            }

            string folder = GetSessionFolder(label);
            int nextNumber = GetNextImageNumber(folder, label);

            string fileName = $"{label}{extraSuffix}-{nextNumber:000}.jpg";
            string fullPath = Path.Combine(folder, fileName);

            try
            {
                Cv2.ImWrite(fullPath, lastFrame);
                lblStatus.Text = $"Saved: {fileName}";

                if (!string.IsNullOrWhiteSpace(extraSuffix))
                {
                    lblStatus.Text += Environment.NewLine +
                                      $"Detected: {extraSuffix.TrimStart('_').Replace("_", ", ")}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed:\n" + ex.Message);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timer?.Stop();
            capture?.Dispose();
            detector?.Dispose();
            lastFrame?.Dispose();
            pictureBoxCamera.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}