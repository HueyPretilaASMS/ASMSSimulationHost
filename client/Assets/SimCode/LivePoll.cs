using System.IO;
using UnityEngine.Networking;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LivePoll : MonoBehaviour {
	// REFLECTION
	public GameObject downstairs, upstairs;
	public Text timeLabel;

	// MODEL
	String content = "";
	float in_Down = 0, in_Up = 0;

	// TIME
	int sixOClock = 0, currentTime = 1800, currentOffset = 0;
	float nextActionTime = 0.0f;
	float period = 0.5f;

	// INIT
	void Start () {
		StartCoroutine(GetText());
	}

	IEnumerator GetText() {
		UnityWebRequest www = UnityWebRequest.Get("https://raw.githubusercontent.com/HueyPretilaASMS/ASMSSimulationHost/master/log.txt");
		yield return www.Send();

		if(www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		}
		else {
			// Show results as text
			Debug.Log(www.downloadHandler.text);
			content = www.downloadHandler.text;
			// Or retrieve results as binary data
			byte[] results = www.downloadHandler.data;
		}

		string[] anaphaseI = content.Split(';');
		string[] anaphaseII = anaphaseI[0].Split(',');

		int.TryParse (anaphaseII [1], out sixOClock);
	}
	
	// UPDATE
	void Update ()  { 
		if (Time.time > nextActionTime ) { 
			nextActionTime = Time.time + period; 

			// CALCULATE VARIABLES
			string[] anaphaseI = content.Split(';');
			foreach (string a_I in anaphaseI) {
				string[] anaphaseII = a_I.Split(',');
				string typeMove = anaphaseII [0];
				int secondsGenisis = int.Parse(anaphaseII [1]);

				if ((secondsGenisis - sixOClock) != currentOffset)
					break;

				if (typeMove == "in_ASMS") {
					if (in_Down >= 0)
						in_Down++;
				}
				if (typeMove == "in_1") {
					if (in_Down >= 0)
						in_Down++;
					if (in_Up > 0)
						in_Up--;
				}
				if (typeMove == "in_2") {
					if (in_Up >= 0)
						in_Up++;
					if (in_Down > 0)
						in_Down--;
				}
			}

			currentOffset++;
			currentTime++;

			// RENDER
			downstairs.transform.localScale = 
				new Vector3(1, 8*(in_Down/(in_Down + in_Up)), 1);
			upstairs.transform.localScale = 
				new Vector3(1, 8*(in_Up/(in_Down + in_Up)), 1);

			timeLabel.text = currentTime.ToString();
		} 
	}
}
