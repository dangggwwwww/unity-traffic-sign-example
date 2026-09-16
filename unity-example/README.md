# Unity Traffic Sign Example

This branch contains a minimal Unity example demonstrating how to integrate a simple ONNX classifier with Unity (Barracuda) to recognize traffic signs in a virtual driving scene. Target Unity version: 2023.2.1.

Contents (in this branch):
- unity-example/README.md (this file)
- unity-example/export_onnx.py (Python script to produce a small demo ONNX model)
- unity-example/Assets/Models/ (place generated ONNX model here: traffic_sign_classifier.onnx)
- unity-example/Assets/Scripts/CameraCapture.cs
- unity-example/Assets/Scripts/BarracudaInference.cs
- unity-example/Assets/Scripts/VehicleController.cs
- unity-example/Assets/Scripts/UIManager.cs
- unity-example/.gitignore

Purpose
---
This demo provides a minimal end-to-end pipeline:
1. Generate demo ONNX classifier (or replace with your trained model).
2. Import model into Unity and use Barracuda for inference.
3. Capture camera image in Unity, run inference, show label on UI and control a simple vehicle.

Notes
---
- The provided ONNX generator (export_onnx.py) creates a very small toy model with 5 classes: None, SpeedLimit50, TurnLeft, Stop, PedestrianCrossing. This model is for functional testing only — train a real detector/classifier for production.
- For detection (bbox) models like YOLO, you will need different post-processing (decode anchors, NMS). This demo is classification-based for simplicity.

How to generate the ONNX model
---
1. Install Python and PyTorch (CPU):
   pip install torch
2. Run in this directory (unity-example/):
   python export_onnx.py
3. The script writes traffic_sign_classifier.onnx to the current directory. Move it into Unity project at:
   Assets/Models/traffic_sign_classifier.onnx

How to import into Unity 2023.2.1
---
1. Open or create a Unity 2023.2.1 project.
2. In Package Manager, install "Barracuda" (com.unity.barracuda).
3. Copy the contents of unity-example/Assets into your Unity project's Assets/ folder.
4. In Unity, the ONNX file will be imported as a NNModel asset (if Barracuda installed).
5. Create a simple scene:
   - Add a GameObject named Vehicle with a Rigidbody and attach VehicleController.cs.
   - Add a Camera as child of Vehicle (or placed at vehicle front), attach CameraCapture.cs (set captureWidth=64, captureHeight=64).
   - Create an empty GameObject (e.g., InferenceManager) and attach BarracudaInference.cs. Assign fields: ModelAsset (the imported NNModel), CameraCapture reference, UIManager reference, VehicleController reference.
   - Create Canvas -> Text (or TextMeshPro) and attach UIManager.cs to a GameObject, drag Text to UIManager.labelText.
6. Press Play. The camera frames will be captured and fed to the model at the infer interval and the UI will update. The VehicleController will change target speed/turn based on labels.

Exporting .unitypackage and Release (manual steps)
---
I cannot create binary .unitypackage or GitHub Releases from here. To create a .unitypackage for distribution:
1. In Unity Editor, select the Assets folders/files you want to include (e.g., Assets/Scripts, Assets/Models, Scenes/...).
2. Menu: Assets -> Export Package... -> Select dependencies -> Export to file, e.g., unity-traffic-sign-example.unitypackage
3. Upload the .unitypackage as a GitHub Release asset on your repository's Releases page.

If you want, after you export the .unitypackage you can upload it to the repository as a Release and I can add Release notes or help script the process.

Contact / Next steps
---
If you want me to also prepare a pre-built small ONNX binary or help with creating a .unitypackage locally, tell me and I'll provide scripts and exact steps.
