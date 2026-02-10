using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effect_pop : MonoBehaviour
{

    public bool _event = false;
    public bool _eventB = false;
    public GameObject player;
    //public setting set;
    public effect_manager manager;

    public setting _setting;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log($"スタート");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {

        Debug.Log($"衝突{col.gameObject}");

        Debug.Log($"衝突{_setting.type}");
        if (_setting.type == setting.effectType.L_red_ef)
        {
            if (col.gameObject == player)
            {

                if (_event == false)
                {
                    _event = true;
                    manager.EffectA();
                }
            }
        }
        else if (_setting.type == setting.effectType.R_blur)
        {
            if (col.gameObject == player)
            {

                if (_event == false)
                {
                    _event = true;
                    manager.EffectB();
                }
            }

        }
        else if (_setting.type == setting.effectType.L_Saturation)
        {
            if (col.gameObject == player)
            {

                if (_event == false)
                {
                    _event = true;
                    manager.EffectC();
                }
            }

        }
        else if (_setting.type == setting.effectType.LR_diff)
        {
            if (col.gameObject == player)
            {
                if (_event == false)
                {
                    _event = true;
                    manager.EffectD();
                }
            }

        }


        if (_setting.typeB == setting.effectType.L_red_ef)
        {
            if (col.gameObject == player)
            {
                if (_eventB == false)
                {
                    _eventB = true;
                    manager.EffectA();
                }
            }
        }
        else if (_setting.typeB == setting.effectType.R_blur)
        {
            if (col.gameObject == player)
            {
                if (_eventB == false)
                {
                    _eventB = true;
                    manager.EffectB();
                }
            }

        }
        else if (_setting.typeB == setting.effectType.L_Saturation)
        {
            if (col.gameObject == player)
            {
                if (_eventB == false)
                {
                    _eventB = true;
                    manager.EffectC();
                }
            }

        }
        else if (_setting.typeB == setting.effectType.LR_diff)
        {
            if (col.gameObject == player)
            {
                if (_eventB == false)
                {
                    _eventB = true;
                    manager.EffectD();
                }
            }
        }

    }
}
