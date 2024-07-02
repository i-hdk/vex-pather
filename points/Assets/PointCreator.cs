using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PointCreator : ScriptableObject
{

    [SerializeField] private List<GameObject> points;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            points.Add(new GameObject("circle", typeof(SpriteRenderer)));
            points[points.Count - 1].AddComponent<Point>();
            points[points.Count - 1].GetComponent<Point>().x = Input.mousePosition.x;
            points[points.Count - 1].GetComponent<Point>().y = Input.mousePosition.y;
            Debug.Log("pressed");
        }
    }
}
