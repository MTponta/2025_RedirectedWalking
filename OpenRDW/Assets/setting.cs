using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setting : MonoBehaviour
{
    GazeDataRecorder gdr;
    public enum effectType
    {
        None = 0,
        L_red_ef = 1,
        R_blur = 2,
        L_Saturation = 3,
        LR_diff = 4

    }

    public effectType type;
    public effectType typeB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
