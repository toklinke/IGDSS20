using UnityEngine;
using UnityEditor;

public class HousingBuilding : Building
{
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
    }

}