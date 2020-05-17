using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    //TODO Deal with terrain borders
    //int _boundary = 50;
    //float _edgeMovementSpeed = 10f;
    float _dragSpeed = 100.0f;

    float _scrollUpperLimit = 100;
    float _scrollLowerLimit = 20;

    float _zoomIncrement = 0.6f;

    public float CameraMinX { get; set; }
    public float CameraMaxX { get; set; }
    public float CameraMinZ { get; set; }
    public float CameraMaxZ { get; set; }

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
        float dragSpeed = _dragSpeed * Time.deltaTime;
        Vector3 move = new Vector3(pos.y, 0, pos.x) * dragSpeed;

        float newX = Mathf.Clamp(
            transform.position.x + move.x, CameraMinX, CameraMaxX
        );
        float newZ = Mathf.Clamp(
            transform.position.z - move.z, CameraMinZ, CameraMaxZ
        );

        GetComponent<Transform>().position = new Vector3(newX, transform.position.y, newZ);
    }


    void zoomScroll()
    {
        // scrollAxis > 0: zoom in
        // scrollAxis < 0: zoom out
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        if (scrollAxis == 0.0f)
            return;

        float zoomIncrement = (
            scrollAxis > 0.0f ? -_zoomIncrement : _zoomIncrement
        );
        float newYPos = transform.position.y + zoomIncrement;
        float newYPosClamped = Mathf.Clamp(
            newYPos, _scrollLowerLimit, _scrollUpperLimit
        );
        var newTransformPos = new Vector3(
            transform.position.x, newYPosClamped, transform.position.z
        );
        GetComponent<Transform>().position = newTransformPos;

        float rotateX = (
            scrollAxis > 0.0f ? -0.1f : 0.1f
        );
        transform.Rotate(rotateX, 0, 0);
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
