using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FramingSystemOptimised : MonoBehaviour
{
    public List<float> TimberLengths;
    public List<string> TimberNames;

    public float WallXScale;
    public float WallYScale;
    public float MaxNogging = 0.45f;
    public float TimberWidth = 0.045f;
    public float TimberDepth = 0.09f;
    public float LintelHeight = 0.18f;
    public float MinDoorHeight = 1.80f;
    public List<Opening> Openings;
    public List<Gap> Gaps;
    public int NumDoors;
    public int NumWindows;
    public TextMeshProUGUI TimberLengthsText;

    public void UpdateText()
    {
        TimberLengthsText.text = "";
        for (int i = 0; i < TimberLengths.Count; i++)
        {
            TimberLengthsText.text += "\n " + TimberNames[i] + ": " + TimberLengths[i] + "m";
        }
    }

    public void NewLayout()
    {
        Openings.Clear();
        Gaps.Clear();
        float spacing = WallXScale / (NumDoors + NumWindows);
        Gaps.Add(new(TimberWidth * 0.5f, spacing * 0.25f));
        for (int i = 0; i < (NumDoors + NumWindows); i++)
        {
            if (i < NumDoors)
            {
                Openings.Add(new("door", (i + 0.25f) * spacing, 0.0f, spacing * 0.5f, WallYScale * 0.8f, 1));
                Gaps.Add(new(Openings[i].XPos + Openings[i].Width, spacing * 0.5f));
            }
            else
            {
                Openings.Add(new("window", (i + 0.25f) * spacing, WallYScale * 0.2f, spacing * 0.5f, WallYScale * 0.5f, 2));
                Gaps.Add(new(Openings[i].XPos + Openings[i].Width, spacing * 0.5f));
            }
        }
        NewFraming();
    }

    public void NewFraming()
    {

        TimberLengths.Clear();
        TimberNames.Clear();

        TimberNames.Add("TopPlate");
        TimberLengths.Add(WallXScale);

        TimberNames.Add("BottomPlate");
        TimberLengths.Add(WallXScale);

        TimberNames.Add("LeftStud");
        TimberLengths.Add(WallYScale - TimberWidth * 2);

        TimberNames.Add("RightStud");
        TimberLengths.Add(WallYScale - TimberWidth * 2);

        for (int i = 0; i < Openings.Count; i++)
        {
            if (Openings[i].Type == "door")
            {
                TimberNames.Add("Door" + (i+1) + "_LeftStud");
                TimberLengths.Add(WallYScale - TimberWidth * 2);

                TimberNames.Add("Door" + (i + 1) + "_RightStud");
                TimberLengths.Add(WallYScale - TimberWidth * 2);

                TimberNames.Add("Door" + (i + 1) + "_HeadTrimmer");
                TimberLengths.Add(Openings[i].Width);

                Openings[i].numStuds = Mathf.FloorToInt(Openings[i].Width / MaxNogging);

                for (int a = 0; a < Openings[i].numStuds; a++)
                {
                    TimberNames.Add("Door" + (i + 1) + "_JackStud" + (a+1));
                    TimberLengths.Add(WallYScale - Openings[i].Height - TimberWidth * 2);
                }
            }
            else
            {
                TimberNames.Add("Window" + (i + 1) + "_LeftStud");
                TimberLengths.Add(WallYScale - TimberWidth * 2);

                TimberNames.Add("Window" + (i + 1) + "_RightStud");
                TimberLengths.Add(WallYScale - TimberWidth * 2);

                TimberNames.Add("Window" + (i + 1) + "_LeftJambStud");
                TimberLengths.Add(Openings[i].YPos + Openings[i].Height - TimberWidth);

                TimberNames.Add("Window" + (i + 1) + "_RightJambStud");
                TimberLengths.Add(Openings[i].YPos + Openings[i].Height - TimberWidth);

                TimberNames.Add("Window" + (i + 1) + "_Sill");
                TimberLengths.Add(Openings[i].Width);

                TimberNames.Add("Window" + (i + 1) + "_Lintel");
                TimberLengths.Add(Openings[i].Width + TimberWidth * 2);

                Openings[i].numStuds = Mathf.FloorToInt(Openings[i].Width / MaxNogging);

                for (int a = 0; a < Openings[i].numStuds; a++)
                {
                    TimberNames.Add("Window" + (i + 1) + "_JackStud_Bottom" + (a + 1));
                    TimberLengths.Add(Openings[i].YPos - TimberWidth * 0.5f);
                    TimberNames.Add("Window" + (i + 1) + "_JackStud_Top" + (a + 1));
                    TimberLengths.Add(WallYScale - Openings[i].Height - Openings[i].YPos - LintelHeight - TimberWidth);
                }
            }
        }
        for (int i = 0; i < Gaps.Count; i++)
        {
            if (i == 0)
            {
                if (i == Openings.Count)
                {
                    //No Openings
                    //Gap is from LStud to RStud
                    Gaps[i].Width = WallXScale - TimberWidth;
                    Gaps[i].XPos = TimberWidth * 0.5f;
                }
                else
                {
                    //First Gap
                    //Gag is from LStud to Opening[0]
                    Gaps[i].Width = Openings[0].XPos - TimberWidth * Openings[0].studNum;
                    Gaps[i].XPos = TimberWidth * 0.5f;
                }
            }
            else if (i == Openings.Count)
            {
                //Last Gap
                //Gap is from Opening[Openings.Count - 1] to RStud
                Gaps[i].Width = WallXScale - Openings[^1].XPos - Openings[^1].Width - TimberWidth * Openings[^1].studNum;
                Gaps[i].XPos = Openings[^1].XPos + Openings[^1].Width + TimberWidth * (Openings[^1].studNum - 0.5f);
            }
            else
            {
                //All in-between gaps
                //Gaps is from Opening[i-1] to Opening[i]
                Gaps[i].Width = Openings[i].XPos - Openings[i - 1].XPos - Openings[i - 1].Width - TimberWidth * (Openings[i].studNum + Openings[i - 1].studNum - 1);
                Gaps[i].XPos = Openings[i - 1].XPos + Openings[i - 1].Width + TimberWidth * (Openings[i - 1].studNum - 0.5f);
            }
            Gaps[i].numStuds = Mathf.FloorToInt(Gaps[i].Width / MaxNogging);

            for (int a = 0; a < Gaps[i].numStuds + 1; a++)
            {
                if (a > 0)
                {
                    TimberNames.Add("Gap" + (i + 1) + "_Stud" + a);
                    TimberLengths.Add(WallYScale - TimberWidth * 2);
                }
                if ((Gaps[i].Width / (Gaps[i].numStuds + 1) - TimberWidth) > 0)
                {
                    TimberNames.Add("Gap" + (i + 1) + "_nogging" + (a + 1));
                    TimberLengths.Add(Gaps[i].Width / (Gaps[i].numStuds + 1) - TimberWidth);
                }
            }
        }
        UpdateText();
    }
}
