using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class View : MonoBehaviour
{
    public ClientData data;

    public static Camera deformCam;

    public static float force = 100000f;
    public static float forceOffset = 1f;

    public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    public static View Create(ClientData data)
    {

        // deformCam = givenCamera;
        // ReSharper disable once PossibleNullReferenceException
        Debug.Log("Attempting cam setup");
        deformCam = GameObject.FindWithTag("deform").GetComponent<Camera>();
        var pos = deformCam.ViewportToWorldPoint(new Vector3(Random.value, Random.value, 0));
        Debug.Log("Completed cam setup");

        var xPos = Mathf.Abs(pos.x) * 2f;
        
        // return null;   
        var views = GameObject.Find("Views");
        if (views == null)
        {
            views = new GameObject("Views");
        }

        var mainGameObject = GameObject.Find("Main");
        if (mainGameObject == null)
        {
            Debug.LogError("NO MAIN!!!!");
        }

        var main = mainGameObject.GetComponent<Main>();
        main.connection.SendTo("player_location", data.id, xPos);

        /*
         connection.on("player_location", (sourceId, xPos)=>{
            
         });
         */
        
        var go = new GameObject(((int)xPos).ToString());
        go.transform.parent = views.transform;
        var view = go.AddComponent<View>();
        view.data = data;
        
        
        // var altPos = Camera.main.WorldToViewportPoint(pos);
        view.transform.position = new Vector3(xPos, 0, 0);
        return view;
    }


    // public void Start() {

    //     deformCam = GameObject.FindWithTag("deform").GetComponent<Camera>();

    // }

    private void Update()
    {
        // var x = Random.value;
        var y = data.Input.y;

        // Debug.Log(data.Input.y);

        
        // x = 0.5f;
        // y = -data.Input.y;


        //Ray inputRay = _cam.ScreenPointToRay(new Vector3(100, y * 2, 1));

        // y = Mathf.Abs(y) / 5;

        y = Remap(y, -90, 90, -5000, 5000);

        Ray inputRay = deformCam.ScreenPointToRay(new Vector3(transform.position.x, y, 0));
        // Ray inputRay = deformCam.ScreenPointToRay(new Vector3(3840, y, 0));

        // Debug.Log(   (y * y )/ 5);
        // Debug.Log(y);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;
                //Vector3 xy = new Vector3(x, y, 1f);
                point += hit.normal * forceOffset;
                // xy += hit.normal * forceOffset;
                deformer.AddDeformingForce(point, force);
            }
        }
    }
}
// var x = data.Input.x;
// var pos = Camera.main.ViewportToWorldPoint(new Vector3(Random.value, 0, 0));
//     }
// }