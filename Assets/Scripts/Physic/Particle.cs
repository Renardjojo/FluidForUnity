using System;
using UnityEngine;

[Serializable]
public class ParticuleDescriptor
{
    [Min(Single.MinValue)]
    public float mass; //m
    [Min(Single.MinValue)]
    public float viscosityCoef; //μ
    [Min(Single.MinValue)]
    public float gazStiffness; //k
    [Min(Single.MinValue)]
    public float baseDensity; //𝜌0

}
public struct Particle
{
    public Vector2 pos;

    public ParticuleDescriptor data;
    // Current state
    public Vector2 velocity; //u
    public float density; //𝜌
    public float pression; //P 
    public Vector2 pressionForce; //Fp
    public Vector2 viscosityForce; //Fv
    public Vector2 force; //Fv
}
