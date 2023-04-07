
using UnityEngine;
using Random = UnityEngine.Random;

public class FluidManager : MonoBehaviour
{
    SmoothedParticleHydrodynamics m_particleGroups;

    [SerializeField, Min(0)]
    internal int m_particlesCounts;

    internal Particle[] m_prevParticle;
    internal Particle[] m_currentParticle;

    [SerializeField]
    internal Vector2 m_spawnPosition;
    
    [SerializeField]
    internal float m_spawnRadius;
    
    void Start()
    {
        GenerateParticles();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < m_particlesCounts; i++)
        {
            SmoothedParticleHydrodynamics.UpdateParticleDensity(ref m_currentParticle[i], m_prevParticle, 3f);
        }
        
        for (int i = 0; i < m_particlesCounts; i++)
        {
            SmoothedParticleHydrodynamics.UpdateParticleForces(ref m_currentParticle[i], m_prevParticle, 3f);
            
            Vector2 prevPos = m_currentParticle[i].pos;
            SmoothedParticleHydrodynamics.UpdateParticleVelocity(ref m_currentParticle[i], Time.fixedDeltaTime);
            CheckCollider(prevPos, ref m_currentParticle[i].pos);
        }
    }

    void GenerateParticles()
    {
        m_prevParticle = new Particle[m_particlesCounts];
        m_currentParticle = new Particle[m_particlesCounts];
        
        for (int i = 0; i < m_particlesCounts; i++)
        {
            Particle newParticle = new Particle();
            newParticle.pos = GetRandomPointInCircleUniform(m_spawnPosition, m_spawnRadius); 
            m_prevParticle[i] = newParticle;
        }
    }
    
    static Vector2 GetRandomPointInCircleUniform(Vector2 center, float radius)
    {
        float t = 2 * Mathf.PI * Random.value;
        float r = Mathf.Sqrt(Random.value * radius);
        float x = r * Mathf.Cos(t);
        float y = r * Mathf.Sin(t);
        return center + new Vector2(x, y);
    }
    
    static void CheckCollider(Vector2 prevPos, ref Vector2 nextPos)
    {
        float magnitude = (nextPos - prevPos).magnitude;
        RaycastHit2D hit = Physics2D.Raycast(prevPos, (nextPos - prevPos)/magnitude, magnitude );
        if (!hit) 
            return;
        
        float dist = (nextPos - hit.point).magnitude;
        nextPos =  hit.point * hit.normal* dist;
    }
}
