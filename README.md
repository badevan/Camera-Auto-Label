# QA Camera Auto Label Software

A lightweight WinForms application for capturing images from a camera and automatically labeling them using YOLO object detection.

Designed for fast dataset creation and QA workflows.

---

## Features

- Live camera feed (OpenCV)
- One-click capture + save workflow
- Automatic folder creation based on label input
- Auto-incrementing filenames (no session tracking required)
- YOLO-based object detection (optional)
- Detection labels appended to filename
- Preview window with bounding boxes before saving

---

## How It Works

1. Enter a label in the textbox (e.g. `10058975`, `FGG4552`, etc.)
2. Click **Capture / Save**
3. The app:
   - Grabs the current camera frame
   - Runs YOLO detection (if model is available)
   - Shows a preview with bounding boxes
4. Choose:
   - **Save** → image is saved with auto-labeling
   - **Cancel** → discard

---

## File Output

Images are saved in a folder based on your label:
