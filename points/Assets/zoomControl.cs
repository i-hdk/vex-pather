using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomControl : MonoBehaviour
{

    public float zoomSize = 15;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(zoomSize>2)
            zoomSize--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if(zoomSize<20)
            zoomSize++;
        }
        GetComponent<Camera>().orthographicSize = zoomSize;
    }
}
