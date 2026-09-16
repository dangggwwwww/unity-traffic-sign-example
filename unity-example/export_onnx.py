# Auto-generated demo ONNX exporter
# export_onnx.py
# Run: python export_onnx.py
import torch
import torch.nn as nn

class SimpleNet(nn.Module):
    def __init__(self, n_classes=5):
        super().__init__()
        self.net = nn.Sequential(
            nn.Conv2d(3,16,3,stride=2,padding=1), nn.ReLU(),
            nn.Conv2d(16,32,3,stride=2,padding=1), nn.ReLU(),
            nn.AdaptiveAvgPool2d(1), nn.Flatten(),
            nn.Linear(32, n_classes)
        )
    def forward(self,x): return self.net(x)

if __name__ == "__main__":
    model = SimpleNet(n_classes=5)
    model.eval()
    dummy = torch.randn(1,3,64,64)
    torch.onnx.export(model, dummy, "traffic_sign_classifier.onnx",
                      input_names=['input'], output_names=['output'],
                      opset_version=11)
    print("Saved traffic_sign_classifier.onnx")
