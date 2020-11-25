using UnityEngine;
using System.Collections.Generic;

public class MeshFactory : MonoBehaviour
{
   public  Mesh mesh;
   private MeshFilter _meshFilter;
   private MeshRenderer _meshRenderer;
   private MeshCollider _meshCollider;
   
   private Vector3[,] vertices;
   private List<Vector3> verticesMesh = new List<Vector3>();
   private List<int> indicesMesh = new List<int>();

   [SerializeField] private int dimensaoX, dimensaoZ;
   [SerializeField] private Material material;

   public float seed, inicioEmX, inicioEmZ, maximoEmY, resolucao;

   private void criarMesh()
   {
      mesh = new Mesh();

      // (x, z+1) 2    (x+1, z+1) 3
      //    º-------------º
      //    | \           |
      //    |    \        |
      //    |       \     |
      //    |          \  |
      //    º-------------º
      // (x, z) 0      (x+1, z) 1

      for (int x = 0; x < dimensaoX - 1; x++)
      {
         for (int z = 0; z < dimensaoZ - 1; z++)
         {
            indicesMesh.Add(verticesMesh.Count);
            indicesMesh.Add(verticesMesh.Count + 2);
            indicesMesh.Add(verticesMesh.Count + 1);

            indicesMesh.Add(verticesMesh.Count + 2);
            indicesMesh.Add(verticesMesh.Count + 3);
            indicesMesh.Add(verticesMesh.Count + 1);
            
            verticesMesh.Add(vertices[x,     z]);
            verticesMesh.Add(vertices[x+1,   z]);
            verticesMesh.Add(vertices[x,   z+1]);
            verticesMesh.Add(vertices[x+1, z+1]);
         }
      }
      
      // Passar os vertices e os triangulos para vetores.
      mesh.vertices = verticesMesh.ToArray();
      mesh.triangles = indicesMesh.ToArray();
            
      mesh.RecalculateNormals(); // Recalcular a direcao dos vertices e das faces (normais).
      mesh.RecalculateBounds();  // Melhorar a aparencia volumétrica do mesh.

      // Caso não possua os componentes, criar eles no objeto e setar as instancias.
      if (!_meshFilter)
         _meshFilter = gameObject.AddComponent<MeshFilter>();
      _meshFilter.mesh = mesh;

      if (!_meshRenderer)
         _meshRenderer = gameObject.AddComponent<MeshRenderer>();
      _meshRenderer.material = material;

      if (!_meshCollider)
         _meshCollider = gameObject.AddComponent<MeshCollider>();
      _meshCollider.sharedMesh = mesh;
            
      // Limpeza
      verticesMesh.Clear();
      indicesMesh.Clear();
   }

   public void criarTerreno()
   {
      vertices = new Vector3[dimensaoX, dimensaoZ];

      for (int x = 0; x < dimensaoX; x++)
      {
         for (int z = 0; z < dimensaoZ; z++)
         {
            var posX = seed + x + inicioEmX * (dimensaoX - 1.0f); // -1.0f para remover espaçamento entre chunks
            var posZ = seed + z + inicioEmZ * (dimensaoZ - 1.0f);
            
            // Valores de 0.0 a 1.0 que vão variar de forma orgânica.
            // Quanto maior o valor da resolução, mais detalhado será o terreno.
            var alturaTerreno = Mathf.PerlinNoise(posX / resolucao, posZ / resolucao);

            // Para deixar o terreno um pouco mais plano
            if (alturaTerreno > 0.3f && alturaTerreno < 0.6f)
               alturaTerreno = 0.4f;

            var posY = alturaTerreno * maximoEmY;
            
            vertices[x,z] = new Vector3(posX, posY, posZ);
         }
      }
      criarMesh();
   }

   private void Start()
   {
      criarTerreno();
   }
}
