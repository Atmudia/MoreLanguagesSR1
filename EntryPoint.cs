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

        private static List<Font> Fonts = new List<Font>();

        public override void PreLoad()
        {
            Instance = this;
            string codeBase = ExecAssembly.CodeBase;
            string text = Uri.UnescapeDataString(new UriBuilder(codeBase).Path);
            if (File.Exists(Path.Combine(Path.GetDirectoryName(text), "Lang")))
            {
                Activate = false;
            }

            base.HarmonyInstance.PatchAll(ExecAssembly);

            TMP_FontAsset openSansFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(x => x.name == "OpenSans SDF");
            if (openSansFont != null)
            {
                openSansFont.m_FallbackFontAssetTable.Add(TMP_FontAsset.CreateFontAsset(Resources.FindObjectsOfTypeAll<Font>().FirstOrDefault(x => x.name == "OpenSans-Semibold")));
            }

            RegisterNewTranslation(Enums.PL, "Polski");
            // RegisterNewTranslation(Enums.TR, "Türkçe");
            foreach (MessageDirector.Lang lang in LanguageController.LANGUAGES.Keys)
            {
                TranslationPatcher.AddUITranslation("l.lang_" + lang.ToString().ToLowerInvariant(), LanguageController.LANGUAGES[lang]);
            }
        }
        public override void PostLoad()
        {
            if (!Activate)
            {
                CreateErrorDialog();
            }
        }

        private void CreateErrorDialog()
        {
            GameObject errorDialog = SRSingleton<GameContext>.Instance.UITemplates.CreateErrorDialog("Please install the MoreLanguages mod correctly and do not ignore mod descriptions. I will specifically not write here how to install a mod correctly so that you will read the mod description.");
            Button okButton = errorDialog.transform.Find("MainPanel/OKButton").GetComponent<Button>();
            okButton.onClick = new Button.ButtonClickedEvent();
            okButton.onClick.AddListener(QuitGameWithError);
        }

        private void QuitGameWithError()
        {
            
            var message = "[MoreLanguages] The shutdown of the game was caused by the MoreLanguages mod because it was installed incorrectly";
            Debug.LogError(new string('=', message.Length));
            Debug.LogError(new string('=', message.Length));
            Debug.LogError(message);
            Debug.LogError(new string('=', message.Length));
            Debug.LogError(new string('=', message.Length));
            Application.Quit();
        }

        public static void RegisterNewTranslation(MessageDirector.Lang lang, string name, Font font = null)
        {
            if (SRModLoader.CurrentLoadingStep > SRModLoader.LoadingStep.PRELOAD)
            {
                throw new Exception("Can't register new language outside of the PreLoad step");
            }

            if (font != null)
            {
                Fonts.Add(font);
            }
            LanguageController.LANGUAGES.Add(lang, name);
        }
    }
}
