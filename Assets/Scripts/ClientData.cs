using UnityEngine;

public class ClientData
{
    public string id;
    public Vector2 Input;
    public View view;

//    private readonly MeshDeformerInput _deform;
     

    public ClientData(string id)
    {
        Input = Vector2.zero;
        view = View.Create(this);
       // Debug.Log("client connected");
//       _deform = MeshDeformerInput.Create(this);

    }

    public void Destroy()
    { 
        Object.Destroy(view.gameObject);
        //  Object.Destroy(_deform.gameObject);
    }
}