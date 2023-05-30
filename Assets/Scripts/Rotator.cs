using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed;
    public float amplitude;
    
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Cos(Time.time * speed) * amplitude);
    }
}
