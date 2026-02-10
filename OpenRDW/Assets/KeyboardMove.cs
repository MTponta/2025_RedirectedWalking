using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class KeyboardMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    [Tooltip("補正をかけたい対象の Transform (プレイヤー)")]
    public Transform targetTransform;

    [Tooltip("補正をかけたい対象の Transform (プレイヤー)")]
    public GameObject rig_target;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 vec3 = targetTransform.forward * 0.6f * Time.deltaTime;
            vec3.y = 0;
            rig_target.transform.position += vec3;

        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 vec3 = targetTransform.forward * 0.6f * Time.deltaTime;
            vec3.y = 0;
            rig_target.transform.position -= vec3;

        }
        if (Input.GetKey(KeyCode.A))
        {
            // A を中心に B を回転
            rig_target.transform.RotateAround(
                targetTransform.position,   // 回転の中心（Aの座標）
                Vector3.up,              // 回転軸（Y軸回転）
                -45 * Time.deltaTime// 毎フレームの回転量
            );

            GazeDataRecorder.Y_rotation += -45 * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            // A を中心に B を回転
            rig_target.transform.RotateAround(
                targetTransform.position,   // 回転の中心（Aの座標）
                Vector3.up,              // 回転軸（Y軸回転）
                45 * Time.deltaTime// 毎フレームの回転量
            );

            GazeDataRecorder.Y_rotation += 45 * Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            MoveParent();

            effect_pop[] effects =
           effectObj.GetComponentsInChildren<effect_pop>(true);

            foreach (var effect in effects)
            {
                effect._event = false; // ← bool名に合わせて変更
                effect._eventB = false; // ← bool名に合わせて変更
            }
        }
    }

    public GameObject effectObj;
    public Transform parent;
    public Transform child;

    public Vector3 targetChildWorldPos;
    public Vector3 targetEuler;

    [ContextMenu("位置リセット")]
    void MoveParent()
    {/*
        float rigY = parent.transform.position.y;
        // target の回転を Euler に変換

        // child の現在の回転（world）を Euler に変換
        Vector3 childEuler = child.rotation.eulerAngles;

        // X と Z を child の値で上書き
        targetEuler.x = childEuler.x;
        targetEuler.z = childEuler.z;

        // 合成した回転を Quaternion に戻す
        Quaternion finalChildWorldRot = Quaternion.Euler(targetEuler);

        // ① 親の回転を逆算
        parent.rotation = finalChildWorldRot * Quaternion.Inverse(child.localRotation);

        // ② 親の位置を逆算
        Vector3 childOffsetWorld = parent.rotation * child.localPosition;
        parent.position = targetChildWorldPos - childOffsetWorld;
        parent.position = new Vector3(parent.position.x, rigY, parent.position.z);
   */
        // 子の local は変更しない

        // --- 子の回転を合成 ---
        Vector3 childEuler = child.rotation.eulerAngles;
        targetEuler.x = childEuler.x;
        targetEuler.z = childEuler.z;

        Quaternion targetChildWorldRot = Quaternion.Euler(targetEuler);

        // --- 親の回転を逆算 ---
        parent.rotation =
            targetChildWorldRot * Quaternion.Inverse(child.localRotation);

        // --- 親の位置を逆算（TransformPoint を使うのが最重要） ---
        Vector3 childWorldPosFromParent =
            parent.TransformPoint(child.localPosition);

        Vector3 delta =
            targetChildWorldPos - childWorldPosFromParent;
        
        delta.y = 0;

        parent.position += delta;
    }
}
