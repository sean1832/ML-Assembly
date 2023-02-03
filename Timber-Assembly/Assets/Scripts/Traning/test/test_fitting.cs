using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class test_fitting : MonoBehaviour
{
    private FramingSystem _frame;
    private GameObject _frameParentObj;

    private bool stop = false;

    private List<TimberElement> _timberTargets = new List<TimberElement>();
    private List<TimberElement> _timberSalvage = new List<TimberElement>();
    private List<string[]> _timberSalvageData;

    private GameObject _stud;
    private GameObject _nogging;


    // Start is called before the first frame update
    void Start()
    {
        _frame = gameObject.GetComponent<FramingSystem>();
        Transform framingParent = _frame.framingParent; // access framing system public fields
        _frameParentObj = framingParent.GameObject();
        _timberSalvageData = CSV_IO.Read(@"Assets\Data\data.csv"); // csv: (name, length)

        _stud = _frame.Stud;
        _nogging = _frame.Nogging;
    }

    void Update()
    {
        // single frame test


        if (_frameParentObj.transform.childCount < 4) return; // ignore if frame timber are less than 4
        if (stop) return;

        List<GameObject> timbers = GetChildren(_frameParentObj);

        // assign target timbers
        foreach (var timber in timbers)
        {
            float length = (float)System.Math.Round(Mathf.Max(timber.transform.localScale.x, timber.transform.localScale.y), 2);
            string name = timber.name;

            // write metadata to timber element object
            TimberElement element = new TimberElement
            {
                GameObject = timber,
                Name = name,
                Length = length,
                IsFitted = false,
                TimberType = "target"
            };

            _timberTargets.Add(element);
        }

        // assign salvaged timbers
        foreach (var timberData in _timberSalvageData)
        {
            if (timberData[0] == string.Empty) break;
            string name = timberData[0];
            float length = float.Parse(timberData[1]);

            TimberElement element = new TimberElement()
            {
                Name = name,
                Length = length,
                TimberType = "salvaged"
            };

            _timberSalvage.Add(element);

        }

        List<TimberElement> matchElements = SoloMatch(_timberTargets, _timberSalvage);
        stop = true;
    }

    // solo fitting
    private static List<TimberElement> SoloMatch(List<TimberElement> targets, List<TimberElement> salvagedTimbers)
    {
        List<TimberElement> matchElements = new List<TimberElement>();

        foreach (var timberTarget in targets)
        {
            foreach (var salvaged in salvagedTimbers)
            {
                float margin = 0.05f; // 5%
                float SalvagedLength = salvaged.Length;
                float SalvagedMax = SalvagedLength + SalvagedLength * margin;
                float SalvagedMin = SalvagedLength - SalvagedLength * margin;

                if (timberTarget.Length >= SalvagedMin && timberTarget.Length <= SalvagedMax && !salvaged.IsFitted)
                {
                    timberTarget.IsFitted = true;
                    timberTarget.FittedSalvageNames = new List<string>(){salvaged.Name};

                    salvaged.IsFitted = true;

                    matchElements.Add(timberTarget);

                    print($"match!   [{timberTarget.Name}] with [{salvaged.Name}]");
                    break; // if match found, exit inner loop
                }
            }
        }
        return matchElements;
    }

    private static List<GameObject> GetChildren(GameObject parent, GameObject filter = null, string filterName = null)
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (child == filter) continue;
            else if (child.name == filterName) continue;
            children.Add(child);
        }
        return children;
    }

}
