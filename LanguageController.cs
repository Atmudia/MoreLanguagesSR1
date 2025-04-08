using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SRML.Console;
using Console = SRML.Console.Console;

namespace MoreLanguages
{
    internal class LanguageController
    {
        internal static void SetTranslations(MessageDirector dir)
        {
            MessageDirector.Lang cultureLang = dir.GetCultureLang();

            if (currLang != cultureLang)
            {
                string langResourceName = GetLangResourceName(cultureLang);
                try
                {
                    if (!LANGUAGE_TO_ASSEMBLY_MAP.TryGetValue(cultureLang, out Assembly value))
                        return;
                    ProcessLangFile(langResourceName, value);
                    currLang = cultureLang;
                    
                }
                catch (Exception ex)
                {
                    EntryPoint.Instance.ConsoleInstance.LogError(ex);
                    throw;
                }
            }
        }

        private static string GetLangResourceName(MessageDirector.Lang lang)
        {
            string langCode = lang.ToString().ToLower();
            return $"MoreLanguages.Lang.{langCode}.yaml"; // Adjust this for your actual namespace and resource structure
        }

        private static void ProcessLangFile(string resourceName, Assembly assembly)
        {
            // Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    // EntryPoint.Instance.ConsoleInstance.LogError($"Resource {resourceName} not found.");
                    return;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string[] lines = reader.ReadToEnd().Split('\n');
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("@import "))
                        {
                            string importedFileName = line.Replace("@import ", "").Trim().Replace("/", ".");
                            string importedResourceName = $"{assembly.GetName().Name}.Lang.{SRSingleton<GameContext>.Instance.MessageDirector.GetCurrentLanguageCode().ToLowerInvariant()}.{importedFileName}";
                            ProcessLangFile(importedResourceName, assembly);
                        }
                        else if (!string.IsNullOrWhiteSpace(line) && line.Contains(":"))
                        {
                            string bundle = line.Substring(0, line.IndexOf(':'));
                            string key = line.Substring(0, line.IndexOf(':', bundle.Length + 1))
                                .Replace($"{bundle}:", string.Empty);

                            string value = line.Replace($"{bundle}:{key}:", string.Empty)
                                .TrimStart().Trim('"')
                                .TrimEnd('"')
                                .Replace("\\n", "\n")
                                .Replace("\\\"", "\"");

                            AddTranslation(bundle.Trim('"'), key.Trim('"'), value);
                        }
                    }
                }
            }
        }

        private static void AddTranslation(string bundle, string key, string value)
        {
            if (!TRANSLATIONS.ContainsKey(bundle))
            {
                TRANSLATIONS.Add(bundle, new Dictionary<string, string>());
            }

            TRANSLATIONS[bundle][key] = value;
        }

        internal static void ResetTranslations(MessageDirector dir)
        {                
            if (currLang == dir.GetCultureLang()) return;

            foreach (string bundle in TRANSLATIONS.Keys)
            {
                TRANSLATIONS[bundle].Clear();
            }
			
            SetTranslations(dir);
        }

        private static MessageDirector.Lang? currLang;

        internal static readonly Dictionary<MessageDirector.Lang, string> LANGUAGES = new Dictionary<MessageDirector.Lang, string>
        {
            { MessageDirector.Lang.EN, "English" },
            { MessageDirector.Lang.DE, "Deutsch" },
            { MessageDirector.Lang.ES, "Español" },
            { MessageDirector.Lang.FR, "Français" },
            { MessageDirector.Lang.RU, "Pyccкий" },
            { MessageDirector.Lang.ZH, "中文" },
            { MessageDirector.Lang.JA, "日本語" },
            { MessageDirector.Lang.SV, "Svenska" },
            { MessageDirector.Lang.PT, "Português-Brasil" },
            { MessageDirector.Lang.KO, "한국어" }
        };
        internal static readonly Dictionary<MessageDirector.Lang, Assembly> LANGUAGE_TO_ASSEMBLY_MAP = new Dictionary<MessageDirector.Lang, Assembly>();
        

        internal static readonly Dictionary<string, Dictionary<string, string>> TRANSLATIONS = new Dictionary<string, Dictionary<string, string>>
        {
            { "global", new Dictionary<string, string>() },
            { "actor", new Dictionary<string, string>() },
            { "pedia", new Dictionary<string, string>() },
            { "ui", new Dictionary<string, string>() },
            { "range", new Dictionary<string, string>() },
            { "build", new Dictionary<string, string>() },
            { "mail", new Dictionary<string, string>() },
            { "keys", new Dictionary<string, string>() },
            { "achieve", new Dictionary<string, string>() },
            { "exchange", new Dictionary<string, string>() },
            { "tutorial", new Dictionary<string, string>() }
        };
    }
}
