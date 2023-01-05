using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSV_IO : MonoBehaviour
{
    public static List<string[]> Read(string path)
    {
        try
        {
            List<string[]> datas = new List<string[]>();

            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var data = line.Split(",");
                datas.Add(data);
            }
            return datas;
        }
        catch(IOException ex)
        {
            Console.WriteLine("Error reading the file: " + ex.Message);
            return null;
        }
    }
}
