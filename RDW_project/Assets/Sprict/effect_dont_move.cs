using UnityEngine;

public class effect_dont_move : MonoBehaviour
{
    public Transform cameraTransform;
    public float rotateMultiplier = 1.0f; // 回転量の倍率（必要なら調整）

    private float startYaw;
    private Vector3 defaultLocalPos;
    private Quaternion defaultLocalRot;

    private Vector3 nowLocalPos;
    private Quaternion nowLocalRot;

    void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // 初期カメラ向きを記録
        startYaw = cameraTransform.eulerAngles.y;

        // オブジェの初期ローカル位置も保存
        defaultLocalPos = transform.localPosition;
        defaultLocalRot = transform.localRotation;
    }

    float BlurValue = 0;
    Material mat;
    float b;

    void OnEnable()
    {

        mat = this.gameObject.GetComponent<Renderer>().material;
        if (BlurValue == 0)
        {
            BlurValue = mat.GetFloat("_BlurAmount");
        }
        else
        {
            mat.SetFloat("_BlurAmount", BlurValue);
        }

         transform.localPosition = defaultLocalPos;
        transform.localRotation = defaultLocalRot;

        // defaultLocalPos = transform.localPosition;
        // defaultLocalRot = transform.localRotation;

        // 初期カメラ向きを記録
        startYaw = cameraTransform.eulerAngles.y;
    }

    void LateUpdate()
    {
        // 現在のカメラのヨー角
        float currentYaw = cameraTransform.eulerAngles.y;

        // 開始から右に回転した角度（左は0として扱う）
        float deltaYaw = Mathf.DeltaAngle(startYaw, currentYaw);

        // 右向きのときだけ動かす
        float rightYaw = Mathf.Max(0f, deltaYaw);
        Debug.Log($"右向きに{rightYaw}");

        // 回転量を調整（大きくしたい場合は rotateMultiplier を増やす）
        float rotateAngle = rightYaw * rotateMultiplier;

        // カメラ中心を軸に Y 回転させる
        Quaternion rot = Quaternion.AngleAxis(-rotateAngle, Vector3.up);
        //回転じゃなくて綺麗に消そう↓

        /* 1214　なめらかにするためのスプリクト書き途中
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        mat = this.gameObject.GetComponent<Renderer>().material;
        b = mat.GetFloat("_BlurAmount");


        mat.SetFloat("_BlurAmount", b / ((((float)i * (float)i) + 5 * (float)i) / 6f));

        GazeDataRecorder.writeBlur(b, b / ((((float)i * (float)i) + 5 * (float)i) / 6f));
        */

        // 初期位置を回転した座標にする
        transform.localPosition = rot * defaultLocalPos;

        transform.localRotation = rot * defaultLocalRot;
    }
}
