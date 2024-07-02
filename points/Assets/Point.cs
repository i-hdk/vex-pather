using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] public float x=0, y=0;
    public Point(float xpos, float ypos)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        name = "copy";
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        //spriteRenderer.sprite = Resources.LoadAll<Sprite>("circle")[0];
        GetComponent<Transform>().position = new Vector2(x, y);
    }
}
