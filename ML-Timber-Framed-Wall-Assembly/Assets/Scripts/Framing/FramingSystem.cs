using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FramingSystem : MonoBehaviour
{
    public Camera mainCam;
    [Range(1.0f, 10.0f)]
    public float wallXScale;
    [Range(1.0f, 5.0f)]
    public float wallYScale;
    public float maxNogging;
    public float timberWidth = 0.045f;
    public float timberDepth = 0.09f;
    public float lintelHeight = 0.18f;
    public float minDoorHeight = 1.80f;
    public Transform wallPivot;
    public TextMeshProUGUI timberLengths;
    public List<Opening> Openings;
    public List<Gap> Gaps;
    public TMP_Dropdown OpeningsDropdown;
    public int selectedOpening;
    public TextMeshProUGUI wallWidthText;
    public GameObject wallXSlider;
    public TextMeshProUGUI wallHeightText;
    public GameObject wallYSlider;
    public TextMeshProUGUI openingXPosText;
    public GameObject OpeningXPosSlider;
    public TextMeshProUGUI openingYPosText;
    public GameObject OpeningYPosSlider;
    public TextMeshProUGUI openingWidthText;
    public GameObject OpeningWidthSlider;
    public TextMeshProUGUI openingHeightText;
    public GameObject OpeningHeightSlider;
    public TMP_InputField timberCollection;
    public Transform framingParent;
    public Transform topPlate;
    public Transform bottomPlate;
    public Transform LStud;
    public Transform RStud;
    public GameObject Stud;
    public GameObject Nogging;
    public GameObject boundingBox;
    public bool adjustSliderValue;
    public float highestOpeningPoint;

    void Start()
    {
        Gaps.Add(new(timberWidth * 0.5f, 1f));
        Openings.Add(new("window", 1f, 1f, 1.5f, 1f, 2));
        OpeningsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "Window " + Openings.Count });
        Gaps.Add(new(Openings[0].XPos + Openings[0].Width, 1f));
        Openings.Add(new("door", 3.2f, 0f, 0.9f, 2.3f, 1));
        OpeningsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "Door " + Openings.Count });
        Gaps.Add(new(Openings[1].XPos + Openings[1].Width, 1f));
        UpdateOpenings("start");
        UpdateGaps("start");
        adjustSliderValue = false;
        wallXSlider.GetComponent<Slider>().minValue = Openings[^1].XPos + Openings[^1].Width + Openings[^1].studNum * timberWidth + timberWidth;
        highestOpeningPoint = Openings[^1].YPos + Openings[^1].Height + timberWidth * 2;
        wallYSlider.GetComponent<Slider>().minValue = highestOpeningPoint;
        wallYSlider.GetComponent<Slider>().maxValue = highestOpeningPoint + 3f;
        SelectedOpening(0);
    }

    void Update()
    {

        int childCount = framingParent.childCount;
        timberLengths.text = "";
        for (int i =0; i < childCount; i++)
        {
            Transform Child = framingParent.GetChild(i);
            timberLengths.text += "\n " + Child.name + ": " + System.Math.Round(Mathf.Max(Child.localScale.x, Child.localScale.y),2) + "m";
        }
    }
    public void NewDoor()
    {
        if (Gaps[^1].Width > 1.5f)
        {
            Openings.Add(new("door", Gaps[^1].XPos + Gaps[^1].Width * 0.5f - 0.45f, 0f, 0.9f, 2.3f, 1));
            OpeningsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "Door " + Openings.Count });
            Gaps.Add(new(Gaps[^1].XPos + Gaps[^1].Width * 0.5f + 0.45f, wallXScale - (Gaps[^1].XPos + Gaps[^1].Width * 0.5f + 0.45f)));
            UpdateOpening(Openings.Count - 1, "new");
            UpdateGap(Openings.Count - 1, "");
            UpdateGap(Openings.Count, "new");
            OpeningsDropdown.value = Openings.Count - 1;
            SelectedOpening(Openings.Count - 1);
            if (Openings.Count == 1)
            {
                OpeningXPosSlider.GetComponent<Slider>().enabled = true;
                OpeningYPosSlider.GetComponent<Slider>().enabled = false;
                OpeningWidthSlider.GetComponent<Slider>().enabled = true;
                OpeningHeightSlider.GetComponent<Slider>().enabled = true;
            }
            wallXSlider.GetComponent<Slider>().minValue = Openings[^1].XPos + Openings[^1].Width + Openings[^1].studNum * timberWidth + timberWidth;
            UpdateWallYSlider();
        }
        else
        {
            print("Not enough room");
        }
    }
    public void NewWindow()
    {
        if (Gaps[^1].Width > 2f)
        {
            Openings.Add(new("window", Gaps[^1].XPos + Gaps[^1].Width * 0.5f - 0.75f, 1f, 1.5f, 1f, 2));
            OpeningsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "Window " + Openings.Count });
            Gaps.Add(new(Gaps[^1].XPos + Gaps[^1].Width * 0.5f + 0.75f, wallXScale - (Gaps[^1].XPos + Gaps[^1].Width * 0.5f + 0.75f)));
            UpdateOpening(Openings.Count - 1, "new");
            UpdateGap(Openings.Count - 1, "");
            UpdateGap(Openings.Count, "new");
            OpeningsDropdown.value = Openings.Count - 1;
            SelectedOpening(Openings.Count - 1);
            if (Openings.Count == 1)
            {
                OpeningXPosSlider.GetComponent<Slider>().enabled = true;
                OpeningYPosSlider.GetComponent<Slider>().enabled = true;
                OpeningWidthSlider.GetComponent<Slider>().enabled = true;
                OpeningHeightSlider.GetComponent<Slider>().enabled = true;
            }
            wallXSlider.GetComponent<Slider>().minValue = Openings[^1].XPos + Openings[^1].Width + Openings[^1].studNum * timberWidth + timberWidth;
            UpdateWallYSlider();
        }
        else
        {
            print("Not enough room");
        }
    }
    public void RemoveOpening()
    {
        foreach (GameObject vert in Openings[selectedOpening].Vertical)
        {
            Destroy(vert);
        }
        foreach (GameObject hori in Openings[selectedOpening].Horizontal)
        {
            Destroy(hori);
        }
        foreach (GameObject stud in Gaps[selectedOpening + 1].Studs)
        {
            Destroy(stud);
        }
        foreach (GameObject nogging in Gaps[selectedOpening + 1].Noggings)
        {
            Destroy(nogging);
        }
        Openings[selectedOpening].Vertical.Clear();
        Openings[selectedOpening].Horizontal.Clear();
        Openings.RemoveAt(selectedOpening);
        OpeningsDropdown.options.RemoveAt(selectedOpening);
        Gaps[selectedOpening + 1].Studs.Clear();
        Gaps[selectedOpening + 1].Noggings.Clear();
        Gaps.RemoveAt(selectedOpening + 1);
        UpdateGap(selectedOpening, "");
        UpdateDropdown();

        if (Openings.Count != 0)
        {
            if (selectedOpening == Openings.Count)
            {
                OpeningsDropdown.RefreshShownValue();
                SelectedOpening(selectedOpening - 1);
            }
            else
            {
                OpeningsDropdown.RefreshShownValue();
                SelectedOpening(selectedOpening);
            }
        }
        else
        {
            OpeningXPosSlider.GetComponent<Slider>().enabled = false;
            OpeningYPosSlider.GetComponent<Slider>().enabled = false;
            OpeningWidthSlider.GetComponent<Slider>().enabled = false;
            OpeningHeightSlider.GetComponent<Slider>().enabled = false;
        }
        wallXSlider.GetComponent<Slider>().minValue = Openings[^1].XPos + Openings[^1].Width + Openings[^1].studNum * timberWidth + timberWidth;
        UpdateWallYSlider();
    }
    public void UpdateOpening(int i, string type)
    {
        if (Openings[i].Type == "door")
        {
            if (type == "XPos")
            {
                //move everything on the x by (current + (newXPos - previousXPos))
                Openings[i].Vertical[0].transform.localPosition = new Vector3(Openings[i].XPos - timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[0].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + timberWidth * 0.5f, timberDepth * 0.5f);
                for (int a = 2; a < Openings[i].numStuds + 2; a++)
                {
                    Openings[i].Vertical[a].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 1), Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f);
                }
            }
            else if (type == "width")
            {
                //move Rstud on x by (current + (newwidth - oldwidth)
                //recalculate Jackstud numbers and position.x
                //recalculate Sill trimmer position.x and scale.x
                Openings[i].Vertical[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[0].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + timberWidth * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[0].transform.localScale = new Vector3(Openings[i].Width, timberWidth, timberDepth);
                Openings[i].numStuds = Mathf.FloorToInt(Openings[i].Width / maxNogging);
                if (Openings[i].oldNumStuds != Openings[i].numStuds)
                {
                    if (Openings[i].oldNumStuds < Openings[i].numStuds)
                    {
                        Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                        Openings[i].Vertical[^1].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - timberWidth * 2, timberDepth);
                        Openings[i].Vertical[^1].name = "Door" + (i + 1) + "_JackStud" + (Openings[i].Vertical.Count - 2);
                    }
                    else
                    {
                        Destroy(Openings[i].Vertical[^1]);
                        Openings[i].Vertical.RemoveAt(Openings[i].Vertical.Count - 1);
                    }
                }
                for (int a = 2; a < Openings[i].numStuds + 2; a++)
                {
                    Openings[i].Vertical[a].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 1), Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f);
                }
            }
            else if (type == "height")
            {
                //move head trimmer on y by (current + (new height - old height)
                //recalculate Jackstud position.y and scales
                Openings[i].Horizontal[0].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + timberWidth * 0.5f, timberDepth * 0.5f);
                for (int a = 2; a < Openings[i].numStuds + 2; a++)
                {
                    Openings[i].Vertical[a].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 1), Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[a].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - timberWidth * 2, timberDepth);
                }
            }
            else if (type == "new")
            {
                Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos - timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Vertical[0].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                Openings[i].Vertical[0].name = "Door" + (i + 1) + "_LStud";
                Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Vertical[1].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                Openings[i].Vertical[1].name = "Door" + (i + 1) + "_RStud";
                Openings[i].Horizontal.Add(Instantiate(Nogging, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + timberWidth * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Horizontal[0].transform.localScale = new Vector3(Openings[i].Width, timberWidth, timberDepth);
                Openings[i].Horizontal[0].name = "Door" + (i + 1) + "_HeadTrimmer";
                Openings[i].numStuds = Mathf.FloorToInt(Openings[i].Width / maxNogging);
                Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Vertical[2].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - timberWidth * 2, timberDepth);
                Openings[i].Vertical[2].name = "Door" + (i + 1) + "_JackStud1";
            }
            Openings[i].oldNumStuds = Openings[i].numStuds;
        }
        else
        {
            if (type == "XPos")
            {
                //move everything on the x by (current - (previousXPos - newXPos))
                Openings[i].Vertical[0].transform.localPosition = new Vector3(Openings[i].XPos - timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[2].transform.localPosition = new Vector3(Openings[i].XPos - timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[3].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[0].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos - timberWidth * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos + Openings[i].Height + lintelHeight * 0.5f, timberDepth * 0.5f);
                int b = 4;
                for (int a = 4; a < Openings[i].numStuds + 4; a++)
                {
                    Openings[i].Vertical[b].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos * 0.5f, timberDepth * 0.5f);
                    b += 1;
                    Openings[i].Vertical[b].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos + Openings[i].Height + lintelHeight + (wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth) * 0.5f, timberDepth * 0.5f);
                    b += 1;
                }
            }
            else if (type == "YPos")
            {
                //move the horizontal by (current - (previousYPos - newYPos))
                //move and scale jack & jamb studs by full calculation
                Openings[i].Vertical[2].transform.localPosition = new Vector3(Openings[i].XPos - timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[2].transform.localScale = new Vector3(timberWidth, Openings[i].YPos + Openings[i].Height - timberWidth, timberDepth);
                Openings[i].Vertical[3].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[3].transform.localScale = new Vector3(timberWidth, Openings[i].YPos + Openings[i].Height - timberWidth, timberDepth);
                Openings[i].Horizontal[0].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos - timberWidth * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos + Openings[i].Height + lintelHeight * 0.5f, timberDepth * 0.5f);
                int b = 4;
                for (int a = 4; a < Openings[i].numStuds + 4; a++)
                {
                    Openings[i].Vertical[b].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, Openings[i].YPos - timberWidth * 0.5f, timberDepth);
                    b += 1;
                    Openings[i].Vertical[b].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos + Openings[i].Height + lintelHeight + (wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth) * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth, timberDepth);
                    b += 1;
                }
            }
            else if (type == "width")
            {
                //move RStud & RJamb
                //scale and position sill and lintel
                //add or remove jackstud & recalculate positions
                Openings[i].Vertical[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[3].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[0].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos - timberWidth * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[0].transform.localScale = new Vector3(Openings[i].Width, timberWidth, timberDepth);
                Openings[i].Horizontal[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos + Openings[i].Height + lintelHeight * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[1].transform.localScale = new Vector3(Openings[i].Width + timberWidth * 2, lintelHeight, timberDepth);

                Openings[i].numStuds = Mathf.FloorToInt(Openings[i].Width / maxNogging);

                if (Openings[i].oldNumStuds != Openings[i].numStuds)
                {
                    if (Openings[i].oldNumStuds < Openings[i].numStuds)
                    {
                        Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                        Openings[i].Vertical[^1].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - timberWidth * 2, timberDepth);
                        Openings[i].Vertical[^1].name = "Window" + (i + 1) + "_JackStud_Bottom" + Openings[i].numStuds;
                        Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                        Openings[i].Vertical[^1].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - timberWidth * 2, timberDepth);
                        Openings[i].Vertical[^1].name = "Window" + (i + 1) + "_JackStud_Top" + Openings[i].numStuds;
                    }
                    else
                    {
                        Destroy(Openings[i].Vertical[^1]);
                        Openings[i].Vertical.RemoveAt(Openings[i].Vertical.Count-1);
                        Destroy(Openings[i].Vertical[^1]);
                        Openings[i].Vertical.RemoveAt(Openings[i].Vertical.Count - 1);
                    }
                }
                int b = 4;
                for (int a = 4; a < Openings[i].numStuds + 4; a++)
                {
                    Openings[i].Vertical[b].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, Openings[i].YPos - timberWidth * 0.5f, timberDepth);
                    b += 1;
                    Openings[i].Vertical[b].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos + Openings[i].Height + lintelHeight + (wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth) * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth, timberDepth);
                    b += 1;
                }
                
            }
            else if (type == "height")
            {
                //move and scale Jamb
                //move sill and lintel
                //move and scale top jackstuds
                Openings[i].Vertical[2].transform.localPosition = new Vector3(Openings[i].XPos - timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[2].transform.localScale = new Vector3(timberWidth, Openings[i].YPos + Openings[i].Height - timberWidth, timberDepth);
                Openings[i].Vertical[3].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f);
                Openings[i].Vertical[3].transform.localScale = new Vector3(timberWidth, Openings[i].YPos + Openings[i].Height - timberWidth, timberDepth);
                Openings[i].Horizontal[0].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos - timberWidth * 0.5f, timberDepth * 0.5f);
                Openings[i].Horizontal[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos + Openings[i].Height + lintelHeight * 0.5f, timberDepth * 0.5f);

                int b = 4;
                for (int a = 4; a < Openings[i].numStuds + 4; a++)
                {
                    b += 1;
                    Openings[i].Vertical[b].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos + Openings[i].Height + lintelHeight + (wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth) * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth, timberDepth);
                    b += 1;
                }
            }
            else if (type == "new")
            {
                Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos - timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Vertical[0].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                Openings[i].Vertical[0].name = "Window" + (i + 1) + "_LStud";
                Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Vertical[1].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                Openings[i].Vertical[1].name = "Window" + (i + 1) + "_RStud";
                Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos - timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Vertical[2].transform.localScale = new Vector3(timberWidth, Openings[i].YPos + Openings[i].Height - timberWidth, timberDepth);
                Openings[i].Vertical[2].name = "Window" + (i + 1) + "_LJambStud";
                Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Vertical[3].transform.localScale = new Vector3(timberWidth, Openings[i].YPos + Openings[i].Height - timberWidth, timberDepth);
                Openings[i].Vertical[3].name = "Window" + (i + 1) + "_RJambStud";
                Openings[i].Horizontal.Add(Instantiate(Nogging, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos - timberWidth * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Horizontal[0].transform.localScale = new Vector3(Openings[i].Width, timberWidth, timberDepth);
                Openings[i].Horizontal[0].name = "Window" + (i + 1) + "_Sill";
                Openings[i].Horizontal.Add(Instantiate(Nogging, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos + Openings[i].Height + lintelHeight * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                Openings[i].Horizontal[1].transform.localScale = new Vector3(Openings[i].Width + timberWidth * 2, lintelHeight, timberDepth);
                Openings[i].Horizontal[1].name = "Window" + (i + 1) + "_Lintel";
                Openings[i].numStuds = Mathf.FloorToInt(Openings[i].Width / maxNogging);
                int b = 4;
                for (int a = 4; a < Openings[i].numStuds + 4; a++)
                {
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, Openings[i].YPos - timberWidth * 0.5f, timberDepth);
                    Openings[i].Vertical[b].name = "Window" + (i + 1) + "_JackStud_Bottom" + (a - 3);
                    b += 1;
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos + Openings[i].Height + lintelHeight + (wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth, timberDepth);
                    Openings[i].Vertical[b].name = "Window" + (i + 1) + "_JackStud_Top" + (a - 3);
                    b += 1;
                }
            }
            Openings[i].oldNumStuds = Openings[i].numStuds;
        }
    }
    public void UpdateOpenings(string type)
    {
        if (type == "start")
        {
            for (int i = 0; i < Openings.Count; i++)
            {
                if (Openings[i].Type == "door")
                {
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos - timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[0].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                    Openings[i].Vertical[0].name = "Door" + (i + 1) + "_LStud";
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[1].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                    Openings[i].Vertical[1].name = "Door" + (i + 1) + "_RStud";
                    Openings[i].Horizontal.Add(Instantiate(Nogging, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + timberWidth * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Horizontal[0].transform.localScale = new Vector3(Openings[i].Width, timberWidth, timberDepth);
                    Openings[i].Horizontal[0].name = "Door" + (i + 1) + "_HeadTrimmer";
                    Openings[i].numStuds = Mathf.FloorToInt(Openings[i].Width / maxNogging);
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[2].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - timberWidth * 2, timberDepth);
                    Openings[i].Vertical[2].name = "Door" + (i + 1) + "JackStud 1";
                }
                else if (Openings[i].Type == "window")
                {
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos - timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[0].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                    Openings[i].Vertical[0].name = "Window" + (i + 1) + "_LStud";
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[1].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                    Openings[i].Vertical[1].name = "Window" + (i + 1) + "_RStud";
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos - timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[2].transform.localScale = new Vector3(timberWidth, Openings[i].YPos + Openings[i].Height - timberWidth, timberDepth);
                    Openings[i].Vertical[2].name = "Window" + (i + 1) + "_LJambStud";
                    Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, (Openings[i].YPos + Openings[i].Height + timberWidth) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Vertical[3].transform.localScale = new Vector3(timberWidth, Openings[i].YPos + Openings[i].Height - timberWidth, timberDepth);
                    Openings[i].Vertical[3].name = "Window" + (i + 1) + "_RJambStud";
                    Openings[i].Horizontal.Add(Instantiate(Nogging, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos - timberWidth * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Horizontal[0].transform.localScale = new Vector3(Openings[i].Width, timberWidth, timberDepth);
                    Openings[i].Horizontal[0].name = "Window" + (i + 1) + "_Sill";
                    Openings[i].Horizontal.Add(Instantiate(Nogging, new Vector3(Openings[i].XPos + Openings[i].Width * 0.5f, Openings[i].YPos + Openings[i].Height + lintelHeight * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                    Openings[i].Horizontal[1].transform.localScale = new Vector3(Openings[i].Width + timberWidth * 2, lintelHeight, timberDepth);
                    Openings[i].Horizontal[1].name = "Window" + (i + 1) + "_Lintel";
                    Openings[i].numStuds = Mathf.FloorToInt(Openings[i].Width / maxNogging);
                    int b = 4;
                    for (int a = 4; a < Openings[i].numStuds + 4; a++)
                    {
                        Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                        Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, Openings[i].YPos - timberWidth * 0.5f, timberDepth);
                        Openings[i].Vertical[b].name = "Window" + (i + 1) + "_JackStud_Bottom" + (a - 3);
                        b += 1;
                        Openings[i].Vertical.Add(Instantiate(Stud, new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos + Openings[i].Height + lintelHeight + (wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth) * 0.5f, timberDepth * 0.5f), Quaternion.identity, framingParent));
                        Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth, timberDepth);
                        Openings[i].Vertical[b].name = "Window" + (i + 1) + "_JackStud_Top" + (a - 3);
                        b += 1;
                    }
                }
            }
        }
        else if (type == "wallY")
        {
            for (int i = 0; i < Openings.Count; i++)
            {
                if (Openings[i].Type == "door")
                {
                    Openings[i].Vertical[0].transform.localPosition =  new Vector3(Openings[i].XPos - timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[0].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                    Openings[i].Vertical[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 0.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[1].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                    for (int a = 2; a < Openings[i].numStuds + 2; a++)
                    {
                        Openings[i].Vertical[a].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 1), Openings[i].Height + (wallYScale - Openings[i].Height) * 0.5f, timberDepth * 0.5f);
                        Openings[i].Vertical[a].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - timberWidth * 2, timberDepth);
                    }

                }
                else if (Openings[i].Type == "window")
                {
                    Openings[i].Vertical[0].transform.localPosition = new Vector3(Openings[i].XPos - timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[0].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                    Openings[i].Vertical[1].transform.localPosition = new Vector3(Openings[i].XPos + Openings[i].Width + timberWidth * 1.5f, wallYScale * 0.5f, timberDepth * 0.5f);
                    Openings[i].Vertical[1].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);

                    int b = 4;
                    for (int a = 4; a < Openings[i].numStuds + 4; a++)
                    {
                        b += 1;
                        Openings[i].Vertical[b].transform.localPosition =  new Vector3(Openings[i].XPos + Openings[i].Width / (Openings[i].numStuds + 1) * (a - 3), Openings[i].YPos + Openings[i].Height + lintelHeight + (wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth) * 0.5f, timberDepth * 0.5f);
                        Openings[i].Vertical[b].transform.localScale = new Vector3(timberWidth, wallYScale - Openings[i].Height - Openings[i].YPos - lintelHeight - timberWidth, timberDepth);
                        b += 1;
                    }
                }
            }
        }
    }
    public void UpdateWall(bool X)
    {
        if (X)
        {
            topPlate.localPosition = new Vector3(wallXScale / 2, wallYScale - timberWidth / 2, timberDepth / 2);
            topPlate.localScale = new Vector3(wallXScale, timberWidth, timberDepth);

            bottomPlate.localPosition = new Vector3(wallXScale / 2, timberWidth / 2, timberDepth / 2);
            bottomPlate.localScale = new Vector3(wallXScale, timberWidth, timberDepth);

            RStud.localPosition = new Vector3(wallXScale - timberWidth / 2, wallYScale / 2, timberDepth / 2);
        }
        else
        {
            topPlate.localPosition = new Vector3(wallXScale / 2, wallYScale - timberWidth / 2, timberDepth / 2);

            LStud.localPosition = new Vector3(timberWidth / 2, wallYScale / 2, timberDepth / 2);
            LStud.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);

            RStud.localPosition = new Vector3(wallXScale - timberWidth / 2, wallYScale / 2, timberDepth / 2);
            RStud.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
        }
        wallPivot.localScale = new Vector3(wallXScale, wallYScale, timberDepth);
    }
    public void WallX(float value)
    {
        wallXScale = value;
        wallWidthText.text = "Wall Width: " + System.Math.Round(wallXScale, 2) + "m";
        UpdateWall(true);
        UpdateGap(Openings.Count, "");
        if (Openings.Count != 0)
        {
            if (selectedOpening == Openings.Count - 1)
            {
                adjustSliderValue = false;
                OpeningWidthSlider.GetComponent<Slider>().maxValue = wallXScale - Openings[selectedOpening].XPos - timberWidth - timberWidth * Openings[selectedOpening].studNum;
                OpeningXPosSlider.GetComponent<Slider>().maxValue = wallXScale - timberWidth - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
                adjustSliderValue = true;
            }
        }
    }
    public void WallY(float value)
    {
        wallYScale = value;
        wallHeightText.text = "Wall Height: " + System.Math.Round(wallYScale, 2) + "m";
        UpdateWall(false);
        UpdateGaps("wallY");
        UpdateOpenings("wallY");
        if (Openings.Count != 0)
        {
            adjustSliderValue = false;
            if (Openings[selectedOpening].Type == "door")
            {
                OpeningYPosSlider.GetComponent<Slider>().maxValue = 0f;
                OpeningHeightSlider.GetComponent<Slider>().maxValue = wallYScale - timberWidth * 2f;
            }
            else
            {
                OpeningYPosSlider.GetComponent<Slider>().maxValue = wallYScale - Openings[selectedOpening].Height - lintelHeight - timberWidth;
                OpeningHeightSlider.GetComponent<Slider>().maxValue = wallYScale - Openings[selectedOpening].YPos - lintelHeight - timberWidth;
            }
            adjustSliderValue = true;
        }
    }
    public void SelectedOpening(int val)
    {
        adjustSliderValue = false;
        selectedOpening = val;
        if (val == 0)
        {
            if (val == Openings.Count - 1)
            {
                //Only Opening
                OpeningXPosSlider.GetComponent<Slider>().minValue = timberWidth + timberWidth * Openings[selectedOpening].studNum;
                OpeningXPosSlider.GetComponent<Slider>().maxValue = wallXScale - timberWidth - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
                OpeningWidthSlider.GetComponent<Slider>().maxValue = wallXScale - Openings[selectedOpening].XPos - timberWidth - timberWidth * Openings[selectedOpening].studNum;
            }
            else
            {
                //First Opening
                OpeningXPosSlider.GetComponent<Slider>().minValue = timberWidth + timberWidth * Openings[selectedOpening].studNum;
                OpeningXPosSlider.GetComponent<Slider>().maxValue = Openings[selectedOpening + 1].XPos - timberWidth * Openings[selectedOpening + 1].studNum - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
                OpeningWidthSlider.GetComponent<Slider>().maxValue = Openings[selectedOpening + 1].XPos - Openings[selectedOpening].XPos - timberWidth * Openings[selectedOpening + 1].studNum - timberWidth * Openings[selectedOpening].studNum;
            }
        }
        else if (val == Openings.Count - 1)
        {
            //Last Opening
            OpeningXPosSlider.GetComponent<Slider>().minValue = Openings[selectedOpening - 1].XPos + Openings[selectedOpening - 1].Width + timberWidth * Openings[selectedOpening - 1].studNum + timberWidth * Openings[selectedOpening].studNum;
            OpeningXPosSlider.GetComponent<Slider>().maxValue = wallXScale - timberWidth - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
            OpeningWidthSlider.GetComponent<Slider>().maxValue = wallXScale - Openings[selectedOpening].XPos - timberWidth - timberWidth * Openings[selectedOpening].studNum;
        }
        else
        {
            //In between Opening
            OpeningXPosSlider.GetComponent<Slider>().minValue = Openings[selectedOpening - 1].XPos + Openings[selectedOpening - 1].Width + timberWidth * Openings[selectedOpening - 1].studNum + timberWidth * Openings[selectedOpening].studNum;
            OpeningXPosSlider.GetComponent<Slider>().maxValue = Openings[selectedOpening + 1].XPos - timberWidth * Openings[selectedOpening + 1].studNum - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
            OpeningWidthSlider.GetComponent<Slider>().maxValue = Openings[selectedOpening + 1].XPos - Openings[selectedOpening].XPos - timberWidth * Openings[selectedOpening + 1].studNum - timberWidth * Openings[selectedOpening].studNum;
        }

        if (Openings[selectedOpening].Type == "door")
        {
            OpeningYPosSlider.GetComponent<Slider>().enabled = false;
            OpeningYPosSlider.GetComponent<Slider>().maxValue = 0f;
            OpeningYPosSlider.GetComponent<Slider>().minValue = 0f;
            OpeningWidthSlider.GetComponent<Slider>().minValue = 0.6f;
            OpeningHeightSlider.GetComponent<Slider>().maxValue = wallYScale - timberWidth * 2f;
            OpeningHeightSlider.GetComponent<Slider>().minValue = minDoorHeight;
        }
        else
        {
            OpeningYPosSlider.GetComponent<Slider>().enabled = true;
            OpeningYPosSlider.GetComponent<Slider>().maxValue = wallYScale - Openings[selectedOpening].Height - lintelHeight - timberWidth;
            OpeningYPosSlider.GetComponent<Slider>().minValue = timberWidth * 2f;
            OpeningWidthSlider.GetComponent<Slider>().minValue = 0.1f;
            OpeningHeightSlider.GetComponent<Slider>().maxValue = wallYScale - Openings[selectedOpening].YPos - lintelHeight - timberWidth;
            OpeningHeightSlider.GetComponent<Slider>().minValue = 0.1f;
        }
        OpeningXPosSlider.GetComponent<Slider>().value = Openings[selectedOpening].XPos;
        openingXPosText.text = "Opening X Position: " + System.Math.Round(Openings[selectedOpening].XPos, 2) + "m";
        OpeningYPosSlider.GetComponent<Slider>().value = Openings[selectedOpening].YPos;
        openingYPosText.text = "Opening Y Position: " + System.Math.Round(Openings[selectedOpening].YPos, 2) + "m";
        OpeningWidthSlider.GetComponent<Slider>().value = Openings[selectedOpening].Width;
        openingWidthText.text = "Opening Width: " + System.Math.Round(Openings[selectedOpening].Width, 2) + "m";
        OpeningHeightSlider.GetComponent<Slider>().value = Openings[selectedOpening].Height;
        openingHeightText.text = "Opening Height: " + System.Math.Round(Openings[selectedOpening].Height, 2) + "m";

        adjustSliderValue = true;
    }
    public void OpeningXPos(float value)
    {
        if (adjustSliderValue)
        {
            Openings[selectedOpening].XPos = value;
            openingXPosText.text = "Opening X Position: " + System.Math.Round(value, 2) + "m";
            UpdateOpening(selectedOpening, "XPos");
            UpdateGap(selectedOpening, "");
            UpdateGap(selectedOpening + 1, "");
            adjustSliderValue = false;
            if (selectedOpening == 0)
            {
                if (selectedOpening == Openings.Count - 1)
                {
                    OpeningWidthSlider.GetComponent<Slider>().maxValue = wallXScale - Openings[selectedOpening].XPos - timberWidth - timberWidth * Openings[selectedOpening].studNum;
                    wallXSlider.GetComponent<Slider>().minValue = Openings[^1].XPos + Openings[^1].Width + Openings[^1].studNum * timberWidth + timberWidth;
                }
                else
                {
                    OpeningWidthSlider.GetComponent<Slider>().maxValue = Openings[selectedOpening + 1].XPos - Openings[selectedOpening].XPos - timberWidth * Openings[selectedOpening + 1].studNum - timberWidth * Openings[selectedOpening].studNum;
                }
            }
            else if (selectedOpening == Openings.Count - 1)
            {
                OpeningWidthSlider.GetComponent<Slider>().maxValue = wallXScale - Openings[selectedOpening].XPos - timberWidth - timberWidth * Openings[selectedOpening].studNum;
                wallXSlider.GetComponent<Slider>().minValue = Openings[^1].XPos + Openings[^1].Width + Openings[^1].studNum * timberWidth + timberWidth;
            }
            else
            {
                OpeningWidthSlider.GetComponent<Slider>().maxValue = Openings[selectedOpening + 1].XPos - Openings[selectedOpening].XPos - timberWidth * Openings[selectedOpening + 1].studNum - timberWidth * Openings[selectedOpening].studNum;
            }
            adjustSliderValue = true;
        }
    }
    public void OpeningYPos(float value)
    {
        if (adjustSliderValue)
        {
            Openings[selectedOpening].YPos = value;
            openingYPosText.text = "Opening Y Position: " + System.Math.Round(value, 2) + "m";
            UpdateOpening(selectedOpening, "YPos");
            adjustSliderValue = false;
            if (Openings[selectedOpening].Type == "door")
            {
                OpeningHeightSlider.GetComponent<Slider>().maxValue = wallYScale - timberWidth * 2f;
            }
            else
            {
                OpeningHeightSlider.GetComponent<Slider>().maxValue = wallYScale - Openings[selectedOpening].YPos - lintelHeight - timberWidth;
            }
            UpdateWallYSlider();
            adjustSliderValue = true;
        }
    }
    public void OpeningWidth(float value)
    {
        if (adjustSliderValue)
        {
            Openings[selectedOpening].Width = value;
            openingWidthText.text = "Opening Width: " + System.Math.Round(value, 2) + "m";
            UpdateOpening(selectedOpening, "width");
            UpdateGap(selectedOpening + 1, "");
            adjustSliderValue = false;
            if (selectedOpening == 0)
            {
                if (selectedOpening == Openings.Count - 1)
                {
                    OpeningXPosSlider.GetComponent<Slider>().maxValue = wallXScale - timberWidth - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
                }
                else
                {
                    OpeningXPosSlider.GetComponent<Slider>().maxValue = Openings[selectedOpening + 1].XPos - timberWidth * Openings[selectedOpening + 1].studNum - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
                }
            }
            else if (selectedOpening == Openings.Count - 1)
            {
                OpeningXPosSlider.GetComponent<Slider>().maxValue = wallXScale - timberWidth - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
            }
            else
            {
                OpeningXPosSlider.GetComponent<Slider>().maxValue = Openings[selectedOpening + 1].XPos - timberWidth * Openings[selectedOpening + 1].studNum - Openings[selectedOpening].Width - timberWidth * Openings[selectedOpening].studNum;
            }
            adjustSliderValue = true;
        }
    }
    public void OpeningHeight(float value)
    {
        if (adjustSliderValue)
        {
            Openings[selectedOpening].Height = value;
            openingHeightText.text = "Opening Height: " + System.Math.Round(value, 2) + "m";
            UpdateOpening(selectedOpening, "height");
            adjustSliderValue = false;
            if (Openings[selectedOpening].Type == "door")
            {
                OpeningYPosSlider.GetComponent<Slider>().maxValue = wallYScale - Openings[selectedOpening].Height - timberWidth * 2;
            }
            else
            {
                OpeningYPosSlider.GetComponent<Slider>().maxValue = wallYScale - Openings[selectedOpening].Height - lintelHeight - timberWidth;
            }
            UpdateWallYSlider();
            adjustSliderValue = true;
        }
    }
    public void UpdateGap(int i, string type)
    {
        if (i == 0)
        {
            if (i == Openings.Count)
            {
                //No Openings
                //Gap is from LStud to RStud
                Gaps[i].Width = wallXScale - timberWidth;
                Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / maxNogging);
                Gaps[i].XPos = timberWidth * 0.5f;
            }
            else
            {
                //First Gap
                //Gag is from LStud to Opening[0]
                Gaps[i].Width = Openings[0].XPos - timberWidth * Openings[0].studNum;
                Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / maxNogging);
                Gaps[i].XPos = timberWidth * 0.5f;
            }
        }
        else if (i == Openings.Count)
        {
            //Last Gap
            //Gap is from Opening[Openings.Count - 1] to RStud
            Gaps[i].Width = wallXScale - Openings[^1].XPos - Openings[^1].Width - timberWidth * Openings[^1].studNum;
            Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / maxNogging);
            Gaps[i].XPos = Openings[^1].XPos + Openings[^1].Width + timberWidth * (Openings[^1].studNum - 0.5f);
        }
        else
        {
            //All in-between gaps
            //Gaps is from Opening[i-1] to Opening[i]
            Gaps[i].Width = Openings[i].XPos - Openings[i - 1].XPos - Openings[i - 1].Width - timberWidth * (Openings[i].studNum + Openings[i - 1].studNum - 1);
            Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / maxNogging);
            Gaps[i].XPos = Openings[i-1].XPos + Openings[i-1].Width + timberWidth * (Openings[i-1].studNum - 0.5f);
        }
        if (type == "new")
        {
            Gaps[i].Noggings.Add(Instantiate(Nogging, new Vector3(0, 0, 0), Quaternion.identity, framingParent));
            Gaps[i].Noggings[0].name = "Gap" + (i + 1) + "_Nogging1";
        }
        if (Gaps[i].oldNumStuds != Gaps[i].numStuds)
        {
            if (Gaps[i].oldNumStuds < Gaps[i].numStuds)
            {

                for (int a = 0; a < (Gaps[i].numStuds - Gaps[i].oldNumStuds); a++)
                {
                    Gaps[i].Studs.Add(Instantiate(Stud, new Vector3(0, 0, 0), Quaternion.identity, framingParent));
                    Gaps[i].Studs[^1].name = "Gap" + (i + 1) + "_Stud" + Gaps[i].Studs.Count;
                    Gaps[i].Noggings.Add(Instantiate(Nogging, new Vector3(0, 0, 0), Quaternion.identity, framingParent));
                    Gaps[i].Noggings[^1].name = "Gap" + (i + 1) + "_Nogging" + Gaps[i].Noggings.Count;
                }
            }
            else if (Gaps[i].oldNumStuds > Gaps[i].numStuds)
            {
                for (int a = 0; a < (Gaps[i].oldNumStuds - Gaps[i].numStuds); a++)
                {
                    Destroy(Gaps[i].Studs[^1]);
                    Destroy(Gaps[i].Noggings[^1]);
                    Gaps[i].Studs.RemoveAt(Gaps[i].Studs.Count-1);
                    Gaps[i].Noggings.RemoveAt(Gaps[i].Noggings.Count - 1);
                }
            }
        }
        for (int a = 0; a < Gaps[i].numStuds + 1; a++)
        {
            if (a > 0)
            {
                Gaps[i].Studs[a - 1].transform.localPosition = new Vector3(Gaps[i].XPos + Gaps[i].Width / (1 + Gaps[i].numStuds) * a, wallYScale * 0.5f, timberDepth * 0.5f);
                Gaps[i].Studs[a - 1].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
            }

            if (a % 2 == 0)
            {
                Gaps[i].Noggings[a].transform.localPosition = new Vector3(Gaps[i].XPos + Gaps[i].Width / ((1 + Gaps[i].numStuds) * 2) * (a * 2 + 1), wallYScale * 0.5f + timberWidth * 0.5f, timberDepth * 0.5f);
            }
            else
            {
                Gaps[i].Noggings[a].transform.localPosition = new Vector3(Gaps[i].XPos + Gaps[i].Width / ((1 + Gaps[i].numStuds) * 2) * (a * 2 + 1), wallYScale * 0.5f - timberWidth * 0.5f, timberDepth * 0.5f);
            }
            Gaps[i].Noggings[a].transform.localScale = new Vector3(Gaps[i].Width / (Gaps[i].numStuds + 1) - timberWidth, timberWidth, timberDepth);
        }
        Gaps[i].oldNumStuds = Gaps[i].numStuds;
    }
    public void UpdateGaps(string type)
    {
        for (int i = 0; i < Gaps.Count; i++)
        {
            if (i == 0)
            {
                if (i == Openings.Count)
                {
                    //No Openings
                    //Gap is from LStud to RStud
                    Gaps[i].Width = wallXScale - timberWidth;
                    Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / maxNogging);
                    Gaps[i].XPos = timberWidth * 0.5f;
                }
                else
                {
                    //First Gap
                    //Gag is from LStud to Opening[0]
                    Gaps[i].Width = Openings[0].XPos - timberWidth * Openings[0].studNum;
                    Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / maxNogging);
                    Gaps[i].XPos = timberWidth * 0.5f;
                }
            }
            else if (i == Openings.Count)
            {
                //Last Gap
                //Gap is from Opening[Openings.Count - 1] to RStud
                Gaps[i].Width = wallXScale - Openings[^1].XPos - Openings[^1].Width - timberWidth * Openings[^1].studNum;
                Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / maxNogging);
                Gaps[i].XPos = Openings[^1].XPos + Openings[^1].Width + timberWidth * (Openings[^1].studNum - 0.5f);
            }
            else
            {
                //All in-between gaps
                //Gaps is from Opening[i-1] to Opening[i]
                Gaps[i].Width = Openings[i].XPos - Openings[i - 1].XPos - Openings[i - 1].Width - timberWidth * (Openings[i].studNum + Openings[i - 1].studNum - 1);
                Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / maxNogging);
                Gaps[i].XPos = Openings[i - 1].XPos + Openings[i - 1].Width + timberWidth * (Openings[i - 1].studNum - 0.5f);
            }
            if (type == "start")
            {
                for (int a = 0; a < Gaps[i].numStuds + 1; a++)
                {
                    if (a > 0)
                    {
                        Gaps[i].Studs.Add(Instantiate(Stud, new Vector3(0, 0, 0), Quaternion.identity, framingParent));
                        Gaps[i].Studs[^1].name = "Gap" + (i + 1) + "_Stud" + Gaps[i].Studs.Count;
                    }
                    Gaps[i].Noggings.Add(Instantiate(Nogging, new Vector3(0, 0, 0), Quaternion.identity, framingParent));
                    Gaps[i].Noggings[^1].name = "Gap" + (i + 1) + "_Nogging" + Gaps[i].Noggings.Count;
                }
            }
            else if (Gaps[i].oldNumStuds != Gaps[i].numStuds)
            {
                if (Gaps[i].oldNumStuds < Gaps[i].numStuds)
                {
                    Gaps[i].Studs.Add(Instantiate(Stud, new Vector3(0, 0, 0), Quaternion.identity, framingParent));
                    Gaps[i].Studs[^1].name = "Gap" + (i + 1) + "_Stud" + Gaps[i].Studs.Count;
                    Gaps[i].Noggings.Add(Instantiate(Nogging, new Vector3(0, 0, 0), Quaternion.identity, framingParent));
                    Gaps[i].Noggings[^1].name = "Gap" + (i + 1) + "_Nogging" + Gaps[i].Noggings.Count;
                }
                else if (Gaps[i].oldNumStuds > Gaps[i].numStuds)
                {
                    Destroy(Gaps[i].Studs[Gaps[i].numStuds]);
                    Destroy(Gaps[i].Noggings[Gaps[i].oldNumStuds]);
                    Gaps[i].Studs.RemoveAt(Gaps[i].numStuds);
                    Gaps[i].Noggings.RemoveAt(Gaps[i].oldNumStuds);
                }
            }
            for (int a = 0; a < Gaps[i].numStuds + 1; a++)
            {
                if (a > 0)
                {
                    Gaps[i].Studs[a - 1].transform.localPosition = new Vector3(Gaps[i].XPos + Gaps[i].Width / (1 + Gaps[i].numStuds) * a, wallYScale * 0.5f, timberDepth * 0.5f);
                    Gaps[i].Studs[a - 1].transform.localScale = new Vector3(timberWidth, wallYScale - timberWidth * 2, timberDepth);
                }

                if (a % 2 == 0)
                {
                    Gaps[i].Noggings[a].transform.localPosition = new Vector3(Gaps[i].XPos + Gaps[i].Width / ((1 + Gaps[i].numStuds) * 2) * (a * 2 + 1), wallYScale * 0.5f + timberWidth * 0.5f, timberDepth * 0.5f);
                }
                else
                {
                    Gaps[i].Noggings[a].transform.localPosition = new Vector3(Gaps[i].XPos + Gaps[i].Width / ((1 + Gaps[i].numStuds) * 2) * (a * 2 + 1), wallYScale * 0.5f - timberWidth * 0.5f, timberDepth * 0.5f);
                }
                Gaps[i].Noggings[a].transform.localScale = new Vector3(Gaps[i].Width / (Gaps[i].numStuds + 1) - timberWidth, timberWidth, timberDepth);
            }
            Gaps[i].oldNumStuds = Gaps[i].numStuds;
        }
    }
    public void UpdateDropdown()
    {
        if (Openings.Count != 0)
        {
            for (int i = 0; i < OpeningsDropdown.options.Count; i++)
            {
                if (Openings[i].Type == "door")
                {
                    OpeningsDropdown.options[i].text = "Door " + (i + 1);
                }
                else
                {
                    OpeningsDropdown.options[i].text = "Window " + (i + 1);
                }

            }
        }
        else
        {
            OpeningsDropdown.RefreshShownValue();
        }
    }
    public void UpdateWallYSlider()
    {
        float maxValue = 0f;
        for (int i = 0; i < Openings.Count; i++)
        {
            if (Openings[i].Type == "door")
            {
                if (Openings[i].YPos + Openings[i].Height + timberWidth * 2 > maxValue)
                {
                    maxValue = Openings[i].YPos + Openings[i].Height + timberWidth * 2;
                }
            }
            else
            {
                if (Openings[i].YPos + Openings[i].Height + timberWidth + lintelHeight > maxValue)
                {
                    maxValue = Openings[i].YPos + Openings[i].Height + timberWidth + lintelHeight;
                }
            }
        }
        highestOpeningPoint = maxValue;
        wallYSlider.GetComponent<Slider>().minValue = highestOpeningPoint;
        wallYSlider.GetComponent<Slider>().maxValue = highestOpeningPoint + 3f;
    }
}
