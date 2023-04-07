using UnityEngine;

public struct Particle
{
    public Vector2 pos;
    public float mass; //m
    public float viscosityCoef; //μ

    // Current state
    public Vector2 velocity; //u
    public float density; //𝜌
    public float pression; //P 
    public Vector2 pressionForce; //Fp
    public Vector2 viscosityForce; //Fv
}
