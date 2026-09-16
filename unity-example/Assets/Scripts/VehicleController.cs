// VehicleController.cs
// 非物理完整车辆，仅演示：目标速度、平滑加/减速、简单转向
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    Rigidbody rb;
    public float maxAccel = 5f;
    public float maxSpeed = 100f;
    float targetSpeed = 0f;
    float currentSpeed = 0f;
    float speedLimit = Mathf.Infinity;
    float turnSteer = 0f; // 角速度或转向设定（简化）

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void Update()
    {
        // 平滑速度逼近 targetSpeed，但不超过 speedLimit
        float effectiveTarget = Mathf.Min(targetSpeed, speedLimit);
        currentSpeed = Mathf.MoveTowards(currentSpeed, effectiveTarget, maxAccel * Time.deltaTime);
        Vector3 forward = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forward);

        // 简单转向：旋转 transform
        if (Mathf.Abs(turnSteer) > 0.1f)
        {
            transform.Rotate(0, turnSteer * Time.deltaTime, 0);
            // 逐步衰减 turnSteer
            turnSteer = Mathf.MoveTowards(turnSteer, 0f, 30f * Time.deltaTime);
        }
    }

    // 对外接口
    public void SetTargetSpeed(float kmh) { targetSpeed = kmh * (1000f/3600f); } // km/h -> m/s
    public void SetSpeedLimit(float kmh) { speedLimit = kmh * (1000f/3600f); }
    public void ClearSpeedLimit() { speedLimit = Mathf.Infinity; }
    public void StopAtDistance(float dist) {
        // 这里示例逻辑：立即把目标速度置 0（更实际应检测距离）
        SetTargetSpeed(0f);
    }
    public void InitiateTurn(float degPerSec) { turnSteer = degPerSec; }
}
