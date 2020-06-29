using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    #endregion

    #region Basic Attributes
    public string _type; //The name of the building
    public float _upkeep; //The money cost per minute
    public float _buildCostMoney; //placement money cost
    public float _buildCostPlanks; //placement planks cost
    public Tile _tile; //Reference to the tile it is built on
    protected List<Tile> _neighborTiles; //List of all neighboring tiles, derived from _tile
    public Dictionary<Tile, Tile> _predecessorHashmap { get; set; }
    public Dictionary<Tile, uint> _travelcostHashmap { get; set; }

    #endregion

    #region Tile Restrictions
    public List<Tile.TileTypes> _canBeBuiltOnTileTypes; //A list that defines all types of tiles it can be placed on. Increase the number in the inspector and then choose from the drop-down menu
    #endregion

    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public int _availableJobs; //The maximum number of workers assigned to a job at this building. Used for efficiency
    public int _occupiedJobs; //The current number of workers assigned to a job at this building. Used for efficiency
    public List<Job> _jobs; // List of all available Jobs
    #endregion

    #region MonoBehaviour
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _neighborTiles = _tile._neighborTiles;
        _workers = new List<Worker>();


        if (_availableJobs > 0)
        {
            GenerateJobs();
            _jobManager = JobManager.Instance;
            _jobManager.RegisterBuilding(this, _jobs);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
    #endregion

    #region Methods
    private void GenerateJobs()
    {
        _jobs = new List<Job>()
            ;
        for (int i = 0; i < _availableJobs; i++)
        {
            _jobs.Add(new Job(this));
        }
    }

    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
        _occupiedJobs++;
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
        _occupiedJobs--;
    }
    #endregion
}
