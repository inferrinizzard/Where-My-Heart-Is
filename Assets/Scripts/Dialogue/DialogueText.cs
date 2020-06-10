using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;

public class DialogueText : MonoBehaviour
{
	public static Dictionary<string, LevelText> autumn, winter;
	public struct LevelText
	{
		public string name, fmod;
		public List<string> text;
		public List<float> timestamps;

		public LevelText(string name, string fmod, IEnumerable<string> text, IEnumerable<string> timestamps)
		{
			this.fmod = fmod;
			this.name = name;
			this.text = text.ToList();
			this.timestamps = timestamps.Select(time =>
			{
				var split = time.Split(':');
				return float.Parse(split[0]) * 60 + float.Parse(split[1]);
			}).ToList();
		}

		public override string ToString() => $"{name} @ {fmod} Level with {text.Count} items in text and {timestamps.AsString()}";
	}

	public static void Load()
	{
		(autumn, winter) =
		Func.Lambda<List<Dictionary<string, LevelText>>,
			(Dictionary<string, LevelText>, Dictionary<string, LevelText>) >
			(json => (json[0], json[1]))
			(JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>
				(Resources.Load<TextAsset>("Script").text)
				.Select(world =>
					world.Value.Select(level =>
						new LevelText(level.Key, level.Value["fmod"][0], level.Value["text"], level.Value["timestamps"]))
					.ToDictionary(kvp => kvp.name, kvp => kvp)
				).ToList());
	}
}
