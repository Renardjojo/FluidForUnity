using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects, CustomEditor(typeof(FluidManager))]
public class FluidManagerEditor : Editor
{
    private static Vector3 pointSnap = Vector3.one * 0.1f;

    private SerializedObject fmTarget;
    private SerializedProperty pos, radius, particles;

    void OnEnable()
    {
        fmTarget = new SerializedObject(targets);
        pos = fmTarget.FindProperty("m_spawnPosition");
        radius = fmTarget.FindProperty("m_spawnRadius");
        particles = fmTarget.FindProperty("m_particle");
    void OnSceneGUI()
    {


        Transform transform = (target as FluidManager).transform;
        //Handles.CircleHandleCap((Vector3)pos, Quaternion.identity, radius, );
        //Handles.color = Handles.xAxisColor;
        //Handles.CircleHandleCap(
        //    0,
        //    transform.position + new Vector3(3f, 0f, 0f),
        //    transform.rotation * Quaternion.LookRotation(Vector3.right),
        //    radius.floatValue,
        //    EventType.Repaint
        //);
    }

    private void PrintParticle()
    {
        
    }
    
}