using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Physic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FluidManager : MonoBehaviour
{
    
    private int m_currentParticulesCounts = 0;

    [Header("Simulation")] [SerializeField, Min(1)]
    internal int m_particlesCounts;

    [SerializeField, Min(Single.MinValue)] internal float m_groupRadius = 3f;
    [SerializeField] private ParticuleDescriptor m_particuleDescriptor;

    [Header("Stability")] [SerializeField, Min(1)]
    internal int m_subDivision = 4;

    [SerializeField, Min(1)] internal int m_maxCollisionRecursion = 4;
    [SerializeField, Min(0f)] internal float m_collisionEpsilonOffset = 0.01f;
    [SerializeField, Min(Single.MinValue)] internal float m_maxVelocity = 10f;
    [SerializeField, Min(Single.MinValue)] internal float m_maxDensity = 10f;

    [Header("Spawner")] 
    [SerializeField] internal Vector2 m_spawnPosition;
    [SerializeField] internal float m_spawnRadius;

    [Header("Debug")] 
    public float timeScale = 1f;
    internal Particle[] m_prevParticle;
    internal Particle[] m_currentParticle;

    [SerializeField] private ParticuleRenderer m_particuleRenderer = new ();
    
    [SerializeField] private float m_timeMax = 5;

    void Start()
    {
        StartCoroutine(GenerateParticles());
        m_particuleRenderer.setup(m_currentParticulesCounts, m_currentParticle);
        List<Particle>[] neighbours = ProcessNeighbour();

        for (int i = 0; i < m_currentParticulesCounts; i++)
        {
            SmoothedParticleHydrodynamics.UpdateParticleDensity(ref m_prevParticle[i], neighbours[i].ToArray(),

                m_groupRadius,m_maxDensity);
        }
        m_maxVelocity /= m_subDivision;
    }

    void Update()
    {
        Time.timeScale = timeScale;

        for (int step = 0; step < m_subDivision; step++)
        {
            List<Particle>[] neighbours = ProcessNeighbour();

            for (int i = 0; i < m_currentParticulesCounts; i++)
            {
                SmoothedParticleHydrodynamics.UpdateParticleDensity(ref m_currentParticle[i], neighbours[i].ToArray(),
                    m_groupRadius, m_maxDensity);
            }

            for (int i = 0; i < m_currentParticulesCounts; i++)
            {
                SmoothedParticleHydrodynamics.UpdateParticleForces(ref m_currentParticle[i], neighbours[i].ToArray(),
                    m_groupRadius, m_maxVelocity);

                Vector2 prevPos = m_currentParticle[i].pos;
                SmoothedParticleHydrodynamics.UpdateParticleVelocity(ref m_currentParticle[i],
                    Time.deltaTime / m_subDivision,  m_maxVelocity);

                CheckCollider(prevPos, ref m_currentParticle[i].pos, ref m_currentParticle[i].velocity);
            }

            m_prevParticle = m_currentParticle;
        }
        m_particuleRenderer.setup(m_currentParticulesCounts, m_currentParticle);
        m_particuleRenderer.Update(m_currentParticulesCounts);

    }

    List<Particle>[] ProcessNeighbour()
    {
        List<Particle>[] particleNeighbour = new List<Particle>[m_currentParticulesCounts];
        float sqGroupRadius = m_groupRadius * m_groupRadius;

        for (int i = 0; i < m_currentParticulesCounts; i++)
        {
            particleNeighbour[i] = new List<Particle>(m_particlesCounts - 1);
            Vector2 currentParticlePos = m_prevParticle[i].pos;

            for (int j = 0; j < m_currentParticulesCounts; j++)
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

    IEnumerator GenerateParticles()
    {

        m_prevParticle = new Particle[m_particlesCounts];
        m_currentParticle = new Particle[m_particlesCounts];

        float currentTime = 0;
        float currentTimeSpawn = 0;
        bool firstLoop = true;
        while (currentTime < m_timeMax)
        {
            currentTime += Time.deltaTime;
            currentTimeSpawn += Time.deltaTime;

            if (!firstLoop && currentTimeSpawn < m_timeMax / m_particlesCounts)
                continue;

            Particle newParticle = new Particle();
            newParticle.pos =
                GetRandomPointInCircleUniform(m_spawnPosition + transform.position.ToVector2(), m_spawnRadius);
            newParticle.data = m_particuleDescriptor;
            if (float.IsNaN(newParticle.velocity.x) || float.IsNaN(newParticle.velocity.y))
                Debug.Log("ok");
            
            m_prevParticle[m_currentParticulesCounts] = newParticle;
            m_currentParticulesCounts += 1;

            currentTimeSpawn = 0;

            if (firstLoop)
                firstLoop = false;

            if (m_currentParticulesCounts == m_particlesCounts)
                break;

            m_currentParticle = m_prevParticle;

            List<Particle> particleNeighbour = new List<Particle>();
            float sqGroupRadius = m_groupRadius * m_groupRadius;


            for (int j = 0; j < m_currentParticulesCounts - 1; j++)
            {
                Vector2 currentParticlePos = m_prevParticle.Last().pos;

                if ((currentParticlePos - m_prevParticle[j].pos).sqrMagnitude < sqGroupRadius)
                {
                    particleNeighbour.Add(m_prevParticle[j]);
                }
            }

            //Update value particule
            Particle temp = m_prevParticle[m_currentParticulesCounts-1];
            SmoothedParticleHydrodynamics.UpdateParticleDensity(ref temp, particleNeighbour.ToArray(),
                m_groupRadius, m_maxDensity);

            yield return null;
        }
    }
    
    static Vector2 GetRandomPointInCircleUniform(Vector2 center, float radius)
    {
        float t = 2 * Mathf.PI * Random.value;
        float r = Mathf.Sqrt(Random.value) * radius;
        float x = r * Mathf.Cos(t);
        float y = r * Mathf.Sin(t);
        return center + new Vector2(x, y);
    }

    void CheckCollider(Vector2 prevPos, ref Vector2 nextPos, ref Vector2 currentVelocity)
    {
        for (int i = 0; i < m_maxCollisionRecursion; i++)
        {
            float magnitude = (nextPos - prevPos).magnitude;
            RaycastHit2D hit = Physics2D.Raycast(prevPos, (nextPos - prevPos) / magnitude, magnitude);
            if (!hit)
                return;

            float dist = (nextPos - hit.point).magnitude;
            float velocityMagnitude = currentVelocity.magnitude;

            if (Mathf.Approximately(velocityMagnitude, 0))
                return;
            
            Vector2 newDir = Vector2.Reflect(currentVelocity / velocityMagnitude, hit.normal);
            currentVelocity = newDir * currentVelocity.magnitude * hit.collider.sharedMaterial.bounciness;


            prevPos = hit.point + hit.normal * m_collisionEpsilonOffset;
            nextPos = hit.point + newDir * dist;
            if (float.IsNaN(nextPos.x) || float.IsNaN(nextPos.y))
                Debug.Log("ok");
        }
    }
}