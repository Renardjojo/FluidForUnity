using UnityEngine;

public struct Particle
{
    public Vector2 pos;
    public float mass; //m
    public float volume; //V
    public float viscosityCoef; //μ

    //public float VolumicMass => mass / volume; //𝜌

    // Current state
    public Vector2 velocity; //u
    public float density; //𝜌
    public Vector2 pression; //P 
    public Vector2 pressionForce; //Fp
    public Vector2 viscosityForce; //Fv
}
