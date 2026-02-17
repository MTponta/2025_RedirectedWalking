using UnityEngine;
using System.IO; // ファイル書き出しに必要
using System.Text; // 文字コードの指定に必要
using System;
using static setting;
using Valve.VR.InteractionSystem;

/// <summary>
/// SteamVR使用時にHMDの視線方向をCSVファイルに出力するクラス
/// </summary>
public class GazeDataRecorder : MonoBehaviour
{

    [Header("出力先 C:\\Users\\[ユーザ名]\\AppData\\LocalLow\\DefaultCompany\\OpenRDW\\GazeLogs")]

    public string FileName = "NAME";

    public setting.effectType E1;
    public setting.effectType E2;


    public static float Y_rotation = 0;

    [Header("↓いじらなくてよい")]
    [Tooltip("視点を取得するカメラ[CameraRig]->Camera(head)->Camera")]
    public Transform headTransform;

    private string fileName = "gaze_data110415.csv";

    // ファイル書き出し用の変数
    private StreamWriter streamWriter = null;

    // ファイルのフルパス
    private string filePath;
    private float o2cRadi = 0;
    public static float Blur = 0;
    public static float Red_point = 0;
    public static float Saturation = 0;
    public static float LRdiff = 0;

    public bool isRecord = false;
    public GameObject effects;
    public setting _setting;
    public O2C o2c;

    public static void writeBlur(float max, float now)
    {
        Blur = now / max;
    }

    public static void writeRed_point(float max, float now)
    {
        Red_point = now / max;
    }

    public static void writeSaturation(float max, float now)
    {
        Saturation = now / max;
    }


    public static void writeLRdiff(float max, float now)
    {
        LRdiff = now / max;
    }

    [ContextMenu("開始A右")]
    public void startA()
    {

        o2c.radius = o2cRadi;
        // アプリケーション終了時にファイルを閉じる (非常に重要！)
        if (streamWriter != null)
        {
            streamWriter.Close();
            Debug.Log($"CSVファイルの出力を終了しました。パス: {filePath}");
        }
        string str = "_" + name + "_A";
        str += _setting.type;
        str += _setting.typeB;
        Start_record(str);
        //effects.SetActive(false);

        effects.SetActive(true);
    }

    [ContextMenu("開始B左")]
    public void startB()
    {

        o2c.radius = o2cRadi;
        // アプリケーション終了時にファイルを閉じる (非常に重要！)
        if (streamWriter != null)
        {
            streamWriter.Close();
            Debug.Log($"CSVファイルの出力を終了しました。パス: {filePath}");
        }
        string str = "_" + name + "_B";
        str += _setting.type;
        str += _setting.typeB;
        Start_record(str);
        o2c.clockwise = false;
        effects.SetActive(true);
    }


    [ContextMenu("終了&次の資格効果")]
    public void Finish()
    {
        o2c.radius = 9999;
        if (streamWriter != null)
        {
            streamWriter.Close();
            Debug.Log($"CSVファイルの出力を終了しました。パス: {filePath}");
        }

    }

    void Start()
    {
        o2cRadi = o2c.radius;
    }

    public void Start_record(string str)
    {
        o2c.isO2C = true;
        fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}"  + str + ".csv";

        // HMDのTransformが設定されていなければ、メインカメラを探す
        if (headTransform == null)
        {
            Debug.Log("headTransformが設定されていません。Camera.mainを探します。");
            // SteamVR環境ではCamera.mainが期待通りにHMDのカメラを指すとは限らないため、
            // インスペクターから手動で設定することを強く推奨します。
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                headTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogError("HMDのカメラが見つかりませんでした。インスペクターから headTransform を設定してください。");
                // エラーが発生したらこのコンポーネントを無効化する
                this.enabled = false;
                return;
            }
        }

        string directoryPath = Path.Combine(Application.persistentDataPath, "GazeLogs");
        Directory.CreateDirectory(directoryPath);

        // 保存先のパスを決定する (C:/Users/[ユーザー名]/AppData/LocalLow/[会社名]/[製品名]/gaze_data.csv のような場所)
        filePath = Path.Combine(directoryPath, fileName);

        // フォルダが無ければ作る
        //Directory.CreateDirectory(filePath);

        // ファイルを新規作成（上書きモード）し、StreamWriterを初期化する
        // 文字コードをUTF-8に指定することで文字化けを防ぐ
        streamWriter = new StreamWriter(filePath, false, Encoding.UTF8);

        // CSVのヘッダー行を書き込む
        string[] headers = {
            "Timestamp",      // 経過時間
            "rigX",
            "rigY",
            "rigZ",
            "rigAngleX",
            "rigAngleY",
            "rigAngleZ",
            "PosX",       // 視線方向ベクトルのX成分
            "PosY",       // 視線方向ベクトルのY成分
            "PosZ",       // 視線方向ベクトルのZ成分 
            "EulerAngleX",    // 視線のオイラー角X (ピッチ)
            "EulerAngleY",    // 視線のオイラー角Y (ヨー)
            "EulerAngleZ",     // 視線のオイラー角Z (ロール)
            "O2CRotation",
            "Blur",
            "Red_point",
            "Saturation",
            "LRDdiff"
        };
        string headerLine = string.Join(",", headers);
        streamWriter.WriteLine(headerLine);

        Debug.Log($"CSVファイルの出力を開始します。パス: {filePath}");
        isRecord = true;
    }

    [ContextMenu("反転")]
    public void popEf()
    {
        effects.SetActive(true);
        o2c.clockwise = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Finish();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            startA();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            startB();
        }

        _setting.type = E1;
        _setting.typeB = E2;

        if (isRecord == false) return;
        // headTransformが正しく設定されている場合のみ実行
        if (headTransform != null && streamWriter != null)
        {
            // 現在の経過時間を取得
            float timestamp = Time.time;

            // 視線の方向ベクトル（ワールド座標系）を取得
            //Vector3 forward = headTransform.local.forward;

            // 視線のオイラー角（ワールド座標系）を取得
            Vector3 eulerAngles = headTransform.localEulerAngles;


            Vector3 pos = headTransform.position;
            Vector3 rigPos = this.transform.position;

            Vector3 rigAngle = this.transform.localEulerAngles;

            // 書き込むデータを作成
            string[] data = {
                timestamp.ToString("F4"),    // 小数点以下4桁まで
                rigPos.x.ToString("F6"),
                rigPos.y.ToString("F6"),
                rigPos.z.ToString("F6"),
                rigAngle.x.ToString("F6"),
                rigAngle.y.ToString("F6"),
                rigAngle.z.ToString("F6"),
                pos.x.ToString("F6"),
                pos.y.ToString("F6"),
                pos.z.ToString("F6"),
                eulerAngles.x.ToString("F6"),
                eulerAngles.y.ToString("F6"),
                eulerAngles.z.ToString("F6"),
                Y_rotation.ToString("F6"),
                Blur.ToString("F6"),
                Red_point.ToString("F6"),
                Saturation.ToString("F6"),
                LRdiff.ToString("F6")
            };
            string dataLine = string.Join(",", data);

            // ファイルに1行書き込む
            streamWriter.WriteLine(dataLine);
            Debug.Log(rigPos + pos*5 +"　と　" + (rigPos + this.transform.rotation * Vector3.Scale(this.transform.lossyScale, pos))+"　と　"　+ headTransform.TransformPoint(headTransform.position));
        }
    }

    void OnApplicationQuit()
    {
        // アプリケーション終了時にファイルを閉じる (非常に重要！)
        if (streamWriter != null)
        {
            streamWriter.Close();
            Debug.Log($"CSVファイルの出力を終了しました。パス: {filePath}");
        }
    }


}