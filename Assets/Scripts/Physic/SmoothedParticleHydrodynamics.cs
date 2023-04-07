using UnityEngine;

public class SmoothedParticleHydrodynamics
{
    static float waterVolumicMass = 997f;


    [SerializeField] float radius;

    Particle m_baseParticle;
    Particle[] m_neighbourParticles;

    void UpdateParticles()
    {
        m_baseParticle.density = ProcessDensity(m_baseParticle);
        m_baseParticle.pression = ProcessPression(m_baseParticle);

        m_baseParticle.pressionForce = ProcessPressionForce(m_baseParticle);
        m_baseParticle.viscosityForce = ProcessViscosityForce(m_baseParticle);
        
        m_baseParticle.velocity = ProcessVelocity(m_baseParticle);
    }

    // r = distance, h = radius
    // Usefull for density
    float GetWeightPoly6(float dist)
    {
        if (dist > radius)
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
        for (int i = 0; i < m_neighbourParticles.Length; i++)
        {
            sum += GetWeightPoly6((m_neighbourParticles[i].pos - particle.pos).magnitude);
        }

        return particle.mass * sum;
    }

    Vector2 ProcessPression(Particle particle)
    {
        //TODO: Vector2 k = (particle.density - m_pos).normalized;
        Vector2 k = Vector2.one;
        
        //p0 = waterVolumicMass ? maybe 
        return k * (particle.density - waterVolumicMass);
    }

    Vector2 ProcessPressionForce(Particle particle)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < m_neighbourParticles.Length; i++)
        {
            sum += (particle.pression + m_neighbourParticles[i].pression) / (2f * m_neighbourParticles[i].density) *
                   GetDirLaplacien(m_neighbourParticles[i].pos - particle.pos);
        }

        return -particle.mass * sum;
    }

    Vector2 ProcessViscosityForce(Particle particle)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < m_neighbourParticles.Length; i++)
        {
            sum += (m_neighbourParticles[i].velocity - particle.velocity) / m_neighbourParticles[i].density *
                   GetWeightLaplacien((m_neighbourParticles[i].pos - particle.pos).magnitude);
        }

        return particle.viscosityCoef * particle.mass * sum;
    }

    Vector2 ProcessVelocity(Particle particle)
    {
        return particle.velocity +
               (Physics2D.gravity + (particle.pressionForce + particle.viscosityForce) / particle.density) *
               Time.deltaTime;
    }
}