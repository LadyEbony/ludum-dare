using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance;
    [Range(0, 0.5f)]
    public float smoothingFactor;   // 0 = stick to player, > 0.999 = lag behind player a bit
    private Vector3 dollyPosition;

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            if (smoothingFactor == 0)
            {
                dollyPosition = target.position;
            }
            else
            {
                dollyPosition = Vector3.Lerp(target.position, dollyPosition, Mathf.Pow(smoothingFactor, Time.deltaTime));
            }

            transform.position = dollyPosition + transform.rotation * Vector3.back * distance;
        }
    }
}
