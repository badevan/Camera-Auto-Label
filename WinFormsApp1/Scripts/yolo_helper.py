import sys
import json
import os
from ultralytics import YOLO

# ========================= CONFIG =========================
MODEL_PATH = "yoloe-26m-seg.pt"   # Must be in the same 'scripts' folder
# =========================================================

# Suppress as much Ultralytics output as possible
os.environ["YOLO_VERBOSE"] = "False"
os.environ["ULTRALYTICS_VERBOSE"] = "False"

try:
    # Load model quietly
    model = YOLO(MODEL_PATH, verbose=False)
    print("YOLOE-26m-seg model loaded successfully", file=sys.stderr)
except Exception as e:
    print(f"Failed to load model: {e}", file=sys.stderr)
    print(json.dumps({"error": f"Failed to load model: {str(e)}"}))
    sys.exit(1)


def run_detection(image_path, prompts, conf=0.25, iou=0.7):
    """Run open-vocabulary detection with YOLOE"""
    try:
        # Set the custom classes (prompts)
        model.set_classes(prompts)

        results = model.predict(
            source=image_path,
            conf=conf,
            iou=iou,
            verbose=False,      # This helps but is not enough alone
            device="cpu",       # Change to "0" if you have GPU and want speed
            stream=False
        )

        detections = []
        if not results or len(results) == 0:
            return detections

        r = results[0]

        if r.boxes is None or len(r.boxes) == 0:
            return detections

        for box in r.boxes:
            cls_id = int(box.cls[0])
            label = r.names[cls_id]
            confidence = float(box.conf[0])
            x1, y1, x2, y2 = box.xyxy[0].tolist()

            detections.append({
                "Label": label,
                "Confidence": confidence,
                "X": int(x1),
                "Y": int(y1),
                "Width": int(x2 - x1),
                "Height": int(y2 - y1)
            })

        return detections

    except Exception as e:
        print(f"Detection error: {e}", file=sys.stderr)
        return []


def main():
    # ====================== STARTUP TEST ======================
    if len(sys.argv) > 1 and sys.argv[1] == "--test":
        print(json.dumps({
            "Prompts": ["test"],
            "Count": 0,
            "Detections": [],
            "Error": None,
            "message": "YOLOE-26m-seg is ready"
        }))
        return

    # ====================== NORMAL DETECTION ======================
    if len(sys.argv) < 3:
        print(json.dumps({"error": "Not enough arguments. Need: image_path prompts [conf] [iou]"}))
        return

    image_path = sys.argv[1]
    prompts_raw = sys.argv[2]

    try:
        conf = float(sys.argv[3]) if len(sys.argv) > 3 else 0.25
        iou = float(sys.argv[4]) if len(sys.argv) > 4 else 0.7
    except (ValueError, IndexError):
        conf = 0.25
        iou = 0.7

    # Parse prompts (comma-separated)
    prompts = [p.strip() for p in prompts_raw.split(",") if p.strip()]

    if not prompts:
        print(json.dumps({"error": "No valid prompts provided."}))
        return

    try:
        detections = run_detection(image_path, prompts, conf, iou)

        output = {
            "Prompts": prompts,
            "Count": len(detections),
            "Detections": detections,
            "Error": None
        }
        # IMPORTANT: Only pure JSON on stdout
        print(json.dumps(output))

    except Exception as e:
        error_output = {
            "Prompts": prompts,
            "Count": 0,
            "Detections": [],
            "Error": str(e)
        }
        print(json.dumps(error_output))


if __name__ == "__main__":
    main()