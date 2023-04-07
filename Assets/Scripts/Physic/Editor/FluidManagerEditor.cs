using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects, CustomEditor(typeof(FluidManager))]
public class FluidManagerEditor : Editor
{
    private static Vector3 pointSnap = Vector3.one * 0.1f;
    private FluidManager m_self;

    void OnEnable()
    {
        m_self = target as FluidManager;
    }

    void DrawAndControlSpawnZone()
    {
        Handles.color = Handles.xAxisColor;
        Handles.CircleHandleCap(
            0,
            m_self.transform.position + new Vector3(3f, 0f, 0f),
            m_self.transform.rotation * Quaternion.LookRotation(Vector3.right),
            m_self.m_spawnRadius,
            EventType.Repaint
        );
        
        Handles.color = Color.green;
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.FreeMoveHandle( m_self.m_spawnPosition, Quaternion.identity,
            HandleUtility.GetHandleSize(m_self.m_spawnPosition) * m_self.m_spawnRadius, Vector3.one, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_self.transform, "Change Look At Target Position");
            m_self.m_spawnPosition = newPos;
        }
        
        Handles.color = Color.blue;
        EditorGUI.BeginChangeCheck();
        Vector3 newRadiusPos = m_self.m_spawnPosition = Handles.FreeMoveHandle( m_self.m_spawnPosition, Quaternion.identity,
            HandleUtility.GetHandleSize(m_self.m_spawnPosition) * m_self.m_spawnRadius, Vector3.one, Handles.SphereHandleCap);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_self.transform, "Change Look At Target Position");
            //example.targetPosition = newTargetPosition;
        }
    }
    
    void OnSceneGUI()
    {
        DrawAndControlSpawnZone();
    }

    private void PrintParticle()
    {
        
    }
    
}