using UnityEngine;
using System.Collections.Generic;

public class SoundPlayer : MonoBehaviour 
{
    public static SoundPlayer main { get; private set; }

	public float masterVolume = 1.0f;
	public AudioClip[] clips;

	AudioSource[] sources = null;
	int nextSource = 0;
	Dictionary<string, AudioClip> clipMap = null;
	Dictionary<string, AudioClip[]> groups = new Dictionary<string, AudioClip[]>();
	Dictionary<string, float> downTimes = new Dictionary<string, float>();

	void Awake()
	{
		int sourceCount = 12;
		sources = new AudioSource[sourceCount];
		for (int i=0; i<sourceCount; i++)
		{
			var sourceGO = new GameObject("Audio-" + i);
			sourceGO.transform.parent = transform;

			var source = sourceGO.AddComponent<AudioSource>();
			sources[i] = source;
		}

		clipMap = new Dictionary<string, AudioClip>();
		foreach(var clip in clips)
		{
			clipMap[clip.name.ToLower()] = clip;
		}
	}

    void OnEnable()
    {
        main = this;
    }

	AudioSource GetNextSource()
	{
		var source = sources[nextSource];
		nextSource++;
		if (nextSource >= sources.Length)
		{
			nextSource = 0;
		}
		return source;
	}

    public void Play(Transform t, string clipName, float volume = 1.0f, float downTime = 0.1f)
	{
        if (Time.time < 1.0f)
            return;

		var source = GetNextSource();
        if (source == null)
            return;

		volume = Mathf.Clamp(volume * masterVolume, 0.0f, 1.0f);

		clipName = clipName.ToLower();
		if (clipMap.ContainsKey(clipName) == false)
		{
			Debug.LogWarning("Can't find clip: " + clipName);
			return;
		}

		if (downTimes.ContainsKey(clipName))
		{
			float allowedTime = downTimes[clipName];
			if (Time.unscaledTime < allowedTime)
				return;
		}

        var pos = t == null ? transform.position : t.position;
        var dif = pos - transform.position;
        var dist = Mathf.Abs(dif.x) + Mathf.Abs(dif.z);
        var pan = dif.x * 0.1f;

        source.volume = Mathf.Clamp(volume * 10.0f - dist * 0.1f, 0.1f, 1.0f);
        source.panStereo = Mathf.Clamp(pan, -1.0f, 1.0f);

		source.clip = clipMap[clipName];
		source.loop = false;
		source.timeSamples = 0;
		source.Play();

		downTimes[clipName] = Time.unscaledTime + downTime;
	}

    public void PlayGroup(Transform t, string keyStart, float volume = 1.0f, float downTime = 0.1f)
	{
		keyStart = keyStart.ToLower();

		if (groups.ContainsKey(keyStart) == false)
		{
			var list = new List<AudioClip>();
			foreach (var clip in clips)
			{
				if (clip.name.ToLower().StartsWith(keyStart))
				{
					list.Add(clip);
				}
			}

			groups[keyStart] = list.ToArray();
		}

		if (groups.ContainsKey(keyStart) == false)
		{
			Debug.LogWarning("Can't find clip group: " + keyStart);
			return;
		}

		var groupClips = groups[keyStart];
		if (groupClips.Length < 1)
		{
			Debug.LogWarning("Empty group: " + keyStart);
			return;
		}

		var selectClip = groupClips[Random.Range(0, groupClips.Length)];
		Play(t, selectClip.name, volume);
	}

    public void Stop(string clipName)
    {
        foreach (var source in sources)
        {
            if (source == null || source.clip == null)
                continue;
            if (source.clip.name.ToLower().StartsWith(clipName.ToLower()))
            {
                source.Stop();
            }
        }
    }

    public void StopAll()
    {
        foreach (var source in sources)
        {
            source.Stop();
        }
    }

    public void Mute(bool mute)
    {
        masterVolume = mute ? 0.0f : 1.0f;
    }

    public bool IsMuted()
    {
        return masterVolume == 0.0f;
    }
}
