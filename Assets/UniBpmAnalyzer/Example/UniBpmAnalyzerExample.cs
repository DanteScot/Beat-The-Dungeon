/*
UniBpmAnalyzer
Copyright (c) 2016 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;

public class UniBpmAnalyzerExample : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] targetClip;

    int count=0;

    private void Start()
    {
        foreach (AudioClip clip in targetClip)
        {
            count++;
            int bpm = UniBpmAnalyzer.AnalyzeBpm(clip);
            if (bpm < 0)
            {
                Debug.LogError("AudioClip is null.");
                return;
            }
            Debug.Log("BPM : " + bpm);
        }
    }
}
