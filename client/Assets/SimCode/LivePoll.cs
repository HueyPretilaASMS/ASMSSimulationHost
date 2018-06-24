using System.IO;
using UnityEngine.Networking;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LivePoll
{
    public class PollProbability
    {
        public float p_Down, p_Up;

        /*
         * 2x2 Transition Matrix
         *      Up      Down
         *      --------------
         * Up  |in_Up   r_Down
         * Down|r_Up    in_Down
         */

        public PollProbability(float r_Down, float r_Up, float in_Down, float in_Up)
        {
            p_Down = in_Down / (in_Down + in_Up);
            p_Up = in_Up / (in_Down + in_Up);
        }
    }

    public class LivePoll : MonoBehaviour
    {
        // REFLECTION
        public bool doRender = true;
        public TextMesh downstairs, upstairs;
        public TextMesh timeLabel;
        public String linkCorpus = "https://raw.githubusercontent.com/HueyPretilaASMS/ComplexSystemModelling/master/log.txt";

        // MODEL
        String content = "";
        float in_Down = 0, in_Up = 0, in_All = 0;
        public List<PollProbability> probabilities = new List<PollProbability>();

        // TIME
        public float hour = 18, minutes = 0, rawTime = 1080;
        float nextStep = 0.0f;
        float period = 0.5f;
        public int up = 1, down = 0;

        public PollProbability currentProbability;

        // INIT
        void Start()
        {
            StartCoroutine(GetText());
        }

        public IEnumerator GetText()
        {
            UnityWebRequest req = UnityWebRequest.Get(linkCorpus);

            yield return req.Send();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
                Debug.Log("Using backup corpus.");

                req = UnityWebRequest.Get("https://raw.githubusercontent.com/HueyPretilaASMS/ComplexSystemModelling/master/log.txt");
                yield return req.Send();

                // Show results as text
                Debug.Log(req.downloadHandler.text);
                content = req.downloadHandler.text;
                // Or retrieve results as binary data
                byte[] results = req.downloadHandler.data;
            }
            else
            {
                // Show results as text
                Debug.Log(req.downloadHandler.text);
                content = req.downloadHandler.text;
                // Or retrieve results as binary data
                byte[] results = req.downloadHandler.data;
            }

            // Initialisation + Delimination
            string[] anaphaseI = content.Split(';');
            string[] anaphaseII = anaphaseI[0].Split(',');
            float.TryParse(anaphaseII[2], out rawTime);
        }

        // UPDATE
        public void Update()
        {
            if (Time.time > nextStep)
            {
                nextStep = Time.time + period;
                Step();
            }
        }

        public void Step()
        {
            if (hour == 24)
                ;
            else
            {

                CalculateStep();

                hour = (float)Math.Floor((decimal)(rawTime / 60));
                minutes = rawTime - (hour * 60);

                Debug.Log(hour + " Hours and " + minutes + " Minutes at " + rawTime);

                if (doRender)
                {
                    // RENDER
                    /*
                    downstairs.transform.localScale =
                        new Vector3(1, 8 * (in_Down / (in_Down + in_Up)), 1);
                    upstairs.transform.localScale =
                        new Vector3(1, 8 * (in_Up / (in_Down + in_Up)), 1);*/
                    bool update = true;
                    /*
                    if (((r_Down / (r_Down + in_Down)) + (r_Up / (r_Up + in_Up))) > 0.5)
                    {
                        up = (100 * (r_Up / (r_Up + in_Up)));
                        down = (100 * (r_Down / (r_Down + in_Down)));
                    } else
                    {
                        if (float.IsNaN(100 * (r_Up / (r_Up + in_Up))) && float.IsNaN(100 * (r_Down / (r_Down + in_Down))))
                            ;
                        else
                        {
                            // Subtract the difference
                            if (float.IsNaN(100 * (r_Up / (r_Up + in_Up))))
                            {
                                up = (100 - (100 * (r_Down / (r_Down + in_Down))));
                            }

                            if (float.IsNaN(100 * (r_Down / (r_Down + in_Down))))
                            {
                                down = (100 - (100 * (r_Up / (r_Up + in_Up))));
                            }

                            update = true;
                        }

                    }

                    //if (100-(down + up)!=0)
                    {
                        float diff = 100 - (down + up);
                        if (up + diff >= 0){
                            up += diff;
                        } else
                        {
                            down += diff;
                        }
                    }*/

                    if (!float.IsNaN(r_Up / in_All) && !float.IsNaN(r_Down / in_All) // If there is a recording error, omit
                        && !(((r_Up / in_All)==0)&& (r_Down / in_All) == 0)) // If both are zero, there was no data change hence maintain the original
                    {
                        up = (int)(100 * in_Up / (in_Down + in_Up));
                        down = (int)(100 * in_Down / (in_Down + in_Up));
                    }
                    
                    upstairs.text = up.ToString() + "%";
                    downstairs.text = down.ToString() + "%";

                    if (minutes > 9)
                        timeLabel.text = hour.ToString() + minutes.ToString();
                    else
                        timeLabel.text = hour.ToString() + "0" + minutes.ToString();
                    rawTime++;

                }
            }
        }

        float r_Down = 0, r_Up = 0;
        // CALCULATE VARIABLES
        public void CalculateStep()
        {
            r_Down = 0;
            r_Up = 0;
            string[] anaphaseI = content.Split(';');

            foreach (string a_I in anaphaseI)
            {
                string[] anaphaseII = a_I.Split(',');
                string typeMove = anaphaseII[0];
                float timeNeeded;
                try
                {
                    timeNeeded = float.Parse(anaphaseII[2]);
                } catch { timeNeeded = 0; }

                // You're not supposed to be calculated
                if (timeNeeded == rawTime)
                {
                    
                    Debug.Log(rawTime + " " + a_I + " from " + timeNeeded);

                    if (typeMove == "in_ASMS")
                    {
                        in_All++;
                        if (in_Down >= 0)
                        {
                            in_Down++;
                            r_Down++;
                        }
                    }

                    if (typeMove == "in_1")
                    {
                        if (in_Down >= 0)
                        {
                            in_Down++;
                            r_Down++;
                        }
                        if (!(in_Up > 0)) //CLIP THE DATA IF THERE ISN'T ANY NEW PEOPLE
                            in_All++;
                        else
                        {
                            in_Up--;
                        }
                    }

                    if (typeMove == "in_2")
                    {
                        if (in_Up >= 0)
                        {
                            r_Up++;
                            in_Up++;
                        }
                        if (in_Down > 0) //CLIP THE DATA IF THERE ISN'T ANY NEW PEOPLE
                            in_All++;
                        else
                        {
                            in_Down--;
                        }
                    }
                }
            }

            PollProbability probability = new PollProbability(r_Down, r_Up, in_Down, in_Up);
        }
    }
}