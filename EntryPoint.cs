using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SRML;
using SRML.SR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoreLanguages
{
    public class EntryPoint : ModEntryPoint
    {
        internal static EntryPoint Instance;

        internal static bool Activate = true;

        internal static Assembly ExecAssembly = Assembly.GetExecutingAssembly();

        private static List<Font> _fonts = new List<Font>();
        public override void PreLoad()
        {
            Instance = this;
            base.HarmonyInstance.PatchAll(ExecAssembly);
            TMP_FontAsset openSansFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(x => x.name == "OpenSans SDF");
            if (openSansFont != null)
            {
                openSansFont.m_FallbackFontAssetTable.Add(TMP_FontAsset.CreateFontAsset(Resources.FindObjectsOfTypeAll<Font>().FirstOrDefault(x => x.name == "OpenSans-Semibold")));
            }

            RegisterNewTranslation(Enums.PL, "Polski", typeof(EntryPoint).Assembly);
            RegisterNewTranslation(Enums.TR, "Türkçe", typeof(EntryPoint).Assembly);
            RegisterNewTranslation(Enums.CS, "Čeština", typeof(EntryPoint).Assembly);
            // RegisterNewTranslation(Enums.UK, "Українська", typeof(EntryPoint).Assembly);
            foreach (MessageDirector.Lang lang in LanguageController.LANGUAGES.Keys)
            {
                TranslationPatcher.AddUITranslation("l.lang_" + lang.ToString().ToLowerInvariant(), LanguageController.LANGUAGES[lang]);
            }
        }
        public static void RegisterNewTranslation(MessageDirector.Lang lang, string name, Assembly assembly, Font font = null)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
            {
                throw new InvalidOperationException("Cannot register new language outside of the PreLoad step.");
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Please provide an assembly so the mod can recognize the files and load them.");
            }

            if (font)
            {
                _fonts.Add(font);
            }
            LanguageController.LANGUAGES[lang] = name;
            LanguageController.LANGUAGE_TO_ASSEMBLY_MAP[lang] = assembly;
        }
    }
}
