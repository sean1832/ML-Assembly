using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class test_fitting : MonoBehaviour
{
    private FramingSystem _frame;
    private GameObject _frameParentObj;

    private bool stop = false;

    private List<TimberElement> _timberTargets = new List<TimberElement>();
    private List<TimberElement> _timberSalvage = new List<TimberElement>();

    private GameObject _stud;
    private GameObject _nogging;


    // Start is called before the first frame update
    void Start()
    {
        _frame = gameObject.GetComponent<FramingSystem>();
        Transform framingParent = _frame.framingParent;
        _frameParentObj = framingParent.GameObject();
        
        _stud = _frame.Stud;
        _nogging = _frame.Nogging;
    }

    void Update()
    {
        // single frame test


        if (_frameParentObj.transform.childCount < 4) return; // ignore if frame timber are less than 4
        if (stop) return;

        List<GameObject> timbers = GetChildren(_frameParentObj);

        // assign 
        foreach (var timber in timbers)
        {
            double length = System.Math.Round(Mathf.Max(timber.transform.localScale.x, timber.transform.localScale.y), 2);
            string name = timber.name;

            // write metadata to timber element object
            TimberElement element = new TimberElement
            {
                GameObject = timber,
                Name = name,
                Length = length,
                IsFitted = false
            };

            _timberTargets.Add(element);
        }

        foreach (var timberTarget in _timberTargets)
        {
            
        }
        stop = true;
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
