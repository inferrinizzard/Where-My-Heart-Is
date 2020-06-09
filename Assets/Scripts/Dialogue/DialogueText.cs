using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;

public class DialogueText : MonoBehaviour
{
	public static Dictionary<string, LevelText> autumn, winter, spring;
	public struct LevelText
	{
		public string name;
		public List<string> preface, triggers;

		public LevelText(string name, IEnumerable<string> preface, IEnumerable<string> triggers)
		{
			this.name = name;
			this.preface = preface.ToList();
			this.triggers = triggers.ToList();
		}

		public override string ToString() => $"{name} Level with {preface.Count} items in preface and {triggers.Count} triggers";
	}

	public static void Load()
	{
		(autumn, winter, spring) =
		Func.Lambda<List<Dictionary<string, LevelText>>,
			(Dictionary<string, LevelText>, Dictionary<string, LevelText>, Dictionary<string, LevelText>) >
			(json => (json[0], json[1], json[2]))
			(JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>
				(Resources.Load<TextAsset>("format").text)
				.Select(world =>
					world.Value.Select(level =>
						new LevelText(level.Key, level.Value["preface"], level.Value["triggers"]))
					.ToDictionary(kvp => kvp.name, kvp => kvp)
				).ToList());
	}
}
