using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class SmoothedParticleHydrodynamics
{
    static float waterVolumicMass = 997f;

    static void UpdateParticle(Particle baseParticle, Particle[] neighbourParticles, float radius)
    {
        baseParticle.density = ProcessDensity(baseParticle, neighbourParticles, radius);
        baseParticle.pression = ProcessPression(baseParticle, neighbourParticles);

        baseParticle.pressionForce = ProcessPressionForce(baseParticle, neighbourParticles, radius);
        baseParticle.viscosityForce = ProcessViscosityForce(baseParticle, neighbourParticles, radius);
        
        baseParticle.velocity = ProcessVelocity(baseParticle, neighbourParticles);
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
        float sum = 0f;
        for (int i = 0; i < neighbourParticles.Length; i++)
        {
            sum += GetWeightPoly6((neighbourParticles[i].pos - baseParticle.pos).magnitude, radius);
        }

        return baseParticle.mass * sum;
    }

    static Vector2 ProcessPression(Particle baseParticle, Particle[] neighbourParticles)
    {
        //TODO: Vector2 k = (baseParticle.density - m_pos).normalized;
        Vector2 k = Vector2.one;
        
        //p0 = waterVolumicMass ? maybe 
        return k * (baseParticle.density - waterVolumicMass);
    }

    static Vector2 ProcessPressionForce(Particle baseParticle, Particle[] neighbourParticles, float radius)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < neighbourParticles.Length; i++)
        {
            sum += (baseParticle.pression + neighbourParticles[i].pression) / (2f * neighbourParticles[i].density) *
                   GetDirLaplacien(neighbourParticles[i].pos - baseParticle.pos, radius);
        }

        return -baseParticle.mass * sum;
    }

    static Vector2 ProcessViscosityForce(Particle baseParticle, Particle[] neighbourParticles, float radius)
    {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < neighbourParticles.Length; i++)
        {
            sum += (neighbourParticles[i].velocity - baseParticle.velocity) / neighbourParticles[i].density *
                   GetWeightLaplacien((neighbourParticles[i].pos - baseParticle.pos).magnitude, radius);
        }

        return baseParticle.viscosityCoef * baseParticle.mass * sum;
    }

    static Vector2 ProcessVelocity(Particle baseParticle, Particle[] neighbourParticles)
    {
        return baseParticle.velocity +
               (Physics2D.gravity + (baseParticle.pressionForce + baseParticle.viscosityForce) / baseParticle.density) *
               Time.deltaTime;
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
