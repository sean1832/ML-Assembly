using System.IO;
using UnityEngine;
using System.Text;
using Random = System.Random;
using System;
using UnityEngine.UI;

public class DummyDataGenerator: MonoBehaviour
{
    [SerializeField][Range(1,50)]private int _dataAmount = 10;
    [SerializeField] private string _path = @"Assets\Data\data.csv";
    [SerializeField] private bool _write = false;


    void Update()
    {
        if (!_write) return;
        WriteToCsv();
        _write = false;

    }

    public void WriteToCsv()
    {
        StringBuilder csvBuilder = new StringBuilder();
        Random random = new System.Random();

        for (int i = 0; i < _dataAmount; i++)
        {
            // generate random length between 1 and 6 meter
            float length = (float)random.NextDouble() * 5 + 1;

            // round by 2
            length = (float)System.Math.Round(length, 2);

            // add to line
            csvBuilder.AppendLine($"T{i + 1},{length}");
        }
        string[] lines = csvBuilder.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None); ;

        // write to file
        File.WriteAllLines(_path, lines);
    }
}
