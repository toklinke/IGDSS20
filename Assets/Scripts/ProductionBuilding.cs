using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    #region Resource Generation
    public GameManager.ResourceTypes _inputResource1; //A drop-down in the inspector that declares if this building takes material to generate its output. Optional
    public GameManager.ResourceTypes _inputResource2; //A drop-down in the inspector that declares if this building takes a second material to generate its output. Optional
    public GameManager.ResourceTypes _outputResource; //A drop-down in the inspector that declares the output material

    public float _resourceGenerationInterval = 30; //If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    private float _generationProgress; //The counter that needs to be incrementally increased during a production cycle

    public float _outputCount = 1; //The number of output materials produced in one production cycle

    public bool _hasInputResource1; //Signals that material 1 has been deliverd to this building
    public bool _hasInputResource2; //Signals that material 2 has been deliverd to this building
    public float _progressPercent; //The _generationProgress on a scale from 0 to 100 %. Only for display.
    public float _efficiency; //The calculated efficiency value, based on the _efficiencyScalesWithNeighboringTiles parameter.
    #endregion


    #region Tile Restrictions
    public Tile.TileTypes _efficiencyScalesWithNeighboringTiles; //Choose if this building should scale with the number of surrounding tiles of a specific type
    [Range(0, 6)]
    public int _minimumNeighbors; //The minimum number of surrounding tiles of the specified type required for the building to start working
    [Range(0, 6)]
    public int _maximumNeighbors; //The maximum number of surrounding tiles of the specified type this building can scale with. For example, if the maximum is defined as (4) and there are (3) empty tiles of the specified type, the efficiency will be 75%
    #endregion

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
        HandleResourceGeneration();
    }
    #endregion

    #region Methods    
    void UpdateEfficiency()
    {
        //If the building scales with neighboring tiles
        if (_minimumNeighbors > 0)
        {
            //list all neighboring tiles that have the correct type and no building
            List<Tile> fittingNeighbors = _neighborTiles.FindAll(s => (s._type == _efficiencyScalesWithNeighboringTiles && s._building == null));
            int count = fittingNeighbors.Count;

            float efficiencySteps = 100f / _maximumNeighbors;
            _efficiency = count * efficiencySteps;

            if (_efficiency > 100)
            {
                _efficiency = 100;
            }

            if (count < _minimumNeighbors)
            {
                _efficiency = 0;
            }
        }
        else
        {
            _efficiency = 100;
        }
        float workforceEfficiency = 0;
        if (_availableJobs != 0)
        {
            workforceEfficiency = _occupiedJobs / _availableJobs;
        }

        float happinessEfficiency = 0;
        for (int i = 0; i < _occupiedJobs; i++)
        {
            happinessEfficiency += _workers[i]._happiness;
        }
        happinessEfficiency /= _occupiedJobs * 0.3f;

        //70% surrounding tiles, 30% happiness. Multiplied with available workforce out of capacity
        _efficiency = _efficiency * 0.7f + happinessEfficiency;
        _efficiency *= workforceEfficiency;
        Mathf.Clamp(_efficiency, 0, 100);
    }

    void HandleResourceGeneration()
    {
        //If something is produced here
        if (_outputResource != GameManager.ResourceTypes.None)
        {
            //Collect material 1
            if (!_hasInputResource1)
            {
                if (_inputResource1 != GameManager.ResourceTypes.None && GameManager.Instance.HasResourceInWarehoues(_inputResource1))
                {
                    GameManager.Instance.RemoveResourceFromWarehouse(_inputResource1, 1);
                    _hasInputResource1 = true;
                }
            }
            //Collect material 2
            if (!_hasInputResource2)
            {
                if (_inputResource1 != GameManager.ResourceTypes.None && GameManager.Instance.HasResourceInWarehoues(_inputResource2))
                {
                    GameManager.Instance.RemoveResourceFromWarehouse(_inputResource2, 1);
                    _hasInputResource2 = true;
                }
            }

            //If no material is required for production
            if (_inputResource1 == GameManager.ResourceTypes.None)
            {
                _hasInputResource1 = true;
            }
            if (_inputResource2 == GameManager.ResourceTypes.None)
            {
                _hasInputResource2 = true;
            }

            //Only produce something if all required materials are available
            if (_hasInputResource1 && _hasInputResource2)
            {

                _generationProgress += Time.deltaTime * (_efficiency / 100f);
                _progressPercent = (int)(_generationProgress / _resourceGenerationInterval * 100);

                //If a production cycle is finished
                if (_generationProgress > _resourceGenerationInterval)
                {
                    _generationProgress = 0;
                    GameManager.Instance.AddResourceToWarehouse(_outputResource, _outputCount);
                    _hasInputResource1 = false;
                    _hasInputResource2 = false;
                }
            }
        }
    }
    #endregion
}
