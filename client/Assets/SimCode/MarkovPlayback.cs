using System;
using System.Collections.Generic;
using UnityEngine;
using Markov;

namespace LivePoll
{
    public class MarkovPlayback : MonoBehaviour
    {
        public LivePoll livePoll;
        public MarkovChain<string> markov = new MarkovChain<string>(1);
        List<string[]> corpuses = new List<string[]>();

        void Start()
        {
            // POPULATE POLL DATA
            if (livePoll == null)
            {
                livePoll = new LivePoll();
                StartCoroutine(livePoll.GetText());
                for (int i = 0; i < 50; i++)
                {
                    Debug.Log("K");
                    livePoll.Step();
                }
            }

            // POPULATE CORPUS WITH COMPLIANT RULES
            for (int i = 0; i < 100; i++)
            {
                string[] corpus = new string[10];
                for (int b = 0; b < livePoll.probabilities.Count; b++)
                {
                    float p = UnityEngine.Random.Range(0.0F, 1.0F);

                    if (b == 0)
                        corpus[0] = "DOWN"; else
                        if (corpus[b-1] == "DOWN")
                            if (p > livePoll.probabilities[b].p_Down)
                                corpus[b] = "DOWN";
                            else
                                corpus[b] = "UP";
                        else
                            if (p > livePoll.probabilities[b].p_Up)
                                corpus[b] = "UP";
                            else
                                corpus[b] = "DOWN";

                    Debug.Log(p);

                    corpuses.Add(corpus);
                }
                markov.Add(corpus);
            }
        }

        void Update()
        {
        }
    }
}
