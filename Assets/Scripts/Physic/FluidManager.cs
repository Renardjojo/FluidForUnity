using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class FluidManager : MonoBehaviour
{
    SmoothedParticleHydrodynamics m_particleGroups;

    [SerializeField, Min(0)]
    int m_particlesCounts;

    private Particle[] m_prevParticle;
    private Particle[] m_currentParticle;

    [SerializeField]
    private Vector2 m_spawnPosition;
    
    [SerializeField]
    private float m_spawnRadius;
    
    void Start()
    {
        GenerateParticles();
    }

    void Update()
    {
        
    }

    void GenerateParticles()
    {
        m_prevParticle = new Particle[m_particlesCounts];
        m_currentParticle = new Particle[m_particlesCounts];
        
        for (int i = 0; i < m_particlesCounts; i++)
        {
            Particle newParticle = new Particle();
            newParticle.pos = GetRandomPointInCircleUniform(); 
            m_prevParticle[i] = newParticle;
        }
    }
    
    public Vector2 GetRandomPointInCircleUniform()
    {
        float t = 2 * Mathf.PI * Random.value;
        float r = Mathf.Sqrt(Random.value);
        float x = r * Mathf.Cos(t);
        float y = r * Mathf.Sin(t);
        return new Vector2(x, y);
    }
}
