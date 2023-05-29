using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class SmoothedParticleHydrodynamics
{
    public static void UpdateParticleDensity(ref Particle baseParticle, Particle[] neighbourParticles, float radius, float minDensity, float maxPressure)
    {
        baseParticle.density = ProcessDensity(baseParticle, neighbourParticles, radius);
        baseParticle.pression = ProcessPressure(baseParticle);
        
        // Clamp pressure to stabilize physic
        baseParticle.density = Mathf.Max(baseParticle.density, minDensity);
        baseParticle.pression = Mathf.Clamp(baseParticle.pression, -maxPressure, maxPressure);
    }
    
    public static void UpdateParticleForces(ref Particle baseParticle, Particle[] neighbourParticles, float radius)
    {
        baseParticle.pressionForce = ProcessPressureForce(baseParticle, neighbourParticles, radius);
        baseParticle.viscosityForce = ProcessViscosityForce(baseParticle, neighbourParticles, radius);
    }
    
    public static void UpdateParticleVelocity(ref Particle baseParticle, float detlaTime)
    {
        baseParticle.velocity = ProcessVelocity(baseParticle, detlaTime);
        baseParticle.pos += baseParticle.velocity * detlaTime;
    }

    // r = distance, h = radius
    // Usefull for density
    static float GetWeightPoly6(float dist, float radius)
    {
        if (dist > radius)
            return 0;

        return 315f / (64f * Mathf.PI * Mathf.Pow(radius, 9)) * Mathf.Pow(radius * radius - dist * dist, 3);
    }

    // r = distance, h = radius
    // Usefull for pression
    static float GetWeightSpiky(float dist, float radius)
    {
        if (dist > radius)
            return 0f;

        return 15f * Mathf.Pow(radius - dist, 3) / (Mathf.PI * Mathf.Pow(radius, 6));
    }

    // r = distance, h = radius
    // Usefull for viscosity
    static Vector2 GetDirLaplacien(Vector2 offset, float radius)
    {
        return -45f * offset.normalized * (radius - offset.magnitude) / (Mathf.PI * Mathf.Pow(radius, 6));
    }

    // r = distance, h = radius
    // Usefull for viscosity
    static float GetWeightLaplacien(float dist, float radius)
    {
        return 45f / (Mathf.PI * Mathf.Pow(radius, 6)) * (radius - dist);
    }

    static float ProcessDensity(Particle baseParticle, Particle[] neighbourParticles, float radius)
    {
        if (neighbourParticles.Length == 0)
            return baseParticle.data.mass;
        
        float sum = 0f;
        
        for (int i = 0; i < neighbourParticles.Length; i++)
        {
            sum += GetWeightPoly6((neighbourParticles[i].pos - baseParticle.pos).magnitude, radius);
        }
        
        return baseParticle.data.mass * sum;
    }

    static float ProcessPressure(Particle baseParticle)
    {
        return baseParticle.data.gazStiffness * (baseParticle.density - baseParticle.data.baseDensity);
    }

    static Vector2 ProcessPressureForce(Particle baseParticle, Particle[] neighbourParticles, float radius)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < neighbourParticles.Length; i++)
        {
            sum += (baseParticle.pression + neighbourParticles[i].pression) / (2f * neighbourParticles[i].density) *
                   GetDirLaplacien(neighbourParticles[i].pos - baseParticle.pos, radius);
        }

        return -baseParticle.data.mass * sum;
    }

    static Vector2 ProcessViscosityForce(Particle baseParticle, Particle[] neighbourParticles, float radius)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < neighbourParticles.Length; i++)
        {
            sum += (neighbourParticles[i].velocity - baseParticle.velocity) / neighbourParticles[i].density *
                   GetWeightLaplacien((neighbourParticles[i].pos - baseParticle.pos).magnitude, radius);
        }

        return baseParticle.data.viscosityCoef * baseParticle.data.mass * sum;
    }

    static Vector2 ProcessVelocity(Particle baseParticle, float detlaTime)
    {
        return baseParticle.velocity +
               (Physics2D.gravity + (baseParticle.pressionForce + baseParticle.viscosityForce) / baseParticle.density) * detlaTime;
    }
}
