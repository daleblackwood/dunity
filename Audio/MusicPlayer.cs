using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MusicClip
{
	public AudioClip clip = null;
	public float startLoop = 0;
	public float endLoop = 0;
    public AudioClip[] slaves;
}

public class MusicPlayer : MonoBehaviour 
{
    public static MusicPlayer main { get; private set; }

	public List<MusicClip> clips = new List<MusicClip>();

	public AudioSource source { get; private set; }
    List<AudioSource> slaveSources = new List<AudioSource>();

	float stopTime = 0.0f;
    MusicClip prevClip = null;

    void OnEnable()
    {
        main = this;
    }

	void Update() 
	{
		if (source == null || source.clip == null)
			return;

		MusicClip activeClip = null;
		foreach (var clip in clips)
		{
			if (clip.clip != source.clip)
				continue;

			activeClip = clip;
			break;
		}

		if (activeClip == null)
		{
			return;
		}

        if (activeClip.clip.loadState == AudioDataLoadState.Loaded)
        {
            float secondsLeft = activeClip.endLoop - source.time;
            if (secondsLeft <= 0.0f)
            {
                float timeTo = activeClip.startLoop + secondsLeft;
                if (float.IsNaN(timeTo))
                {
                    timeTo = activeClip.startLoop;
                }
                timeTo = Mathf.Clamp(timeTo, 0.0f, activeClip.clip.length);
                source.time = timeTo;
            }
        }

        if (activeClip != prevClip)
        {
        }

		if (Camera.main != null)
		{
			transform.position = Camera.main.transform.position;
		}
	}

	public void Stop()
	{
        if (source == null)
            return;
        
		stopTime = source.time;
		source.Stop();
	}

	public void Resume()
	{
        if (source == null)
            return;
		source.Play();
		source.time = stopTime;
	}

    public void Play(string clipName, float startTime = -1.0f)
    {
		if (clipName == "none" && source != null)
		{
			source.Stop();
		}

		if (source != null && source.clip != null && source.clip.name.ToLower() == clipName.ToLower())
		{
            if (startTime >= 0.0f)
			{
                source.time = startTime;
			}
			return;
		}

		MusicClip clip = null;
		foreach (var c in clips)
		{
			if (c.clip.name.ToLower() == clipName.ToLower())
			{
				clip = c;
				break;
			}
		}

		if (clip == null)
		{
			Debug.LogWarning("Couldn't find music " + clipName);
			return;
		}

		if (source == null)
		{
			source = GetComponent<AudioSource>();
			if (source == null)
				source = gameObject.AddComponent<AudioSource>();
			source.loop = true;
		}

        source = GetSource();

		source.clip = clip.clip;
        source.time = startTime >= 0.0f ? startTime : 0.0f;
		source.Play();
	}

    AudioSource GetSource()
    {
        var source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        return source;
    }

    public void Replace(string newClipName, float fade = 0.0f)
    {
        var time = source.time;
        Play(newClipName, time);
    }

    public bool IsPlaying(string clipName)
    {
        return source.clip.name.ToLower().StartsWith(clipName.ToLower());
    }

    public void Mute(bool mute)
    {
        GetSource().mute = mute;
    }

    public bool IsMuted()
    {
        return GetSource().mute;
    }
}
