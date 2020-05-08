using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("clicked on: " + gameObject.name);
        //Debug.Log(gameObject.renderer.material.color);
    }



}
