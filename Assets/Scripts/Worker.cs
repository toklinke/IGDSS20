using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;//Reference to the GameManager
    #endregion

    public float _age; // The age of this worker
    public float _happiness; // The happiness of this worker

    public Job _job; //The job this worker is assigned to
    public HousingBuilding _home; //The house this worker is assigned to

    public float _agingInterval = 15; //The time in seconds it takes for a worker to age by one year
    private float _agingProgress; //The counter that needs to be incrementally increased during a production cycle
    private bool _becameOfAge = false;
    private bool _retired = false;


    private Vector3? _currentGoalPos;
    private Vector3 _currentStartPos;
    private float _currentMoveProgress; // 0.0 (start pos) to 1.0 (goal pos)
    private float _currentMoveSpeed = 2.0f;
    public Vector3? CurrentGoalPos
    {
        get { return _currentGoalPos; }
        set
        {
            _currentGoalPos = value;
            _currentStartPos = this.transform.position;
            _currentMoveProgress = 0.0f;

            if (_currentGoalPos == null) return;

            var pathLength = (_currentGoalPos.Value - _currentStartPos).magnitude;
            _currentMoveSpeed = (1.0f / pathLength) * _movementSpeed;
        }
    }
    private readonly float _movementSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Age();

        UpdateGoal();
        MoveToGoal();
    }

    private void Age()
    {
        _agingProgress += Time.deltaTime;
        if (_agingProgress > _agingInterval)
        {
            _agingProgress = 0;
            _age++;
            ConsumeResourcesAndCalculateHappiness();
            ChanceOfDeath();
        }

        if (_age > 14 && !_becameOfAge)
        {
            BecomeOfAge();
        }

        if (_age > 64 && !_retired)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
        }
    }


    private enum CommuteState { AtHome, ToWork, AtWork, ToHome }
    private CommuteState CurrentCommuteState = CommuteState.AtHome;
    private float CommuteWaitTimeLeft = 0.0f;
    private readonly float WaitTimeAtWork = 5.0f;
    private readonly float WaitTimeAtHome = 3.0f;
    private Tile CommuteNextTile = null;

    private void UpdateGoal()
    {
        switch(CurrentCommuteState)
        {
            case CommuteState.AtHome:
                CommuteWaitTimeLeft -= Time.deltaTime;
                if (CommuteWaitTimeLeft <= 0.0f)
                {
                    if (_job != null)
                    {
                        CurrentCommuteState = CommuteState.ToWork;
                        CommuteNextTile = _job._building._predecessorHashmap[_home._tile];
                        CurrentGoalPos = CommuteNextTile.transform.position;
                    }
                }
            break;
            case CommuteState.ToWork:
                if (_currentMoveProgress >= 1.0f)
                {
                    if (CommuteNextTile == _job._building._tile)
                    {
                        // arrived at work
                        CurrentCommuteState = CommuteState.AtWork;
                        CommuteWaitTimeLeft = WaitTimeAtWork;
                    }
                    else
                    {
                        // next tile on path to work
                        CommuteNextTile = _job._building._predecessorHashmap[CommuteNextTile];
                        CurrentGoalPos = CommuteNextTile.transform.position;
                    }
                }
            break;
            case CommuteState.AtWork:
                CommuteWaitTimeLeft -= Time.deltaTime;
                if (CommuteWaitTimeLeft <= 0.0f)
                {
                    CurrentCommuteState = CommuteState.ToHome;
                    CommuteNextTile = _home._predecessorHashmap[_job._building._tile];
                    CurrentGoalPos = CommuteNextTile.transform.position;
                }
            break;
            case CommuteState.ToHome:
                if (_currentMoveProgress >= 1.0f)
                {
                    if (CommuteNextTile == _home._tile)
                    {
                        // arrived at home
                        CurrentCommuteState = CommuteState.AtHome;
                        CommuteWaitTimeLeft = WaitTimeAtHome;
                    }
                    else
                    {
                        // next tile on path to work
                        CommuteNextTile = _home._predecessorHashmap[CommuteNextTile];
                        CurrentGoalPos = CommuteNextTile.transform.position;
                    }
                }
            break;
        }
    }

    private void MoveToGoal()
    {
        if (CurrentGoalPos == null) return;

        _currentMoveProgress += _currentMoveSpeed * Time.deltaTime;
        if (_currentMoveProgress > 1.0f)
            _currentMoveProgress = 1.0f;

        var direction = CurrentGoalPos.Value - _currentStartPos;
        var newPos = _currentStartPos + direction * _currentMoveProgress;
        // set height to correct tile height
        if (_currentMoveProgress < 0.5)
        {
            newPos.y = _currentStartPos.y;
        }
        else
        {
            newPos.y = CurrentGoalPos.Value.y;
        }
        this.transform.position = newPos;
    }

    private void ConsumeResourcesAndCalculateHappiness()
    {
        bool fish = _gameManager.RemoveResourceFromWarehouse(GameManager.ResourceTypes.Fish, 2);
        bool clothes = _gameManager.RemoveResourceFromWarehouse(GameManager.ResourceTypes.Clothes, 2);
        bool schnapps = _gameManager.RemoveResourceFromWarehouse(GameManager.ResourceTypes.Schnapps, 2);
        bool job = _job != null;

        float happinessTarget = (fish ? 25 : 0) + (clothes ? 25 : 0) + (schnapps ? 25 : 0) + (job ? 25 : 10);
        _happiness = (happinessTarget + _happiness) / 2;
    }

    private void ChanceOfDeath()
    {
        float chanceOfDeath = _age * 0.1f * (100f / _happiness);

        float rng = Random.Range(0f, 100f);

        if (rng < chanceOfDeath)
        {
            Die();
        }
    }

    public void AssignToJob(Job job)
    {
        _job = job;
    }

    public void AssignToHome(HousingBuilding home)
    {
        _home = home;
    }

    public void BeBorn()
    {
        _age = 0;
        gameObject.name = "Child";
    }

    public void BecomeOfAge()
    {
        _becameOfAge = true;

        _jobManager = JobManager.Instance;
        _jobManager.RegisterWorker(this);
        gameObject.name = "Worker";
    }

    private void Retire()
    {
        _retired = true;
        _jobManager.RemoveWorker(this);
        gameObject.name = "Retiree";
    }

    private void Die()
    {
        _jobManager.RemoveWorker(this);
        _home.RemoveWorker(this);
        GameManager.Instance.RemoveWorker(this);
        print("A " + gameObject.name + " has died");

        Destroy(this.gameObject, 1f);
    }
}
