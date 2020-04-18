/* WorldParent.cs
 * 
 * Scales all of its children vertically such that the children appear to have a consistent height no matter the camera angle
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldParent : MonoBehaviour
{
    private static WorldParent _instance;

    private Camera cam;
    public bool dynamicScalingCorrection = false;
    [Range(0, 89f)]
    public float maxCorrectionAngle = 60f;

    public static WorldParent instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = FindObjectOfType<WorldParent>();
            if (_instance) return _instance;
            throw new System.Exception("Network manager not instanced in scene");
        }
        set
        {
            _instance = value;
        }
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if(dynamicScalingCorrection && cam != null)
        {
            if(Mathf.Abs(cam.transform.eulerAngles.x) < maxCorrectionAngle)
            {
                Vector3 scale = transform.localScale;
                scale.y = 1 / Mathf.Cos(cam.transform.eulerAngles.x * Mathf.Deg2Rad);
                transform.localScale = scale;
            }
        }
    }
}
