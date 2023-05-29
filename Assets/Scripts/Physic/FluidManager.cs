using System;
using System.Collections.Generic;
using Physic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FluidManager : MonoBehaviour
{
    [SerializeField, Min(1)] internal int m_particlesCounts;
    
    [Header("Stability")]
    [SerializeField, Min(1)] internal int m_subDivision = 4;
    [SerializeField, Min(Single.MinValue)] internal float m_minDensity = 0.1f;
    [SerializeField, Min(Single.MinValue)] internal float m_maxPressure = 10f;
    [SerializeField, Min(Single.MinValue)] internal float m_maxPressureForce = 10f;

    [SerializeField, Min(Single.MinValue)] internal float m_groupRadius = 3f;

    internal Particle[] m_prevParticle;
    internal Particle[] m_currentParticle;

    [SerializeField] internal Vector2 m_spawnPosition;
    [SerializeField] internal float m_spawnRadius;

    [SerializeField] private ParticuleDescriptor m_particuleDescriptor;

    [SerializeField] 
    private ParticuleDescriptor m_particuleDescriptor;

    [SerializeField] private ParticuleRenderer m_particuleRenderer;
    
    void Start()
    {
        GenerateParticles();
        m_particuleRenderer.setup(m_particlesCounts, m_currentParticle);
    void Start()
    {
        GenerateParticles();

        List<Particle>[] neighbours = ProcessNeighbour();

        for (int i = 0; i < m_particlesCounts; i++)
        {
            SmoothedParticleHydrodynamics.UpdateParticleDensity(ref m_prevParticle[i], neighbours[i].ToArray(),
                m_groupRadius, m_minDensity, m_maxPressure);
        }

        m_maxPressure /= m_subDivision;
        m_maxPressureForce /= m_subDivision;
    }

    void FixedUpdate()
    {
        for (int step = 0; step < m_subDivision; step++)
        {
            SmoothedParticleHydrodynamics.UpdateParticleDensity(ref m_currentParticle[i], neighbours[i].ToArray(), m_groupRadius);
        }
        
        for (int i = 0; i < m_particlesCounts; i++)
        {
            SmoothedParticleHydrodynamics.UpdateParticleForces(ref m_currentParticle[i], neighbours[i].ToArray(), m_groupRadius);
            
            Vector2 prevPos = m_currentParticle[i].pos;
            SmoothedParticleHydrodynamics.UpdateParticleVelocity(ref m_currentParticle[i], Time.fixedDeltaTime);
            CheckCollider(prevPos, ref m_currentParticle[i].pos, ref m_currentParticle[i].velocity);
        }
        m_particuleRenderer.Update(m_particlesCounts);
        m_prevParticle = m_currentParticle;
            List<Particle>[] neighbours = ProcessNeighbour();

            for (int i = 0; i < m_particlesCounts; i++)
            {
                SmoothedParticleHydrodynamics.UpdateParticleDensity(ref m_currentParticle[i], neighbours[i].ToArray(),
                    m_groupRadius, m_minDensity, m_maxPressure);
            }

            for (int i = 0; i < m_particlesCounts; i++)
            {
                SmoothedParticleHydrodynamics.UpdateParticleForces(ref m_currentParticle[i], neighbours[i].ToArray(),
                    m_groupRadius);

                // Clamp pressure force to stabilize physic
                if (m_currentParticle[i].pressionForce.sqrMagnitude > m_maxPressureForce * m_maxPressureForce)
                    m_currentParticle[i].pressionForce = m_currentParticle[i].pressionForce.normalized * m_maxPressureForce;
                
                if (m_currentParticle[i].viscosityForce.sqrMagnitude > m_maxPressureForce * m_maxPressureForce)
                    m_currentParticle[i].viscosityForce = m_currentParticle[i].viscosityForce.normalized * m_maxPressureForce;

                Vector2 prevPos = m_currentParticle[i].pos;
                SmoothedParticleHydrodynamics.UpdateParticleVelocity(ref m_currentParticle[i], Time.fixedDeltaTime / m_subDivision);

                CheckCollider(prevPos, ref m_currentParticle[i].pos, ref m_currentParticle[i].velocity);
            }

            m_prevParticle = m_currentParticle;
        }
    }

    List<Particle>[] ProcessNeighbour()
    {
        List<Particle>[] particleNeighbour = new List<Particle>[m_particlesCounts];
        float sqGroupRadius = m_groupRadius * m_groupRadius;

        for (int i = 0; i < m_particlesCounts; i++)
        {
            particleNeighbour[i] = new List<Particle>();
            Vector2 currentParticlePos = m_prevParticle[i].pos;

            for (int j = 0; j < m_particlesCounts; j++)
            {
                // Don't include self
                if (i == j)
                    continue;

                if ((currentParticlePos - m_prevParticle[j].pos).sqrMagnitude < sqGroupRadius)
                {
                    particleNeighbour[i].Add(m_prevParticle[j]);
                }
            }
        }

        return particleNeighbour;
    }

    void GenerateParticles()
    {
        m_prevParticle = new Particle[m_particlesCounts];
        m_currentParticle = new Particle[m_particlesCounts];

        for (int i = 0; i < m_particlesCounts; i++)
        {
            Particle newParticle = new Particle();
            newParticle.pos =
                GetRandomPointInCircleUniform(m_spawnPosition + transform.position.ToVector2(), m_spawnRadius);
            newParticle.data = m_particuleDescriptor;
            m_prevParticle[i] = newParticle;
        }

        m_currentParticle = m_prevParticle;
    }

    static Vector2 GetRandomPointInCircleUniform(Vector2 center, float radius)
    {
        float t = 2 * Mathf.PI * Random.value;
        float r = Mathf.Sqrt(Random.value) * radius;
        float x = r * Mathf.Cos(t);
        float y = r * Mathf.Sin(t);
        return center + new Vector2(x, y);
    }

    static void CheckCollider(Vector2 prevPos, ref Vector2 nextPos, ref Vector2 currentVelocity)
    {
        const int MAX_ITERATION = 5;
        for (int i = 0; i < MAX_ITERATION; i++)
        {
            float magnitude = (nextPos - prevPos).magnitude;
            RaycastHit2D hit = Physics2D.Raycast(prevPos, (nextPos - prevPos) / magnitude, magnitude);
            if (!hit)
                return;

            float dist = (nextPos - hit.point).magnitude;
            float velocityMagnitude = currentVelocity.magnitude;

            Vector2 newDir = Vector2.Reflect(currentVelocity / velocityMagnitude, hit.normal);
            currentVelocity = currentVelocity.magnitude * newDir * hit.collider.sharedMaterial.bounciness;

            nextPos = hit.point + newDir * dist;
        }
    }
}