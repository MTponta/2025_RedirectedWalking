using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O2C : MonoBehaviour
{
    [Tooltip("補正をかけたい対象の Transform (プレイヤー)")]
    public Transform targetTransform;

    [Tooltip("補正をかけたい対象の Transform (プレイヤー)")]
    public GameObject rig_target;


    [Tooltip("曲率半径（m）。小さくするほど急に曲がる")]
    public float radius = 0.6f;

    [Tooltip("右回り(true) / 左回り(false)")]
    public bool clockwise = true;

    public bool isO2C =false;

    // 内部：前フレームの座標を使って実速度を近似する（参照なしでスピード推定）
    private Vector3 prevPos;

    void Start()
    {
        if (targetTransform == null)
            targetTransform = this.transform;
        prevPos = targetTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isO2C == false) return;
        // 2) 速度を近似（m/s）
        Vector3 delta = new Vector3(targetTransform.position.x - prevPos.x, 0, targetTransform.position.z - prevPos.z);
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 1e-6f);

        // 速度がほぼ０のときは補正しない
        if (speed < 1e-3f)
        {
            prevPos = targetTransform.position;
            return;
        }

        // 3) 必要な角速度 (rad/s) = v / r
        float angVelRad = speed / Mathf.Max(radius, 1e-6f); // rad/s
        float angDegPerSec = angVelRad * Mathf.Rad2Deg;

        // 4) 右回り（clockwise）なら負の角度（UnityのY回転は左回りが正）
        float sign = clockwise ? -1f : 1f;
        float angleThisFrame = sign * angDegPerSec * Time.deltaTime;

        // 5) プレイヤーの回転に適用（Y軸回転）
        //rig_target.transform.Rotate(0f, angleThisFrame, 0f, Space.World);

        // A を中心に B を回転
        rig_target.transform.RotateAround(
            targetTransform.position,   // 回転の中心（Aの座標）
            Vector3.up,              // 回転軸（Y軸回転）
            angleThisFrame// 毎フレームの回転量
        );
        GazeDataRecorder.Y_rotation += angleThisFrame;

        // 6) 更新
        prevPos = targetTransform.position;
    }
}
