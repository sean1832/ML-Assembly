using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class Train : Agent
{
    [SerializeField][Range(1,5)] private float wallHeight = 1;

    [SerializeField][Range(5,20)] private float wallLength = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.GetComponent<Gh_IO>().msgToGh = $"{wallHeight},{wallLength}";
    }
}
