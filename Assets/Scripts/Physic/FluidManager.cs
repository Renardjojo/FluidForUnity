using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// SmoothedParticleHydrodynamics ok mais grosse intérrogation sur p0 
// Faire le fluid manager mais questionnement sur la gestion des SmoothedParticleHydrodynamics.
// Comment les particules se déplace ?
// Toujours dans le même groupe ou peuvent changer de groupe ?

public class FluidManager : MonoBehaviour
{
    SmoothedParticleHydrodynamics[] particleGroups;

    [SerializeField]
    int particlesCounts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
