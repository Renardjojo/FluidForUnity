using System;
using UnityEngine;

[Serializable]
public struct ParticuleDescriptor
{
    [Min(Single.MinValue)]
    public float mass; //m
    [Min(Single.MinValue)]
    public float viscosityCoef; //μ
}
public struct Particle
{
    public Vector2 pos;

    public ParticuleDescriptor data;
    // Current state
    public Vector2 velocity; //u
    public float density; //𝜌
    public Vector2 pression; //P 
    public Vector2 pressionForce; //Fp
    public Vector2 viscosityForce; //Fv
}
