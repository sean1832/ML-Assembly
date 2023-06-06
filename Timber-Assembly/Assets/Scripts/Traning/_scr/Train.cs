using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using Random = UnityEngine.Random;


public class Train : Agent
{
    
    [SerializeField][Range(0.01f, 1.0f)] private float _windowMoveSpeed = 0.1f;
    [SerializeField][Range(0.01f, 0.5f)] private float _windowSizeSpeed = 0.1f;
    [SerializeField] private bool _export = false;

    private int _token;

    private float _previousScore;

    private float _wallHeight;
    private float _wallWidth;

    private float _windowInitPosX;
    private float _windowInitPosY;
    private float _windowInitSizeX;
    private float _windowInitSizeY;

    private float _windowPosX;
    private float _windowPosY;
    private float _windowSizeX;
    private float _windowSizeY;

    private string _previousDataFromGh = "";
    private string _lastDataFromGh = "";
    private bool _dataFromGhIsChanged = false;

    [System.Serializable]
    public class InputData
    {
        public float WindowPosX;
        public float WindowPosY;
        public float WindowSizeX;
        public float WindowSizeY;
        public float WallHeight;
        public float WallWidth;
    }

    [System.Serializable]
    public class GhData
    {
        public float Score;
        public int CutCount;
        public float NewSubjectVolume;
        public float RecycleRate;
        public int WasteRate;
        public float MaterialEfficiency;
        public int LaborEfficiency;
        public int TimeEfficiency;
        public Vector2 WallScale;
        public Vector3[] WindowPos;
        public Vector2[] WindowScale;
        public bool[] IsAtBounds;
        public InputData InputParams;
    }


    private GhData _ghData;

    private void Start()
    {
        // start coroutine to get data from grasshopper
        StartCoroutine(WaitForGrasshopperDataAndUpdate());
    }
    private IEnumerator WaitForGrasshopperDataAndUpdate()
    {
        while (true)
        {
            // get from grasshopper
            string dataFromGh = gameObject.GetComponent<Gh_IO>().msgFromGh;

            // check for message changes
            _previousDataFromGh = _lastDataFromGh;
            _lastDataFromGh = dataFromGh;
            if (_lastDataFromGh != _previousDataFromGh)
            {
                _dataFromGhIsChanged = true;
            }

            if (_dataFromGhIsChanged)
            {
                _ghData = JsonUtility.FromJson<GhData>(dataFromGh);
                Academy.Instance.EnvironmentStep();
                _dataFromGhIsChanged = false;
            }

            yield return null;
        }
    }

    public override void OnEpisodeBegin()
    {
        _token = 5;
        try
        {
            _previousScore = _ghData.Score;
        }
        catch (NullReferenceException e)
        {
            _previousScore = 0;
        }
       

        // randomize wall height & length
        _wallWidth = Random.Range(3f, 10f);
        _wallHeight = Random.Range(2.1f, 3.5f);

        // randomize window initial scale
        _windowInitSizeX = Random.Range(0.5f, 1.5f);
        _windowInitSizeY = Random.Range(0.5f, 1.0f);

        // randomize window initial position
        float tenPercWallWidth = _wallWidth * 0.1f;
        float tenPercWallHeight = _wallHeight * 0.1f;
        float halfWindowSizeX = _windowSizeX * 0.5f;
        float halfWindowSizeY = _windowSizeY * 0.5f;

        _windowInitPosX = Random.Range(halfWindowSizeX + tenPercWallWidth, _wallWidth - halfWindowSizeX - tenPercWallWidth);
        _windowInitPosY = Random.Range(halfWindowSizeY + tenPercWallHeight, _wallHeight - halfWindowSizeY - tenPercWallHeight);

        _windowPosX = _windowInitPosX;
        _windowPosY = _windowInitPosY;
        _windowSizeX = _windowInitSizeX;
        _windowSizeY = _windowInitSizeY;

        gameObject.GetComponent<Gh_IO>().msgToGh = $"{_windowPosX},{_windowPosY},{_windowSizeX},{_windowSizeY},{_wallHeight},{_wallWidth}";
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // only observe until package is received
        if (_dataFromGhIsChanged)
        {
            print("Observation ON");
            // efficiency = 7
            sensor.AddObservation(_ghData.CutCount);
            sensor.AddObservation(_ghData.NewSubjectVolume);
            sensor.AddObservation(_ghData.RecycleRate);
            sensor.AddObservation(_ghData.WasteRate);
            sensor.AddObservation(_ghData.MaterialEfficiency);
            sensor.AddObservation(_ghData.LaborEfficiency);
            sensor.AddObservation(_ghData.TimeEfficiency);

            // position & scale = 7
            sensor.AddObservation(_ghData.WallScale.x);
            sensor.AddObservation(_ghData.WallScale.y);
            foreach (var winPos in _ghData.WindowPos)
            {
                sensor.AddObservation(winPos.x);
                sensor.AddObservation(winPos.y);
                sensor.AddObservation(winPos.z);
            }

            foreach (var winScale in _ghData.WindowScale)
            {
                sensor.AddObservation(winScale.x);
                sensor.AddObservation(winScale.y);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        print($"Step: {StepCount} | Reward: {_ghData.Score}");
        // window param
        _windowPosX += actions.ContinuousActions[0] * Time.deltaTime * _windowMoveSpeed;
        _windowPosY += actions.ContinuousActions[1] * Time.deltaTime * _windowMoveSpeed;
        _windowSizeX += actions.ContinuousActions[2] * Time.deltaTime * _windowSizeSpeed;
        _windowSizeY += actions.ContinuousActions[3] * Time.deltaTime * _windowSizeSpeed;

        // send to grasshopper
        gameObject.GetComponent<Gh_IO>().msgToGh = $"{_windowPosX},{_windowPosY},{_windowSizeX},{_windowSizeY},{_wallHeight},{_wallWidth}";

        float reward = _ghData.Score - _previousScore;

        SetReward(reward);

        // success condition
        if (reward > 3)
        {
            AddReward(10);
            if (_export && _token > 0)
            {
                _token--;
                ExportParam(_ghData.InputParams);
                print($"Exported at {_ghData.Score}");
            }
            EndEpisode();
        }

        // if any of the window smaller than 200x200, end episode
        if (_ghData.WindowScale.Any(winScale => winScale.x < 0.2f || winScale.y < 0.2f))
        {
            AddReward(-5);
            ResetWindow();
        }

        //if any of the window is touching boundary, end episode
        if (_ghData.IsAtBounds.Any(isAtBound => isAtBound))
        {
            AddReward(-5);
            ResetWindow();
        }

        //// success condition
        //if (_ghData.Score >= 3)
        //{
        //    AddReward(+1000);
        //    if (_export)
        //    {
        //        ExportParam(_ghData.InputParams);
        //    }
        //    EndEpisode();
        //}
    }

    // Create a new method for manual stepping
    public void ManualStep()
    {
        // Call RequestDecision for agent's decision-making
        RequestDecision();

        // Call RequestAction for agent's action execution
        RequestAction();
    }

    public override void Heuristic(in ActionBuffers actionOut)
    {
        ActionSegment<float> continuousActions = actionOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void ResetWindow()
    {
        print("window reseted");
        _windowPosX = _windowInitPosX;
        _windowPosY = _windowInitPosY;
        _windowSizeX = _windowInitSizeX;
        _windowSizeY = _windowInitSizeY;
    }

    private void ExportParam(InputData param)
    {
        // export param to file
        string path = "Assets/Results/param.txt";

        // if directory not exist, create directory
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        // create path if not exist
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }

        string paramString = $"{param.WindowPosX},{param.WindowPosY},{param.WindowSizeX},{param.WindowSizeY},{param.WallHeight},{param.WallWidth}";

        // append to file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(paramString);
        writer.Close();

        print("param exported");
    }
}
