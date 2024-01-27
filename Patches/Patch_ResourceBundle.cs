using System;
using System.Collections.Generic;
using HarmonyLib;
using Console = SRML.Console.Console;

namespace MoreLanguages.Patches
{
	[HarmonyPatch(typeof(ResourceBundle), "LoadFromText")]
	internal static class Patch_ResourceBundle
	{
		[HarmonyPriority(800)]
		private static void Postfix(string path, Dictionary<string, string> __result, string text)
		{
			if (!EntryPoint.Activate) 
				return;
			LanguageController.ResetTranslations(SRSingleton<GameContext>.Instance.MessageDirector);
			foreach (KeyValuePair<string, string> keyValuePair in LanguageController.TRANSLATIONS[path])
			{
				__result[keyValuePair.Key] = keyValuePair.Value;
			}
		}
	}
}
