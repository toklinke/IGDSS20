using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    #region Manager References
    public static JobManager Instance; //Singleton of this manager. Can be called with static reference JobManager.Instance
    GameManager _gameManager; //Reference to GameManager
    #endregion

    private Dictionary<string, List<Job>> _availableJobsByType = new Dictionary<string, List<Job>>(); //Holds a list of all available jobs for each building _type
    private Dictionary<string, List<Job>> _occupiedJobsByType = new Dictionary<string, List<Job>>(); //Holds a list of all occupied jobs for each building _type
    private List<Job> _availableJobs = new List<Job>();

    public List<Worker> _unoccupiedWorkers = new List<Worker>();
    public int _unoccupiedWorkersDisplay;
    public int _unoccupiedJobsDisplay;


    #region MonoBehaviour
    //Awake is called when creating this object
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        HandleUnoccupiedWorkers();
    }
    #endregion

    #region Methods
    public void RegisterBuilding(Building b, List<Job> jobs)
    {
        if (!_availableJobsByType.ContainsKey(b._type))
        {
            _availableJobsByType.Add(b._type, new List<Job>());
            _occupiedJobsByType.Add(b._type, new List<Job>());
        }
        //register into list
        _availableJobsByType[b._type].AddRange(jobs);
        _availableJobs.AddRange(jobs);
        _unoccupiedJobsDisplay += jobs.Count;

        HandleUnoccupiedWorkers();
    }

    private void HandleUnoccupiedWorkers()
    {
        if (_unoccupiedWorkers.Count > 0)
        {
            for (int i = 0; i < _unoccupiedWorkers.Count; i++)
            {
                Worker w = _unoccupiedWorkers[0];

                if (_availableJobs.Count > 0)
                {
                    Job job = FindAnyOpenJob();

                    if (job != null)
                    {
                        AssignWorkerToJob(w, job);
                        _unoccupiedWorkers.RemoveAt(0);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkersDisplay++;
        _unoccupiedWorkers.Add(w);
    }


    private Job FindAnyOpenJob()
    {
        Job job = null;

        if (_availableJobs.Count > 0)
        {
            int rng = Random.Range(0, _availableJobs.Count);

            job = _availableJobs[rng];
        }

        return job;
    }

    private Job FindOpenJob(string jobType)
    {
        Job job = null;

        if (_availableJobsByType[jobType].Count > 0)
        {
            job = _availableJobsByType[jobType][0];
        }

        return job;
    }

    public void RemoveWorker(Worker w)
    {
        if (w._job != null)
        {
            Job job = w._job;
            job.RemoveWorker(w);
            w._job = null;

            _occupiedJobsByType[job._building._type].Remove(job);
            _availableJobsByType[job._building._type].Add(job);
            _availableJobs.Add(job);
            _unoccupiedJobsDisplay++;
        }
        else
        {
            if (_unoccupiedWorkers.Contains(w))
            {
                _unoccupiedWorkers.Remove(w);
            }
        }
    }

    public void AssignWorkerToJob(Worker w, Job job)
    {

        _availableJobsByType[job._building._type].Remove(job);
        _occupiedJobsByType[job._building._type].Add(job);
        _availableJobs.Remove(job);
        _unoccupiedWorkersDisplay--;
        _unoccupiedJobsDisplay--;

        job.AssignWorker(w);
        w.AssignToJob(job);
    }

    #endregion
}
