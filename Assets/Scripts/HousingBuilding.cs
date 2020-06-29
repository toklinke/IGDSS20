using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    public float _workerGenerationInterval = 30; //If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    private float _generationProgress; //The counter that needs to be incrementally increased during a production cycle
    public float _progressPercent; //The _generationProgress on a scale from 0 to 100 %. Only for display.
    public float _efficiency; //The calculated efficiency value, based on the _efficiencyScalesWithNeighboringTiles parameter.

    public int _availableHousings; //The maximum number of workers assigned to this building.
    public int _startingPeople; //The number of workers spawned when instantiating this building.
    public int _housedPeople; //The current number of workers assigned to this building.


    #region MonoBehaviour
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        SpawnStartingWorkers();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


        //TODO: requires marketplace as neighbor. 

        if (_housedPeople < _availableHousings)
        {
            if (_housedPeople > 0)
            {

                _efficiency = 0;
                for (int i = 0; i < _housedPeople; i++)
                {
                    _efficiency += _workers[i]._happiness;
                }
                Mathf.Clamp(_efficiency /= _housedPeople, 0, 100);
            }
            else
            {
                _efficiency = 100;
            }

            _generationProgress += Time.deltaTime * (_efficiency / 100f);
            _progressPercent = (int)(_generationProgress / _workerGenerationInterval * 100);

            //If a generation cycle is finished
            if (_generationProgress > _workerGenerationInterval)
            {
                _generationProgress = 0;
                SpawnWorker();
                print("A Child was born");
            }
        }
    }

    private void SpawnStartingWorkers()
    {
        for (int i = 0; i < _startingPeople; i++)
        {
            if (_housedPeople < _availableHousings)
            {
                Worker w = SpawnWorker();
                w._age = 14;
                w.BecomeOfAge();
            }
        }
    }

    private Worker SpawnWorker()
    {
        _housedPeople++;
        Worker w = GameManager.Instance.SpawnWorker(this);
        _workers.Add(w);
        w.BeBorn();

        return w;
    }

    public void RemoveWorker(Worker w)
    {
        _housedPeople--;
        _workers.Remove(w);
    }
    #endregion
}
