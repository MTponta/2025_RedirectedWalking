using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class effect_manager : MonoBehaviour
{
    public GameObject effectA;
    public GameObject effectB;
    public GameObject effectC;
    public GameObject effectD;

    // Start is called before the first frame update
    void Start()
    {

        // effectA.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool eA = false;
    async public void EffectA()
    {
        //フラグ管理
        if (eA) return;
        eA = true;

        //視覚効果の発生開始を記録
        GazeDataRecorder.writeRed_point(1, 1);

        //点滅
        for (int i = 0; i < 4; i++)
        {
            effectA.SetActive(true);
            await Task.Delay(2);
            effectA.SetActive(false);

            await Task.Delay(80);

        }

        //視覚効果の終了を記録
        GazeDataRecorder.writeRed_point(1, 0);
        eA = false;
    }

    private bool eB = false;
    async public void EffectB()
    {
        //フラグ管理
        if (eB) return;
        eB = true;

        float b;
        Material mat;

        effectB.SetActive(false);
        effectB.SetActive(true);
        mat = effectB.GetComponent<Renderer>().material;
        b = mat.GetFloat("_BlurAmount");

        for (int i = 7; i > 0; i--)
        {

            //ブラーの強さを初期値に向けて変化させていく
            mat.SetFloat("_BlurAmount", b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            //視覚効果の発生開始を記録
            GazeDataRecorder.writeBlur(b, b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            await Task.Delay(100);
        }

        await Task.Delay(1200);
        for (int i = 1; i <= 7; i++)
        {
            //ブラーの強さを初期値に向けて変化させていく
            mat.SetFloat("_BlurAmount", b / ((((float)i * (float)i) + 9 * (float)i) / 10f));


            //視覚効果の終了を記録
            GazeDataRecorder.writeBlur(b, b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            await Task.Delay(150);
        }

        eB = false;

        mat.SetFloat("_BlurAmount", b);
        effectB.SetActive(false);
        GazeDataRecorder.writeBlur(b, 0);
    }

    private bool eC = false;
    async public void EffectC()
    {
        //フラグ管理
        if (eC) return;
        eC = true;

        float b;
        Material mat;

        effectC.SetActive(false);
        effectC.SetActive(true);
        mat = effectC.GetComponent<Renderer>().material;
        b = mat.GetFloat("_Saturation");

        for (int i = 7; i > 0; i--)
        {
            //彩度を初期値に向けて変化させていく
            mat.SetFloat("_Saturation", b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            //視覚効果の発生開始を記録
            GazeDataRecorder.writeSaturation(b, b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            await Task.Delay(100);
        }

        await Task.Delay(1200);
        for (int i = 1; i <= 7; i++)
        {
            //彩度を初期値に向けて変化させていく
            mat.SetFloat("_Saturation", b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            GazeDataRecorder.writeSaturation(b, b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            await Task.Delay(150);
        }

        eC = false;

        mat.SetFloat("_Saturation", b);
        effectC.SetActive(false);


        //視覚効果の終了を記録
        GazeDataRecorder.writeSaturation(b, 0);
    }

    private bool eD = false;
    async public void EffectD()
    {

        if (eD) return;
        eD = true;

        float b;
        Material mat;

        effectD.SetActive(false);
        effectD.SetActive(true);
        mat = effectD.GetComponent<Renderer>().material;
        b = mat.GetFloat("_Alpha");

        for (int i = 7; i > 0; i--)
        {
            mat.SetFloat("_Alpha", b / ((((float)i * (float)i) + 9 * (float)i) / 10f));
            //透明度を初期値に向けて上昇させていくスプリクト

            GazeDataRecorder.writeLRdiff(b, b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            //Debug.Log($"{mat[m].GetFloat("_BlurAmount")} で、({(((float)i * (float)i) + (float)i) / 2f})");
            await Task.Delay(100);
        }

        await Task.Delay(900);
        for (int i = 1; i <= 7; i++)
        {
            mat.SetFloat("_Alpha", b / ((((float)i * (float)i) + 9 * (float)i) / 10f));
            //透明度を初期値に向けて上昇させていくスプリクト

            GazeDataRecorder.writeLRdiff(b, b / ((((float)i * (float)i) + 9 * (float)i) / 10f));

            //Debug.Log($"{mat[m].GetFloat("_BlurAmount")} で、({(((float)i * (float)i) + (float)i) / 2f})");
            await Task.Delay(150);
        }

        eD = false;
        mat.SetFloat("_Alpha", b);
        effectD.SetActive(false);

        GazeDataRecorder.writeLRdiff(b, 0);
    }

}
