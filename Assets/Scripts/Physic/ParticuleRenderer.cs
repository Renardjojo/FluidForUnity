using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParticuleRenderer
{
    // Material to use for drawing the meshes.
    public Material material;
  
    private Matrix4x4[] matrices; 
    //private MaterialPropertyBlock block = new ();
    
    
    public Mesh mesh;
    public Vector3 Scale =Vector3.one;
    
    public void setup(int population, Particle[] particles)
    {
        
        matrices = new Matrix4x4[population];
        Vector4[] colors = new Vector4[population];

        //block = new MaterialPropertyBlock();

        for (int i = 0; i < population; i++) {
            // Build matrix.
            Vector3 position = particles[i].pos;
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Scale;

            matrices[i] = Matrix4x4.TRS(position, rotation, scale);

            colors[i] = Color.blue;
        }

        // Custom shader needed to read these!!
        //block.SetVectorArray("_Colors", colors);
    }
    
    public void Update(int population) 
    {
        // Draw a bunch of meshes each frame.
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, population);
    }
}
