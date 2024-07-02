using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    [SerializeField] private GameObject defaultCircle;

    // Start is called before the first frame update
    void Start()
    {
        defaultCircle = GameObject.Find("Circle");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            points.Add(Instantiate(defaultCircle));
            points[points.Count - 1].AddComponent<Point>();
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            points[points.Count - 1].GetComponent<Point>().x = mouseWorldPos.x;
            points[points.Count - 1].GetComponent<Point>().y = mouseWorldPos.y;
            Debug.Log("pressed");
        }
    }
}
