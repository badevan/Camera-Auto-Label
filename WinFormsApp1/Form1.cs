using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Point = OpenCvSharp.Point;
using Size = System.Drawing.Size;

namespace QA_Camera_Auto_Label
{
    public partial class Form1 : Form
    {
        private VideoCapture? capture;
        private System.Windows.Forms.Timer? timer;
        private Mat? lastFrame;

        private const string PythonExe = "python";
        private const string PythonHelperFileName = "yolo_helper.py";
        private const string ScriptsFolder = "scripts";

        private bool pythonHelperAvailable = false;

        public Form1()
        {
            InitializeComponent();
            Text = "QA Camera Auto Label Software";
            FormBorderStyle = FormBorderStyle.Sizable;        // Changed from FixedSingle
            MinimumSize = new Size(600, 400);

            CheckPythonHelperAtStartup();
            SetupCamera();
        }

        private void CheckPythonHelperAtStartup()
        {
            try
            {
                string helperPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    ScriptsFolder,
                    PythonHelperFileName);

                if (!File.Exists(helperPath))
                {
                    MessageBox.Show($"Python helper script not found!\n\nExpected location:\n{helperPath}\n\nPlease place yolo_helper.py in the 'scripts' folder.",
                        "Missing Python Script", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    pythonHelperAvailable = false;
                    lblStatus.Text = "Python helper not found.";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

                // Test run with --test flag
                var psi = new ProcessStartInfo
                {
                    FileName = PythonExe,
                    Arguments = $"\"{helperPath}\" --test",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                using var process = Process.Start(psi) ?? throw new Exception("Failed to start Python process.");

                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    pythonHelperAvailable = true;
                    lblStatus.Text = "✅ Python helper ready.";
                    lblStatus.ForeColor = Color.Green;
                }
                else
                {
                    throw new Exception($"Python exited with code {process.ExitCode}:\n{stderr}");
                }
            }
            catch (Exception ex)
            {
                pythonHelperAvailable = false;
                lblStatus.Text = "Python helper not available.";
                lblStatus.ForeColor = Color.Red;

                MessageBox.Show($"Python helper failed to run at startup:\n\n{ex.Message}\n\n" +
                               "Make sure:\n" +
                               "• Python is installed and 'python' is in your PATH\n" +
                               "• yolo_helper.py is located in the 'scripts' folder\n" +
                               "• All required Python packages are installed (ultralytics, etc.)",
                    "Python Helper Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

                if (!pythonHelperAvailable)
                    lblStatus.Text = "Camera ready. (Python detection unavailable)";
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

        private string GetCurrentLabel() => txtLabel.Text.Trim();

        private string GetPromptString() => txtPrompts.Text.Trim();

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

        private float GetConfidenceThreshold() => (float)nudConfidence.Value;

        private float GetIouThreshold() => (float)nudIou.Value;

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

            float confidenceThreshold = GetConfidenceThreshold();
            float iouThreshold = GetIouThreshold();

            if (pythonHelperAvailable)
            {
                try
                {
                    var pythonDetections = RunPythonHelper(lastFrame, confidenceThreshold, iouThreshold);

                    var detectedClasses = pythonDetections
                        .Where(d => d.Confidence >= confidenceThreshold)
                        .Select(d => d.Label)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .Take(4)
                        .ToList();

                    string extraSuffix = detectedClasses.Any()
                        ? "_" + string.Join("_", detectedClasses.Select(SanitizeFilePart))
                        : "";

                    ShowDetectionPreview(lastFrame, pythonDetections, label, extraSuffix);
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    MessageBox.Show($"Python detection failed:\n{ex.Message}\n\nImage will be saved without auto-detection.",
                        "Detection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // Final fallback: save without detection
            PerformSave(label, "");
            lblStatus.Text = "Python unavailable - Image saved without detection.";
            lblStatus.ForeColor = Color.Orange;
        }

        private List<DetectionResult> RunPythonHelper(Mat frame, float confidenceThreshold, float iouThreshold)
        {
            string helperPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                ScriptsFolder,
                PythonHelperFileName);

            if (!File.Exists(helperPath))
                throw new FileNotFoundException("Python helper script not found.", helperPath);

            string tempImagePath = Path.Combine(Path.GetTempPath(), $"qa_capture_{Guid.NewGuid():N}.jpg");

            try
            {
                Cv2.ImWrite(tempImagePath, frame);

                string prompts = GetPromptString();
                if (string.IsNullOrWhiteSpace(prompts))
                    throw new InvalidOperationException("Please enter at least one prompt for Python detection.");

                var psi = new ProcessStartInfo
                {
                    FileName = PythonExe,
                    Arguments = $"\"{helperPath}\" \"{tempImagePath}\" \"{prompts}\" " +
                               $"{confidenceThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture)} " +
                               $"{iouThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                using var process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start Python process.");

                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new InvalidOperationException($"Python helper failed (exit code {process.ExitCode}):\n{stderr}");

                if (string.IsNullOrWhiteSpace(stdout))
                    throw new InvalidOperationException("Python helper returned no output.");

                var envelope = JsonSerializer.Deserialize<PythonDetectionEnvelope>(stdout,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (envelope?.Error != null)
                    throw new InvalidOperationException(envelope.Error);

                return envelope?.Detections ?? new List<DetectionResult>();
            }
            finally
            {
                try { if (File.Exists(tempImagePath)) File.Delete(tempImagePath); }
                catch { /* ignore temp file cleanup errors */ }
            }
        }

        private void ShowDetectionPreview(Mat originalFrame, IReadOnlyList<DetectionResult> detections, string label, string extraSuffix)
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
            float confidenceThreshold = GetConfidenceThreshold();

            foreach (var det in detections.Where(d => d.Confidence >= confidenceThreshold))
            {
                var rect = new Rect(det.X, det.Y, det.Width, det.Height);
                Cv2.Rectangle(displayMat, rect, Scalar.Red, 3);
                Cv2.PutText(
                    displayMat,
                    $"{det.Label} {det.Confidence:P0}",
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

            var btnSave = new Button { Text = "Save Image", Width = 140, Height = 50 };
            var btnCancel = new Button { Text = "Cancel", Width = 120, Height = 50 };

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
                lblStatus.Text = $"Saved: {fileName} | Conf: {GetConfidenceThreshold():0.00} | IoU: {GetIouThreshold():0.00}";

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

        private static string SanitizeFilePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "unknown";

            char[] invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(value.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            cleaned = cleaned.Replace(' ', '_');
            while (cleaned.Contains("__"))
                cleaned = cleaned.Replace("__", "_");

            return cleaned.Trim('_');
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timer?.Stop();
            capture?.Dispose();
            lastFrame?.Dispose();
            pictureBoxCamera.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }

    // ====================== Helper Classes ======================
    public class DetectionResult
    {
        public string Label { get; set; } = "";
        public float Confidence { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class PythonDetectionEnvelope
    {
        public List<string>? Prompts { get; set; }
        public int Count { get; set; }
        public List<DetectionResult>? Detections { get; set; }
        public string? Error { get; set; }
    }
}