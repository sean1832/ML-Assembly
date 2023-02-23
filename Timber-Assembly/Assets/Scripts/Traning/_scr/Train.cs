using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class Train : Agent
{

    //[SerializeField][Range(1,5)] private float wallHeight = 1;
    //[SerializeField][Range(5,20)] private float wallLength = 5;

    // actions:
    // wall height
    // wall length
    // window number choice (1-2)
    // each window position
    //  - up & down
    //  - left & right
    // each window size
    //  - up & down
    //  - left & right
    

    // observe:
    // position of the window
    // scale of window
    // wall height & width

    // number of minimum cut
    // 

    // reward:
    // score

    // continue to manipulate value until ...
    [SerializeField] [Range(0.01f, 1.0f)] private float _windowMoveSpeed = 0.1f;
    [SerializeField] [Range(0.01f, 0.5f)] private float _windowSizeSpeed = 0.1f;

    private float _wallHeight;
    private float _wallWidth;

    private float _windowPosX;
    private float _windowPosY;
    private float _windowSizeX;
    private float _windowSizeY;

    private string _previousDataFromGh = "";
    private string _lastDataFromGh = "";
    private bool _dataFromGhIsChanged = false;




    private GhData _ghData = new GhData();
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnEpisodeBegin()
    {
        // randomize wall height & length
        _wallWidth = Random.Range(3f, 10f);
        _wallHeight = Random.Range(2.1f, 3.5f);

        // randomize window initial scale
        _windowSizeX = Random.Range(0.5f, 1.5f);
        _windowSizeY = Random.Range(0.5f, 1.0f);

        // randomize window initial position
        float tenPercWallWidth = _wallWidth * 0.1f;
        float tenPercWallHeight = _wallHeight * 0.1f;
        float halfWindowSizeX = _windowSizeX * 0.5f;
        float halfWindowSizeY = _windowSizeY * 0.5f;

        _windowPosX = Random.Range(halfWindowSizeX + tenPercWallWidth, _wallWidth - halfWindowSizeX - tenPercWallWidth);
        _windowPosY = Random.Range(halfWindowSizeY + tenPercWallHeight, _wallHeight - halfWindowSizeY - tenPercWallHeight);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // efficiency = 6
        sensor.AddObservation(_ghData.TotalOffcutsAmount);
        sensor.AddObservation(_ghData.TotalSalvageLength);
        sensor.AddObservation(_ghData.MaterialEfficiency);
        sensor.AddObservation(_ghData.OffcutsCount);
        sensor.AddObservation(_ghData.LaborEfficiency);
        sensor.AddObservation(_ghData.ReuseCount);

        // position & scale = 7
        sensor.AddObservation(_ghData.WallScale.H);
        sensor.AddObservation(_ghData.WallScale.W);
        foreach (var winPos in _ghData.WindowPosition)
        {
            sensor.AddObservation(winPos.X);
            sensor.AddObservation(winPos.Y);
            sensor.AddObservation(winPos.Z);
        }

        foreach (var winScale in _ghData.WindowScale)
        {
            sensor.AddObservation(winScale.H);
            sensor.AddObservation(winScale.W);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // window param
        _windowPosX += actions.ContinuousActions[0] * Time.deltaTime * _windowMoveSpeed;
        _windowPosY += actions.ContinuousActions[1] * Time.deltaTime * _windowMoveSpeed;
        _windowSizeX += actions.ContinuousActions[2] * Time.deltaTime * _windowSizeSpeed;
        _windowSizeY += actions.ContinuousActions[3] * Time.deltaTime * _windowSizeSpeed;

        // send to grasshopper
        gameObject.GetComponent<Gh_IO>().msgToGh = $"{_windowPosX},{_windowPosY},{_windowSizeX},{_windowSizeY},{_wallHeight},{_wallWidth}";

        // get from grasshopper
        string dataFromGh = gameObject.GetComponent<Gh_IO>().msgFromGh;

        // check for message changes
        _previousDataFromGh = _lastDataFromGh;
        _lastDataFromGh = dataFromGh;
        if (_lastDataFromGh != _previousDataFromGh)
        {
            _dataFromGhIsChanged = true;
        }

        // if data is changed
        if (!_dataFromGhIsChanged) return;
        _ghData = JsonUtility.FromJson<GhData>(dataFromGh);

        // if any of the window smaller than 200x200, end episode
        if (_ghData.WindowScale.Any(winScale => winScale.H < 0.2f || winScale.W < 0.2f))
        {
            AddReward(-50);
            EndEpisode();
        }

        // if any of the window is touching boundary, end episode
        if (_ghData.IsAtBounds.Any(isAtBound => isAtBound))
        {
            AddReward(-50);
            EndEpisode();
        }

        // otherwise
        SetReward(_ghData.Score);

    }


    
    //void Update()
    //{
    //    gameObject.GetComponent<Gh_IO>().msgToGh = $"{wallHeight},{wallLength}";
    //}
}
