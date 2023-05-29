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
    
    
    private Mesh mesh = new ();

    private Mesh CreateQuad(float width = 1f, float height = 1f) 
    {
        Vector3[] vertices = new Vector3[4]
        {
            new (0, 0, 0),
            new (width, 0, 0),
            new (0, height, 0),
            new (width, height, 0)
        };
        Mesh newMesh =  new Mesh();
        newMesh.vertices = vertices;
        return newMesh;
    }
    
    public void setup(int population, Particle[] particles)
    {
        mesh = CreateQuad();
        
        
        matrices = new Matrix4x4[population];
        Vector4[] colors = new Vector4[population];

        //block = new MaterialPropertyBlock();

        for (int i = 0; i < population; i++) {
            // Build matrix.
            Vector3 position = particles[i].pos;
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;

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
