using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class SmoothedParticleHydrodynamics
{
    public static void UpdateParticleDensity(ref Particle baseParticle, Particle[] neighbourParticles, float radius,
        float maxDensity)
    {
        baseParticle.density = ProcessDensity(baseParticle, neighbourParticles, radius, maxDensity);
        baseParticle.pression = ProcessPressure(baseParticle);
    }

    public static void UpdateParticleForces(ref Particle baseParticle, Particle[] neighbourParticles, float radius, float maxVelocity)
    {
        baseParticle.force = ProcessForces(baseParticle, neighbourParticles, radius, maxVelocity);
    }

    public static void UpdateParticleVelocity(ref Particle baseParticle, float detlaTime, float maxVelocity)
    {
        baseParticle.velocity = ProcessVelocity(baseParticle, detlaTime, maxVelocity);
        baseParticle.pos += baseParticle.velocity * detlaTime;
    }

    // r = distance, h = radius
    // Useful for density
    static float DensityKernel(float distanceSquared, float radius)
    {
        if (distanceSquared > radius * radius)
            return 0;

        // Doyub Kim
        float x = 1.0f - distanceSquared / (radius * radius);
        return 315f / (64f * Mathf.PI * (radius * radius * radius)) * x * x * x;
    }

    static Vector2 PressureForceKernel(float distance, float radius, Vector2 directionFromCenter)
    {
        float x = 1.0f - distance / radius;
        return -45.0f / (Mathf.PI * Mathf.Pow(radius, 4)) * x * x * directionFromCenter;
    }

    static float ViscosityForceKernel(float distance, float radius)
    {
        // Btw, it derives 'distance' not 'radius' (h)
        float x = 1.0f - distance / radius;
        return 90f / (Mathf.PI * Mathf.Pow(radius, 5)) * x;
    }

    static float ProcessDensity(Particle baseParticle, Particle[] neighbourParticles, float radius, float maxDensity)
    {
        if (neighbourParticles.Length == 0)
            return -1f;
        
        float sum = 0f;

        for (int i = 0; i < neighbourParticles.Length; i++)
        {
            sum += DensityKernel((baseParticle.pos - neighbourParticles[i].pos).sqrMagnitude, radius);
        }

        return baseParticle.data.mass * Mathf.Clamp(sum, -maxDensity, maxDensity) + 0.000001f; //avoid 0
    }

    static float ProcessPressure(Particle baseParticle)
    {
        return baseParticle.data.gazStiffness * (baseParticle.density - baseParticle.data.baseDensity);
    }

    static Vector2 ProcessForces(Particle baseParticle, Particle[] neighbourParticles, float radius, float maxVelocity)
    {
        Vector2 pressureForce = Vector2.zero;
        Vector2 viscosityForce = Vector2.zero;

        float sqrDensity = baseParticle.density * baseParticle.density;
        float sqrMass = baseParticle.data.mass * baseParticle.data.mass;
        for (int i = 0; i < neighbourParticles.Length; i++)
        {
            float distance = (baseParticle.pos - neighbourParticles[i].pos).magnitude;
            if (distance > 0.0f)
            {
                Vector2 direction = (baseParticle.pos - neighbourParticles[i].pos) / distance;

                pressureForce -= sqrMass *
                                 (baseParticle.pression / sqrDensity + neighbourParticles[i].pression /
                                     (neighbourParticles[i].density * neighbourParticles[i].density)) *
                                 PressureForceKernel(distance, radius, direction);

                viscosityForce += baseParticle.data.viscosityCoef * sqrMass *
                    (neighbourParticles[i].velocity - neighbourParticles[i].velocity) /
                    neighbourParticles[i].density * ViscosityForceKernel(distance, radius);
            }
        }
        
        return Physics2D.gravity * baseParticle.data.mass + Vector2.ClampMagnitude(pressureForce + viscosityForce, maxVelocity);
    }

    static Vector2 ProcessVelocity(Particle baseParticle, float detlaTime, float maxVelocity)
    {
        return baseParticle.velocity + (baseParticle.force / baseParticle.data.mass) * detlaTime;
    }
}