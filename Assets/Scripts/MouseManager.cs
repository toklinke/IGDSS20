using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    #region Manager References
    public static MouseManager Instance; //Singleton of this manager. Can be called with static reference MouseManager.Instance
    private GameManager _gameManager; //Reference to GameManager.Instance
    #endregion

    #region Camera
    public Camera _camera; //The camera object
    private Vector3 _lastPanPosition; //The position of the mouse when the dragging begins

    private float PanSpeed = 200f; //Movement speed multiplier of the camera translation
    private float ZoomSpeedMouse = 50f; //Multiplier for zoom factor

    public float[] BoundsX = new float[2]; //Camera bounds on the X axis
    public float[] BoundsZ = new float[2]; //Camera bounds on the Z axis
    public float[] ZoomBounds = new float[] { 10f, 85f }; //Zoom bounds on the Y axis
    #endregion

    #region MonoBehaviour
    //Awake is called when creating this object
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        MouseSelection();
        MousePanning();
    }
    #endregion

    #region Methods
    void MouseSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            int layerMask = LayerMask.GetMask("Tiles");

            if (Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                //Debug.Log("You selected the " + hit.collider.name);
                Tile t = hit.collider.GetComponent<Tile>();
                _gameManager.TileClicked(t._coordinateHeight, t._coordinateWidth);
            }
        }
    }

    void MousePanning()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(1))
        {
            _lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

    void PanCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera
        Vector3 offset = _camera.ScreenToViewportPoint(_lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(-offset.y * PanSpeed, 0, offset.x * PanSpeed);

        // Perform the movement
        _camera.transform.Translate(move, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = _camera.transform.position;
        pos.x = Mathf.Clamp(_camera.transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(_camera.transform.position.z, BoundsZ[0], BoundsZ[1]);
        _camera.transform.position = pos;

        // Cache the position
        _lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed)
    {
        if (offset == 0)
        {
            return;
        }

        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }

    public void InitializeBounds(float Xleft, float Xright, float Ztop, float Zbottom)
    {

        BoundsX = new float[2] { Xleft, Xright };
        BoundsZ = new float[2] { Ztop, Zbottom };

        _camera.transform.position = new Vector3((Xleft + Xright) / 2, _camera.transform.position.y, (Ztop + Zbottom) / 2);
    }
    #endregion
}
