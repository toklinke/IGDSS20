using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    //TODO Deal with terrain borders
    //int _boundary = 50;
    //float _edgeMovementSpeed = 10f;
    float _dragSpeed = 1.4f;

    float _scrollUpperLimit = 100;
    float _scrollLowerLimit = 20;

    float _zoomIncrement = 0.6f;


    //int _screenHeight;
    //int _screenWidth;


    private Vector3 dragOrigin;

    void Start()
    {
        //_screenHeight = Screen.height;
        //_screenWidth = Screen.width;
    }

    void Update()
    {
        panCamera();
        zoomScroll();

    }

    //TODO deal with map boundaries
    void panCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.y * _dragSpeed, 0, pos.x * _dragSpeed);

        float newX = transform.position.x + move.x;
        float newZ = transform.position.z - move.z;

        // Basic Implementation of hardcoded limits
        // TODO get actual pan limits
        if (newX > 0 && newX < 200 && (newZ > 0 && newZ < 200))
        {
            GetComponent<Transform>().position = new Vector3(newX, transform.position.y, newZ);
        }

    }


    void zoomScroll()
    {

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // Zoom in
        {
            float newYPos = transform.position.y - _zoomIncrement;

            if (newYPos < _scrollLowerLimit)
            {
                return;
            }

            GetComponent<Transform>().position = new Vector3(transform.position.x, newYPos, transform.position.z);
            transform.Rotate(-0.1f, 0, 0);
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0) // Zoom out
        {

            float newYPos = transform.position.y + _zoomIncrement;
            if (newYPos > _scrollUpperLimit)
            {
                return;
            }

            GetComponent<Transform>().position = new Vector3(transform.position.x, newYPos, transform.position.z);
            transform.Rotate(0.1f, 0, 0);

        }

    }

    /*
    void edgeMovement()
    {

        if (Input.mousePosition.x > _screenWidth - _boundary)
        {
            Debug.Log("Hello World reached the boundary right");

            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (_edgeMovementSpeed * Time.deltaTime));

        }

        if (Input.mousePosition.x < 0 + _boundary)
        {
            Debug.Log("Hello World reached the boundary left");
        }



        if (Input.mousePosition.y > _screenHeight - _boundary)
        {
            Debug.Log("Hello World reached the boundary upper");
        }

        if (Input.mousePosition.y < 0 + _boundary)
        {
            Debug.Log("Hello World reached the boundary lower");

        }

    }
    */








}
