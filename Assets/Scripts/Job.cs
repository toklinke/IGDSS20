using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job
{
    public Worker _worker;
    public Building _building;

    public Job(Building building)
    {
        _building = building;
    }

    public void AssignWorker(Worker w)
    {
        _worker = w;
        _building.WorkerAssignedToBuilding(w);
    }

    public void RemoveWorker(Worker w)
    {
        _worker = null;
        _building.WorkerRemovedFromBuilding(w);
    }

    public bool Occupied()
    {
        return _worker != null;
    }
}
