// CameraCapture.cs
// 负责把指定 Camera 的视野按指定分辨率渲染到 Texture2D 并提供像素数据给外部调用
using UnityEngine;
using System;

[RequireComponent(typeof(Camera))]
public class CameraCapture : MonoBehaviour
{
    public int captureWidth = 64;
    public int captureHeight = 64;
    Camera cam;
    RenderTexture rt;
    Texture2D tex;

    void Awake() {
        cam = GetComponent<Camera>();
        rt = new RenderTexture(captureWidth, captureHeight, 24, RenderTextureFormat.ARGB32);
        tex = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
    }

    // 获取归一化像素数组 (r,g,b) 每通道 0..1，按 x,y 顺序返回 length = w*h*3
    public float[] CaptureNormalizedRGB()
    {
        var prev = cam.targetTexture;
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        Color[] cols = tex.GetPixels();
        float[] data = new float[cols.Length * 3];
        for (int i = 0; i < cols.Length; i++) {
            data[i*3 + 0] = cols[i].r;
            data[i*3 + 1] = cols[i].g;
            data[i*3 + 2] = cols[i].b;
        }

        cam.targetTexture = prev;
        RenderTexture.active = null;
        return data;
    }
}
