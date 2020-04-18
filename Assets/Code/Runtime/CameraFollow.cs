using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(0, 0.5f)]
    public float smoothingFactor;   // 0 = stick to player, > 0.999 = lag behind player a bit

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Vector3 targetPosition = target.position + offset;
            if (smoothingFactor == 0)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(targetPosition, transform.position, Mathf.Pow(smoothingFactor, Time.deltaTime));
            }
        }
    }
}
