// BarracudaInference.cs
// 使用 Barracuda 在 Unity 内部对摄像头捕获的图像做分类推理（分类器输出 5 类）
using UnityEngine;
using Unity.Barracuda;
using System.Linq;
using System;

public class BarracudaInference : MonoBehaviour
{
    public NNModel modelAsset; // 拖入 Unity 导入的 ONNX (变为 NNModel)
    public CameraCapture cameraCapture; // 拖入 CameraCapture 的引用
    public UIManager uiManager; // 用于显示结果
    public VehicleController vehicleController; // 用于发送控制命令
    public float inferInterval = 0.2f; // 每隔多少秒推理一次

    Model runtimeModel;
    IWorker worker;
    string[] labels = new string[] { "None", "SpeedLimit50", "TurnLeft", "Stop", "PedestrianCrossing" };
    float timer = 0f;

    void Start()
    {
        if (modelAsset == null) { Debug.LogError("请在 Inspector 指定 NNModel (导入的 ONNX)"); return; }
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= inferInterval)
        {
            timer = 0f;
            RunInference();
        }
    }

    void RunInference()
    {
        // 获取 64x64 归一化 RGB
        float[] pixels = cameraCapture.CaptureNormalizedRGB();
        int h = cameraCapture.captureHeight;
        int w = cameraCapture.captureWidth;

        // 构造 Tensor：shape (1, channels, height, width)
        Tensor input = new Tensor(1, 3, h, w);
        // Barracuda indexing: input[b, c, y, x]
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int idx = (y * w + x) * 3;
                input[0, 0, y, x] = pixels[idx + 0]; // R
                input[0, 1, y, x] = pixels[idx + 1]; // G
                input[0, 2, y, x] = pixels[idx + 2]; // B
            }
        }

        worker.Execute(input);
        Tensor output = worker.PeekOutput(); // 假设输出名为 "output" 或唯一输出
        // 读取概率向量
        int n = output.length;
        float maxv = float.NegativeInfinity;
        int maxi = 0;
        for (int i = 0; i < n; i++)
        {
            float v = output[0, i]; // 单 batch 输出访问 (某版本需要 output[0,i]，也可用 output[i])
            if (v > maxv) { maxv = v; maxi = i; }
        }

        string bestLabel = labels[maxi];
        float confidence = Mathf.Exp(maxv) / 1.0f; // NOTE: 如果输出是 logits，可做 softmax；这里仅示例
        uiManager.ShowLabel(bestLabel, confidence);

        // 根据识别结果控制车辆（简化决策）
        switch (bestLabel)
        {
            case "SpeedLimit50":
                vehicleController.SetSpeedLimit(50f);
                break;
            case "Stop":
                vehicleController.StopAtDistance(10f); // 到 10m 时停车
                break;
            case "TurnLeft":
                vehicleController.InitiateTurn(-30f); // 左转目标角度
                break;
            case "PedestrianCrossing":
                vehicleController.SetSpeedLimit(10f);
                break;
            default:
                vehicleController.ClearSpeedLimit();
                break;
        }

        input.Dispose();
        output.Dispose();
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
