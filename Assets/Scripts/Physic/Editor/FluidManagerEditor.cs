using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects, CustomEditor(typeof(FluidManager))]
public class FluidManagerEditor : Editor
{
    private static Vector3 pointSnap = Vector3.one * 0.1f;

    private SerializedObject target;
    private SerializedProperty pos, radius;

    void OnEnable()
    {
        target = new SerializedObject(targets);
        pos = target.FindProperty("m_spawnPosition");
        radius = target.FindProperty("m_spawnRadius");
    }

    void OnSceneGUI()
    {
        //Transform transform = ((FluidManager)target).transform;
        //Handles.color = Handles.xAxisColor;
        //Handles.CircleHandleCap(
        //    0,
        //    transform.position + new Vector3(3f, 0f, 0f),
        //    transform.rotation * Quaternion.LookRotation(Vector3.right),
        //    radius.floatValue,
        //    EventType.Repaint
        //);
    }
}