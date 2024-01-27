using System;
using HarmonyLib;
using TMPro;

namespace MoreLanguages.Patches
{
	[HarmonyPatch(typeof(XlateText), "Awake")]
	internal class Patch_XlateText
	{
		private static void Postfix(XlateText __instance)
		{
			if (__instance.GetComponent<TMP_Text>() == null)
				return;
			if (__instance.GetComponent<CustomLangSupport>() == null)
			{
				__instance.gameObject.AddComponent<CustomLangSupport>();
			}
		}
	}
}
