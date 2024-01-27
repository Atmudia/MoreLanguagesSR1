using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace MoreLanguages.Patches
{
	[HarmonyPatch(typeof(FabricateGadgetUI), "Awake")] 
	internal class Patch_FabricateGadgetUI
	{
		private static void Postfix(FabricateGadgetUI __instance)
		{
			foreach (var obj in __instance.purchaseUI.gameObject.transform.Find("MainPanel/MainBody/SelectionPanel/Content/ButtonList/AvailButtonList"))
			{
				Transform transform = (Transform)obj;
				UnityEngine.Object.DestroyImmediate(transform.transform.Find("Content/Count").GetComponent<CustomLangSupport>());
				TMP_Text component = transform.transform.Find("Content/Count").GetComponent<TMP_Text>();
				component.enableWordWrapping = false;
				component.fontSizeMax = 40f;
				component.fontSizeMin = 10f;
				component.margin = new Vector4(0f, 0f, 0f, 0f);
				component.enableAutoSizing = false;
			}
		}
	}
}
