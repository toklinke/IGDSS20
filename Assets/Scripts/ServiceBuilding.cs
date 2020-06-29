using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceBuilding : Building
{

    public float _efficiency; //The calculated efficiency value.
    //What service does this building provide, and how far does it reach?

    #region MonoBehaviour
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateEfficiency();
    }
    #endregion

    #region Methods    
    void UpdateEfficiency()
    {

        _efficiency = 100;


        float workerEfficiency = _occupiedJobs / _availableJobs;
        _efficiency *= workerEfficiency;
    }
    #endregion
}
