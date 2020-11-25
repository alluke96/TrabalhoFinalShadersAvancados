using UnityEngine;
using System.Collections.Generic;

public class ChunkFactory : MonoBehaviour
{
    // Contém os objetos com o script do meshFactory, porém ainda não cria o terreno.
    private List<GameObject> chunksOut = new List<GameObject>();
    
    // Conforme o terreno for criado, será retirado da lista anterior e colocado nesta.
    private List<GameObject> chunksIn = new List<GameObject>();

    [SerializeField] private int areaChunks; // Se o valor for 4 por exemplo, teremos uma área total 4x4.
    
    public GameObject centro, prefabChunk;

    private void verificarChunks()
    {
        // Enquanto a somatoria da quantidade de itens das duas listas nao for igual a area definida ao quadrado:
        while (chunksOut.Count + chunksIn.Count != areaChunks * areaChunks)
        {
            // Caso seja um valor menor que a area total:
            if (chunksOut.Count + chunksIn.Count < areaChunks * areaChunks)
            {
                // Instancia um novo gameobject na posicao 0,0,0 e adiciona à lista de obj sem terreno gerado.
                var obj = Instantiate(prefabChunk, Vector3.zero, Quaternion.identity);
                obj.transform.name = "Chunk " + chunksOut.Count;
                chunksOut.Add(obj);
            }
            // Caso seja maior:
            else
            {
                // Destroi o obj do indice 0 da lista.
                var obj = chunksOut[0];
                chunksOut.Remove(obj);
                Destroy(obj);
            }
        }
        
        // Inicializar o centro
        // Caso não tenha nenhum valor de centro atribuído:
        if (!centro)
        {
            // Usa o primeiro obj da lista sem terreno gerado.
            var obj = chunksOut[0];
            centro = obj;
            
            MeshFactory meshFactory = obj.GetComponent<MeshFactory>();
            meshFactory.criarTerreno();
            
            // Troca ele de lista.
            chunksOut.Remove(obj);
            chunksIn.Add(obj);
        }
    }

    // Reajusta os valores iniciais de x e z de cada obj (caso necessario)
    public void NovaChunkCentro(GameObject chunkCentro)
    {
        MeshFactory mfCentro = chunkCentro.GetComponent<MeshFactory>();
        MeshFactory mfCentroAnterior = centro.GetComponent<MeshFactory>();

        // Só reajustar o centro caso a distancia em x ou z do chunk atual seja maior que 0 em relacao ao chunk anterior
        // ou se ainda existir algum mesh na lista sem terreno.
        if (mfCentro.inicioEmX - mfCentroAnterior.inicioEmX >= 0 ||
            mfCentro.inicioEmZ - mfCentroAnterior.inicioEmZ >= 0 ||
            chunksOut.Count > 0)
        {
            centro = chunkCentro;

            for (int i = 0; i < chunksIn.Count; i++)
            {
                MeshFactory meshFactory = chunksIn[i].GetComponent<MeshFactory>();

                if (Vector2.Distance(new Vector2(mfCentro.inicioEmX, mfCentro.inicioEmZ), 
                                     new Vector2(meshFactory.inicioEmX, meshFactory.inicioEmZ)) 
                                        >= areaChunks / 2)
                {
                    chunksOut.Add(chunksIn[i]);
                    chunksIn.Remove(chunksIn[i]); 
                    i--; // para não dar index out of range
                }
            }
            
            verificarChunks();

            for (int x = (int) mfCentro.inicioEmX - areaChunks / 2; 
                x < (int) mfCentro.inicioEmX + areaChunks / 2; 
                x++)
            {
                for (int z = (int) mfCentro.inicioEmZ - areaChunks / 2;
                    z < (int) mfCentro.inicioEmZ + areaChunks / 2;
                    z++)
                {
                    var existeChunk = false;

                    for (int i = 0; i < chunksIn.Count; i++)
                    {
                        MeshFactory meshFactory = chunksIn[i].GetComponent<MeshFactory>();
                        if (meshFactory.inicioEmX == x && meshFactory.inicioEmZ == z)
                        {
                            existeChunk = true;
                        }
                    }

                    if (!existeChunk)
                    {
                        var obj = chunksOut[0];
                        MeshFactory meshFactory = obj.GetComponent<MeshFactory>();
                        meshFactory.inicioEmX = x;
                        meshFactory.inicioEmZ = z;
                        meshFactory.criarTerreno();
                            
                        chunksOut.Remove(obj);
                        chunksIn.Add(obj);
                    }
                }
            }
        }
    }

    private void Start()
    {
        verificarChunks();
        NovaChunkCentro(centro);
    }
}
