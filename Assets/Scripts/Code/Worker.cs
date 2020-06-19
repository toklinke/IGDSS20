using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References
    //JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;//Reference to the GameManager
    #endregion
    public float _age; // The age of this worker
    public float _happiness { get; set; } // The happiness of this worker
    bool _hasFood;
    private float _agingTimer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        _agingTimer += Time.deltaTime;
        if(_agingTimer >= 15)
        {
            Age();
            _agingTimer = 0.0f;
            _hasFood = false;
        }
    }

    public void consumeResources()
    {
        _hasFood = true;
    }

    private float calculateHappiness(bool hasJob)
    {
        _happiness = 0;
        if (hasJob)
        {
            _happiness += 0.5f;
        }
        if (_hasFood)
        {
            _happiness += 0.5f;
        }

        return _happiness;
    }


    private void Age()
    {
        //TODO: Implement a life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.

        if (_age > 14)
        {
            BecomeOfAge();
        }

        if (_age > 64)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
        }
    }


    public void BecomeOfAge()
    {
        //_jobManager.RegisterWorker(this);
    }

    private void Retire()
    {
        //_jobManager.RemoveWorker(this);
    }

    private void Die()
    {
        Destroy(this.gameObject, 1f);
    }
}
