using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundRenderer : MonoBehaviour
{

    [SerializeField] private EdgeCollider2D collider;

    [SerializeField]
    LineRenderer ln;
    void Start()
    {
        if (!collider || !ln)
            return;
        var points = new List<Vector3>();

        ln.positionCount = collider.points.Length;
        for (var index = 0; index < collider.points.Length; index++)
        {
            ln.SetPosition(index, collider.points[index]);
        }
    }

}
