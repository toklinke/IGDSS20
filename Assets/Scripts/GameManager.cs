using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Manager References
    public static GameManager Instance; //Singleton of this manager. Can be called with static reference GameManager.Instance
    private MouseManager _mouseManager; //Reference to MouseManager.Instance
    private NavigationManager _navigationManager;
    #endregion

    #region Map generation
    public Texture2D _heightMap; //Reference to the height map texture file
    public GameObject[] _tilePrefabs; //References to the tile prefabs
    public Transform _tileParentObject; //Reference to the parent object in the hierarchy for all spawned tiles
    private Tile[,] _tileMap; //2D array of all spawned tiles
    private float _heightFactor = 50; //Multiplier for placement of tiles on the Y-axis
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public Transform _buildingParentObject; //Reference to the parent object in the hierarchy for all spawned buildings
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    public List<Building> _buildings; //List of all currently spawned buildings. Used for upkeep in economy ticks
    #endregion

    #region Economy
    private float _economyTickRate = 60; //Every X seconds the economy will tick
    private float _economyTimer; //The current progress within an economy tick cycle
    public float _money = 50000; //The currently available money
    private float _IncomePerPerson = 5; //Each person of the population pays taxes in every economy tick. This amount will be decided by population happiness.
    #endregion

    #region Population
    public int _population; //Number of people available. Currently only one tier of workers
    public GameObject _workerPrefab;
    public Transform _workerParentObject; //Reference to the parent object in the hierarchy for all spawned workers
    //public List<Worker> _workers; // All spawned workers
    #endregion

    #region Resources
    public int _maximumResourceCountInWarehouse = 100; //How much of each resource can be stored in the global warehouse
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType


    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
    #endregion

    #region Enumerations
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
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
        GenerateMap();
        _mouseManager = MouseManager.Instance;
        _mouseManager.InitializeBounds(0, _heightMap.width * 10, 0, _heightMap.height * 8.66f);
        _navigationManager = new NavigationManager();

        _buildings = new List<Building>();
        PopulateResourceDictionary();

        AddResourceToWarehouse(ResourceTypes.Fish, 20);
        AddResourceToWarehouse(ResourceTypes.Planks, 20);
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateEconomyTimer();
        UpdateInspectorNumbersForResources();
    }
    #endregion

    #region Methods
    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        foreach (ResourceTypes type in ResourceTypes.GetValues(typeof(ResourceTypes)))
        {
            _resourcesInWarehouse.Add(type, 0);
        }
    }

    //Handles the progression within an economy cycle
    void UpdateEconomyTimer()
    {
        _economyTimer += Time.deltaTime;

        if (_economyTimer > _economyTickRate)
        {
            _economyTimer = 0;
            TickEconomy();
        }
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
        }
    }

    // select building prefab with button click
    public void UiBuildingButtonClicked(int buildingPrefabIndex)
    {
        _selectedBuildingPrefabIndex = buildingPrefabIndex;
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    //Instantiates individual hexagonal tile prefabs
    void GenerateMap()
    {
        _tileMap = new Tile[_heightMap.height, _heightMap.width];

        //Spawn tiles on grid
        for (int h = 0; h < _heightMap.height; h++)
        {
            for (int w = 0; w < _heightMap.width; w++)
            {

                Color c = _heightMap.GetPixel(w, h);
                float max = c.maxColorComponent;
                float tileHeight = _heightFactor * max;

                //Determine tile type
                GameObject selectedPrefab;

                if (max == 0.0f)
                {
                    selectedPrefab = _tilePrefabs[0];
                }
                else if (max < 0.2f)
                {
                    selectedPrefab = _tilePrefabs[1];
                }
                else if (max < 0.4f)
                {
                    selectedPrefab = _tilePrefabs[2];
                }
                else if (max < 0.6f)
                {
                    selectedPrefab = _tilePrefabs[3];
                }
                else if (max < 0.8f)
                {
                    selectedPrefab = _tilePrefabs[4];
                }
                else
                {
                    selectedPrefab = _tilePrefabs[5];
                }

                GameObject go = Instantiate(selectedPrefab, _tileParentObject);
                go.transform.position = new Vector3(h * 8.66f, tileHeight, w * 10f + (h % 2 == 0 ? 0f : 5f));
                Tile t = go.GetComponent<Tile>();
                t._coordinateHeight = h;
                t._coordinateWidth = w;
                _tileMap[h, w] = t;

            }
        }

        //Populate list of neighbors
        for (int h = 0; h < _tileMap.GetLength(0); h++)
        {
            for (int w = 0; w < _tileMap.GetLength(1); w++)
            {
                Tile t = _tileMap[h, w];
                Tuple<List<Tile>, bool[]> data = FindNeighborsOfTile(t);
                t._neighborTiles = data.Item1;

                t.setEdges(data.Item2);
            }
        }
    }

    //Returns a list of all neighbors of a given tile
    private Tuple<List<Tile>, bool[]> FindNeighborsOfTile(Tile t)
    {
        // Assumes the bool default value is false.
        bool[] edgeArr = new bool[6];

        List<Tile> neighborList = new List<Tile>();

        int height = t._coordinateHeight;
        int width = t._coordinateWidth;

        bool simpleTopValue = false;
        bool simpleBottomValue = false;

        //top, top-left, left, right, bottom, bottom-left
        //Check edge cases
        //top
        if (height > 0)
        {
            neighborList.Add(_tileMap[height - 1, width]);
            simpleTopValue = checkForEqualTileType(_tileMap[height, width], _tileMap[height - 1, width]);
        }
        //bottom
        if (height < _heightMap.height - 1)
        {
            neighborList.Add(_tileMap[height + 1, width]);
            simpleBottomValue = checkForEqualTileType(_tileMap[height, width], _tileMap[height + 1, width]);
        }
        //left
        if (width > 0)
        {
            neighborList.Add(_tileMap[height, width - 1]);
            edgeArr[5] = checkForEqualTileType(_tileMap[height, width], _tileMap[height, width - 1]);
        }
        //right
        if (width < _heightMap.width - 1)
        {
            neighborList.Add(_tileMap[height, width + 1]);
            edgeArr[2] = checkForEqualTileType(_tileMap[height, width], _tileMap[height, width + 1]);
        }

        //if the column is even
        //top-left + bottom-left
        if (height % 2 == 0)
        {
            if (height > 0 && width > 0)
            {
                neighborList.Add(_tileMap[height - 1, width - 1]);

                edgeArr[0] = checkForEqualTileType(_tileMap[height, width], _tileMap[height - 1, width - 1]);

                //Index 0
            }
            if (height < _heightMap.height - 1 && width > 0)
            {
                neighborList.Add(_tileMap[height + 1, width - 1]);

                edgeArr[4] = checkForEqualTileType(_tileMap[height, width], _tileMap[height + 1, width - 1]);
                //Index 4
            }

            // Missing Indices -> 1,3
            edgeArr[1] = simpleTopValue;
            edgeArr[3] = simpleBottomValue;

            
        }
        //if the column is uneven
        //top-right + bottom-right
        else
        {
            if (height > 0 && width < _heightMap.width - 1)
            {
                neighborList.Add(_tileMap[height - 1, width + 1]);

                edgeArr[1] = checkForEqualTileType(_tileMap[height, width], _tileMap[height - 1, width + 1]);
                // Index 1
            }
            if (height < _heightMap.height - 1 && width < _heightMap.width - 1)
            {
                neighborList.Add(_tileMap[height + 1, width + 1]);

                edgeArr[3] = checkForEqualTileType(_tileMap[height, width], _tileMap[height + 1, width + 1]);
                // Index 3
            }

            // Missing Indices -> 0,4
            edgeArr[0] = simpleTopValue;
            edgeArr[4] = simpleBottomValue;

            
        }
        return new Tuple<List<Tile>, bool[]>(neighborList, edgeArr);
    }

    // Checks whether two tiles are of equal type
    private bool checkForEqualTileType(Tile t1, Tile t2)
    {
        return t1._type.Equals(t2._type);

    }


    //Calculates money income and upkeep when an economy cycle is completed
    private void TickEconomy()
    {
        //income
        float income = 0;
        income = _population * _IncomePerPerson;

        _money += income;

        //upkeep
        float upkeep = 0;
        for (int i = 0; i < _buildings.Count; i++)
        {
            upkeep += _buildings[i]._upkeep;
        }

        _money -= upkeep;
    }

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(int height, int width)
    {
        Tile t = _tileMap[height, width];
        //print(t._type);

        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is a selected building prefab
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            //check if building can be placed
            Building buildingType = _buildingPrefabs[_selectedBuildingPrefabIndex].GetComponent<Building>();

            if (t._building == null && _money >= buildingType._buildCostMoney && _resourcesInWarehouse[ResourceTypes.Planks] >= buildingType._buildCostPlanks && buildingType._canBeBuiltOnTileTypes.Contains(t._type))
            {
                GameObject go = Instantiate(_buildingPrefabs[_selectedBuildingPrefabIndex], _buildingParentObject);
                go.transform.position = t.transform.position;
                Building b = go.GetComponent<Building>();
                b._tile = t;
                t._building = b;
                _buildings.Add(b);
                _money -= b._buildCostMoney;
                _resourcesInWarehouse[ResourceTypes.Planks] -= b._buildCostPlanks;

                System.Tuple<Dictionary<Tile, Tile>, Dictionary<Tile, uint>> pathfindingData = _navigationManager.generateTravelMapForBuilding(t);

                b._predecessorHashmap = pathfindingData.Item1;
                b._travelcostHashmap = pathfindingData.Item2;

            }



        }
    }

    //Adds the amount of the specified resource to the dictionary
    public void AddResourceToWarehouse(ResourceTypes resource, float amount)
    {
        if (_resourcesInWarehouse[resource] + amount > _maximumResourceCountInWarehouse)
        {
            _resourcesInWarehouse[resource] = _maximumResourceCountInWarehouse;
        }
        else
        {
            _resourcesInWarehouse[resource] += amount;
        }
    }

    //Subtracts the amount of the specified resource to the dictionary
    public bool RemoveResourceFromWarehouse(ResourceTypes resource, float amount)
    {
        if (_resourcesInWarehouse[resource] - amount >= 0)
        {
            _resourcesInWarehouse[resource] -= amount;
            return true;
        }
        else
        {
            _resourcesInWarehouse[resource] = 0;
            return false;
        }
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }

    public Worker SpawnWorker(HousingBuilding home)
    {
        Worker w = Instantiate(_workerPrefab, home._tile.transform.position, Quaternion.identity).GetComponent<Worker>();
        w.transform.SetParent(_workerParentObject);
        w.AssignToHome(home);
        _population++;
        return w;
    }

    public void RemoveWorker(Worker w)
    {
        _population--;
    }
    #endregion
}
