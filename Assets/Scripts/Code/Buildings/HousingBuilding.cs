using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class HousingBuilding : AbstractBuilding
{

    float ProductionCycleProgress;
    int capacity = 10;

    public HousingBuilding(int upkeepCost) : base(upkeepCost)
    {
        // Adding 2 New Workers on building
        _workers = new List<Worker>();
        _workers.Add(new Worker());
        _workers.Add(new Worker());
    }

    public int getNumberOfWorkers()
    {
        return _workers.Count;
    }

    public override void gameTick()
    {
        // Spawn a new Worker after 30 Seconds/ Depneding on happiness 
        // Max Number of workers = 10

        float avgHappines = 0.0f;

        foreach (Worker w in _workers)
        {
            avgHappines += w._happiness;
        }

        avgHappines = avgHappines / _workers.Count;


        ProductionCycleProgress += avgHappines;
        if (ProductionCycleProgress >= (float)30)
        {
            if (_workers.Count < capacity)
            {
                _workers.Add(new Worker());
            }
            ProductionCycleProgress -= 30;
        }
    }

}

