using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_frameConnection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        var frame = gameObject.GetComponent<FramingSystemOptimised>();
        //frame.NewLayout();


        //for (int i = 0; i < frame.TimberLengths.Count; i++)
        //{
        //    float timberLength = frame.TimberLengths[i];
        //    string timberName = frame.TimberNames[i];
        //    print($"timber name: {timberName}, timber length: {timberLength}");
        //}
        //print("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
