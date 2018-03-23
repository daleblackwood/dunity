using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicPlayer))]
public class MusicPlayerEditor : Editor
{
	override public void OnInspectorGUI()
	{
		MusicPlayer player = (MusicPlayer)target;

		bool hasClip = player.source != null && player.source.clip != null;

		string clipName = hasClip ? player.source.clip.name : "none";
		EditorGUILayout.LabelField("Playing", clipName);
		if (player.source != null)
		{
			EditorGUILayout.Slider(player.source.clip.name, player.source.time, 0.0f, player.source.clip.length);
			if (player.source.isPlaying && GUILayout.Button("Stop Music"))
				player.Stop();
		}

		EditorGUILayout.Space();

        DrawDefaultInspector();

		/*int clipCount = EditorGUILayout.IntField("Clips", player.clips.Count);
		while (clipCount < player.clips.Count)
		{
			player.clips.RemoveAt(player.clips.Count-1);
		}
		while (clipCount > player.clips.Count)
		{
			player.clips.Add(new MusicClip());
		}

		int index = 0;
		foreach (var clip in player.clips)
		{
			clipName = (++index).ToString() + ". ";
			clipName += clip.clip == null ? "(empty)" : clip.clip.name;

			EditorGUILayout.BeginHorizontal();


			clip.clip = (AudioClip)EditorGUILayout.ObjectField(clip.clip, typeof(AudioClip), false);
			clip.startLoop = EditorGUILayout.FloatField(clip.startLoop);
			float endLoop = EditorGUILayout.FloatField(clip.endLoop);
			if (endLoop < clip.startLoop)
				endLoop = clip.startLoop;
			clip.endLoop = endLoop;

			EditorGUILayout.EndHorizontal();
		}*/

	}
}
