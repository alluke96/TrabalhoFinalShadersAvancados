using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float _velocidade;
    [SerializeField] private ChunkFactory chunkFactory;
    
    private Rigidbody rigidbody;

    private void Start() 
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Manda um raycast pra baixo, caso colida com algo, acessa o script do chunkFactory do objeto selecionado.
    // Passa o objeto alvo do raycast para reajuste do centro.
    private void Update() 
    {
        Move();
        Ray downRay = new Ray(transform.position, -Vector3.up);
        RaycastHit raycastHit;
        
        if (Physics.Raycast(downRay, out raycastHit))
        {
            try
            {
                Debug.DrawRay(transform.position, -Vector3.up, Color.red);
                chunkFactory.NovaChunkCentro(raycastHit.transform.gameObject);
            }
            catch (Exception e)
            {
                Debug.Log("O player está muito alto, usando o centro do GameManager");
            }
        }

        else if (chunkFactory.centro)
        {
            MeshFactory mfCentroAnterior = chunkFactory.centro.GetComponent<MeshFactory>();
            transform.position = mfCentroAnterior.mesh.vertices[0] + Vector3.up * 10;
        }
    }

    private void Move() 
    {
        transform.Rotate(Input.GetAxis("Horizontal") * 90 * Time.deltaTime * Vector3.up);	
        _velocidade = Mathf.Lerp(_velocidade,Input.GetAxis("Vertical") * 10, Time.deltaTime);
        rigidbody.velocity = _velocidade * transform.forward + Vector3.up * rigidbody.velocity.y;      
        
        if(Input.GetKey(KeyCode.Space))
            transform.Translate(0,10 * Time.deltaTime,0);
        else if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
            transform.Translate(0,-10 * Time.deltaTime,0);
    }
}
