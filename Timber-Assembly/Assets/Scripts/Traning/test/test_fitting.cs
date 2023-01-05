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
        Transform framingParent = _frame.framingParent;
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

        foreach (var timberTarget in _timberTargets)
        {
            foreach (var salvagedTimber in _timberSalvage)
            {

            }
        }
        stop = true;
    }

    private static GameObject CreateObject(string name)
    {
        GameObject obj = new GameObject(name);
        return obj;
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
