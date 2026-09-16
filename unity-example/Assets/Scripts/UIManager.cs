// UIManager.cs
// 简单显示当前识别标签与置信度（需在场景中有 Text 或 TMP 组件并绑定）
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text labelText;

    public void ShowLabel(string label, float confidence)
    {
        if (labelText != null)
            labelText.text = $"{label} ({confidence:0.00})";
        else
            Debug.Log($"Label: {label} conf {confidence}");
    }
}
