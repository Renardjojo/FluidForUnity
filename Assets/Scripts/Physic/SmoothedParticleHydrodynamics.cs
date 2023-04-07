using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class SmoothedParticleHydrodynamics
{
    static float waterVolumicMass = 997f;

    [SerializeField]
    Vector2 m_pos;

    [SerializeField]
    float radius;

    Particle[] m_particles;

    void UpdateParticles()
    {
        for (int i = 0; i < m_particles.Length; i++)
        {
            m_particles[i].density = ProcessDensity(m_particles[i]);
            m_particles[i].pression = ProcessPression(m_particles[i]);
        }

        for (int i = 0; i < m_particles.Length; i++)
        {
            m_particles[i].pressionForce = ProcessPressionForce(m_particles[i]);
            m_particles[i].viscosityForce = ProcessViscosityForce(m_particles[i]);
        }

        for (int i = 0; i < m_particles.Length; i++)
            m_particles[i].velocity = ProcessVelocity(m_particles[i]);
    }

    // r = distance, h = radius
    // Usefull for density
    float GetWeightPoly6(float dist)
    {
         if(dist > radius)
            return 0;

        return 315f / (64f * Mathf.PI * Mathf.Pow(radius, 9)) * Mathf.Pow(radius * radius - dist * dist, 3);
    }

    // r = distance, h = radius
    // Usefull for pression
    float GetWeightSpiky(float dist)
    {
        if (dist > radius)
            return 0f;

        return 15f * Mathf.Pow(radius - dist, 3) / (Mathf.PI * Mathf.Pow(radius, 6));
    }

    // r = distance, h = radius
    // Usefull for viscosity
    Vector2 GetDirLaplacien(Vector2 offset)
    {
        return -45f * offset.normalized * (radius - offset.magnitude) / (Mathf.PI * Mathf.Pow(radius, 6));
    }

    // r = distance, h = radius
    // Usefull for viscosity
    float GetWeightLaplacien(float dist)
    {
        return 45f / (Mathf.PI * Mathf.Pow(radius, 6)) * (radius - dist);
    }
    
    float ProcessDensity(Particle particle)
    {
        float sum = 0f;
        for (int i = 0; i < m_particles.Length; i++)
        {
            sum += GetWeightPoly6((m_particles[i].pos - particle.pos).magnitude);
        }
        return particle.mass * sum;
    }

    Vector2 ProcessPression(Particle particle)
    {
        Vector2 k = (particle.pos - m_pos).normalized;

        //p0 = waterVolumicMass ? maybe 
        return k * (particle.density - waterVolumicMass);
    }

    Vector2 ProcessPressionForce(Particle particle)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < m_particles.Length; i++)
        {
            sum += (particle.pression + m_particles[i].pression) / (2f * m_particles[i].density) * GetDirLaplacien(m_particles[i].pos - particle.pos);
        }
        return -particle.mass * sum;
    }

    Vector2 ProcessViscosityForce(Particle particle)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < m_particles.Length; i++)
        {
            sum += (m_particles[i].velocity - particle.velocity) / m_particles[i].density * GetWeightLaplacien((m_particles[i].pos - particle.pos).magnitude);
        }
        return particle.viscosityCoef * particle.mass * sum;
    }

    Vector2 ProcessVelocity(Particle particle)
    {
       return particle.velocity + (Physics2D.gravity + (particle.pressionForce + particle.viscosityForce) / particle.density) * Time.deltaTime;
    }

    static void  CheckCollider(Vector2 prevPos, ref Vector2 nextPos)
    {
        float magnitude = (nextPos - prevPos).magnitude;
        RaycastHit2D hit = Physics2D.Raycast(prevPos, (nextPos - prevPos)/magnitude, magnitude );
        if (!hit) 
            return;
        
        float dist = (nextPos - hit.point).magnitude;
        nextPos =  hit.point * hit.normal* dist;
    }
}
