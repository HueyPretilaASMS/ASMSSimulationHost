using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LivePoll;

public class LivePollBubble : MonoBehaviour {
    public Transform downState;
    public Transform upState;
    public float speed = 1;
    public LivePoll.LivePoll livePoll;

    public Transform targettedState;
    public string currentState = "DOWN";

    float currentTime = 0;
    float nextStep = 0.0f;
    float period = 2f;

    // Use this for initialization
    void Start () {
        targettedState = downState;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Implemented movement");
        float movement = speed * Vector3.Distance(targettedState.position, transform.position) * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targettedState.position, movement);

        if (currentState == "DOWN")
        {
            targettedState = downState;
        }
        else
        {
            targettedState = upState;
        }
        Debug.Log("Changed states");


        if (Time.time > nextStep)
        {
            nextStep = Time.time + period;
            if (currentTime != livePoll.rawTime)
            {
                var rnd = new System.Random();
                float p = (float)rnd.Next(0, 100) / 100;

                if (currentState == "DOWN")
                {

                    if (p < (float)livePoll.down / 100)
                        currentState = "DOWN";
                    else
                        currentState = "UP";

                    Debug.LogWarning(p + " vs Down prob " + (float)livePoll.down / 100);
                }
                else
                {
                    if (p < (float)livePoll.up / 100)
                        currentState = "UP";
                    else
                        currentState = "DOWN";

                    Debug.LogWarning(p + " vs Up prob " + (float)livePoll.up / 100);
                }


                currentTime = livePoll.rawTime;
            }
        }
        Debug.Log("Check if time changed.");

    }
}
