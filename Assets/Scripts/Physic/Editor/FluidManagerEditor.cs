using Physic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects, CustomEditor(typeof(FluidManager))]
public class FluidManagerEditor : Editor
{
    private static float s_handleSize = 0.2f;
    private static float s_particuleSize = 0.05f;

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
            m_self.transform.position + m_self.m_spawnPosition.ToVector3(),
            m_self.transform.rotation * Quaternion.LookRotation(Vector3.forward),
            m_self.m_spawnRadius,
            EventType.Repaint
        );
        
        Handles.color = Color.green;
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.FreeMoveHandle(m_self.transform.position + m_self.m_spawnPosition.ToVector3(), Quaternion.identity,
            HandleUtility.GetHandleSize(m_self.m_spawnPosition.ToVector3()) * s_handleSize, Vector3.one, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_self, "Change spawn position of fluid manager");
            m_self.m_spawnPosition = newPos.ToVector2();
        }
        
        Handles.color = Color.blue;
        EditorGUI.BeginChangeCheck();
        Vector3 newRadiusPos = Handles.FreeMoveHandle(m_self.transform.position + m_self.m_spawnPosition.ToVector3() + m_self.m_spawnRadius * Vector3.left, Quaternion.identity,
            HandleUtility.GetHandleSize(m_self.m_spawnPosition.ToVector3()) * s_handleSize, Vector3.one, Handles.SphereHandleCap);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_self, "Change spawn radius of fluid manager");
            m_self.m_spawnRadius = (newRadiusPos.ToVector2() - m_self.m_spawnPosition).magnitude;
        }
    }
    
    void OnSceneGUI()
    {
        DrawAndControlSpawnZone();
        PrintParticle();
    }

    private void PrintParticle()
    {
        if (m_self.m_prevParticle == null)
            return;
        
        Particle[] particles = m_self.m_prevParticle;
        foreach (var particle in particles)
        {
            Handles.DrawSolidDisc(particle.pos, Vector3.forward, HandleUtility.GetHandleSize(particle.pos) * s_particuleSize);
        }
    }
    
}