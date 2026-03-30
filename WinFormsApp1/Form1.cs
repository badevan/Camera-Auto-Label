using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(600, 400);

            CheckPythonHelperAtStartup();
            SetupCamera();
        }

        private void CheckPythonHelperAtStartup()
        {
            // Your existing method - unchanged (it's fine)
            try
            {
                string helperPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ScriptsFolder, PythonHelperFileName);
                if (!File.Exists(helperPath))
                {
                    MessageBox.Show($"Python helper script not found!\n\nExpected location:\n{helperPath}",
                        "Missing Python Script", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    pythonHelperAvailable = false;
                    lblStatus.Text = "Python helper not found.";
                    lblStatus.ForeColor = Color.Red;
                    return;
                }

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
                MessageBox.Show($"Python helper failed to run at startup:\n\n{ex.Message}",
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

                lblStatus.Text = pythonHelperAvailable
                    ? "✅ Camera ready • Background detection enabled"
                    : "✅ Camera ready (Python detection unavailable)";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open camera:\n" + ex.Message);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (capture == null || !capture.IsOpened()) return;

            using var frame = new Mat();
            if (capture.Read(frame) && !frame.Empty())
            {
                lastFrame?.Dispose();
                lastFrame = frame.Clone();

                pictureBoxCamera.Image?.Dispose();
                pictureBoxCamera.Image = BitmapConverter.ToBitmap(frame);
            }
        }

        // ====================== CAPTURE BUTTON ======================
        private async void btnCapture_Click(object sender, EventArgs e)
        {
            string label = GetCurrentLabel();
            if (string.IsNullOrWhiteSpace(label))
            {
                MessageBox.Show("Please enter a label name first.");
                return;
            }

            if (lastFrame == null)
            {
                MessageBox.Show("No camera frame available yet.");
                return;
            }

            string folder = GetSessionFolder(label);
            int imageNumber = GetNextImageNumber(folder, label);
            string baseFileName = $"{label}-{imageNumber:000}.jpg";
            string fullPath = Path.Combine(folder, baseFileName);

            try
            {
                Cv2.ImWrite(fullPath, lastFrame);

                lblStatus.Text = $"Saved: {baseFileName}";
                lblStatus.ForeColor = Color.Green;

                // Start background detection + rename (non-blocking)
                if (pythonHelperAvailable)
                {
                    _ = Task.Run(() => BackgroundDetectionAndRename(fullPath, label, imageNumber));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save image:\n" + ex.Message);
            }
        }

        // ====================== BACKGROUND DETECTION & RENAME ======================
        private void BackgroundDetectionAndRename(string originalFilePath, string baseLabel, int imageNumber)
        {
            try
            {
                // Small delay to make sure file is fully written to disk
                Task.Delay(400).Wait();

                using var mat = Cv2.ImRead(originalFilePath);
                if (mat.Empty()) return;

                // Capture values on UI thread before going to background
                float confThreshold = GetConfidenceThreshold();
                float iouThreshold = GetIouThreshold();
                string prompts = GetPromptString();

                var detections = RunPythonHelperInternal(mat, prompts, confThreshold, iouThreshold);

                if (detections == null || detections.Count == 0)
                    return;

                string extraSuffix = GenerateExtraSuffix(detections, confThreshold);
                if (string.IsNullOrEmpty(extraSuffix))
                    return;

                string newFileName = $"{baseLabel}{extraSuffix}-{imageNumber:000}.jpg";
                string folder = Path.GetDirectoryName(originalFilePath)!;
                string newFullPath = Path.Combine(folder, newFileName);

                if (File.Exists(originalFilePath))
                {
                    if (File.Exists(newFullPath))
                        newFullPath = Path.Combine(folder, $"{baseLabel}{extraSuffix}-{imageNumber:000}_v2.jpg");

                    File.Move(originalFilePath, newFullPath, overwrite: true);

                    // Update UI safely
                    this.Invoke((MethodInvoker)(() =>
                    {
                        lblStatus.Text = $"Renamed → {newFileName}";
                        lblStatus.ForeColor = Color.Green;
                    }));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Background rename failed: {ex.Message}");

                this.Invoke((MethodInvoker)(() =>
                {
                    lblStatus.Text = "Image saved (auto-rename failed)";
                    lblStatus.ForeColor = Color.Orange;
                }));
            }
        }

        private List<DetectionResult> RunPythonHelperInternal(Mat frame, string prompts, float conf, float iou)
        {
            if (string.IsNullOrWhiteSpace(prompts))
                return new List<DetectionResult>();

            string helperPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ScriptsFolder, PythonHelperFileName);

            string tempImagePath = Path.Combine(Path.GetTempPath(), $"qa_capture_{Guid.NewGuid():N}.jpg");

            try
            {
                Cv2.ImWrite(tempImagePath, frame);

                var psi = new ProcessStartInfo
                {
                    FileName = PythonExe,
                    Arguments = $"\"{helperPath}\" \"{tempImagePath}\" \"{prompts}\" " +
                               $"{conf.ToString(System.Globalization.CultureInfo.InvariantCulture)} " +
                               $"{iou.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
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

                string cleanJson = CleanPythonOutput(stdout);

                var envelope = JsonSerializer.Deserialize<PythonDetectionEnvelope>(cleanJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (envelope?.Error != null)
                    throw new InvalidOperationException(envelope.Error);

                return envelope?.Detections ?? new List<DetectionResult>();
            }
            finally
            {
                try { if (File.Exists(tempImagePath)) File.Delete(tempImagePath); }
                catch { }
            }
        }

        private string GenerateExtraSuffix(List<DetectionResult> detections, float confidenceThreshold)
        {
            var detectedClasses = detections
                .Where(d => d.Confidence >= confidenceThreshold)
                .Select(d => d.Label)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(4)
                .ToList();

            return detectedClasses.Any()
                ? "_" + string.Join("_", detectedClasses.Select(SanitizeFilePart))
                : "";
        }

        private static string CleanPythonOutput(string output)
        {
            if (string.IsNullOrWhiteSpace(output))
                return "{}";

            string clean = Regex.Replace(output, @"\x1B\[[0-?]*[ -/]*[@-~]", "");
            clean = clean.Trim();

            int start = clean.IndexOf('{');
            int end = clean.LastIndexOf('}');

            if (start >= 0 && end >= start)
                clean = clean.Substring(start, end - start + 1);

            return clean.Trim();
        }

        // ====================== HELPER METHODS ======================
        private string GetCurrentLabel() => txtLabel.Text.Trim();
        private string GetPromptString() => txtPrompts.Text.Trim();
        private string GetSessionFolder(string label)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, label);
            Directory.CreateDirectory(folder);
            return folder;
        }

        private int GetNextImageNumber(string folder, string label)
        {
            return Directory.GetFiles(folder, $"{label}*.jpg").Length + 1;
        }

        private float GetConfidenceThreshold() => (float)nudConfidence.Value;
        private float GetIouThreshold() => (float)nudIou.Value;

        private static string SanitizeFilePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "unknown";

            char[] invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(value.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
            cleaned = cleaned.Replace(' ', '_');
            while (cleaned.Contains("__")) cleaned = cleaned.Replace("__", "_");
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