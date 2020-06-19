using UnityEngine;
using UnityEditor;

public class HousingBuilding : Building
{

    float ProductionCycleProgress;
    int capacity = 10;

    public HousingBuilding(int upkeepCost) : base(upkeepCost)
    {

    }


    void DoIt()
    {

    }

    public void gameTick()
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
            if (_workers.Count < 10)
            {
                _workers.Add(new Worker());
            }
            ProductionCycleProgress -= 30;
        }
    }


}

