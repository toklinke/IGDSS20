using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public abstract class Building
{
    #region Manager References
    //JobManager _jobManager; //Reference to the JobManager
    #endregion

    //protected int UpkeepCost;
    public int UpkeepCost { get; }


    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    #endregion

    public Building(int upkeepCost)
    {
        UpkeepCost = upkeepCost;
    }

    public abstract void gameTick();


    #region Methods   
    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }
    #endregion
}
