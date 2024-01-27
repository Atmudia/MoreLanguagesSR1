using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                string langFilePath = GetLangFilePath(cultureLang);
                try
                {   
                    ProcessLangFile(langFilePath);
                    currLang = cultureLang;
                    
                }
                catch (Exception ex)
                {
                    EntryPoint.Instance.ConsoleInstance.LogError(ex.Source);
                    throw;
                }
            }
        }

        private static string GetLangFilePath(MessageDirector.Lang lang)
        {
            Assembly execAssembly = EntryPoint.ExecAssembly;
            string langCode = lang.ToString().ToLower();
            string codeBase = execAssembly.CodeBase;
            UriBuilder uriBuilder = new UriBuilder(codeBase);
            string assemblyPath = Uri.UnescapeDataString(uriBuilder.Path);
            return Path.Combine(Path.GetDirectoryName(assemblyPath), "Lang\\" + langCode + ".yaml");
        }

        private static void ProcessLangFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;
            StreamReader streamReader = new StreamReader(filePath);
            string[] lines = streamReader.ReadToEnd().Split('\n');
            foreach (string line in lines)
            {
                if (line.StartsWith("@import "))
                {
                    string importedFilePath = line.Replace("@import ", "").Trim().Replace("/", "\\");
                    FileInfo fileInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(filePath), SRSingleton<GameContext>.Instance.MessageDirector.GetCurrentLanguageCode().ToLowerInvariant() + "\\" + importedFilePath));
                    if (fileInfo.Exists)
                    {
                        ProcessLangFile(fileInfo.FullName);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line) && line.Contains(":"))
                {
    
                    string bundle = line.Substring(0, line.IndexOf(':'));
					
                    string key = line.Substring(0, line.IndexOf(':', bundle.Length+1))
                        .Replace(string.Format("{0}:", bundle), string.Empty);
					
                    string value = line.Replace(string.Format("{0}:{1}:", bundle, key), string.Empty);

                    AddTranslation(bundle.Trim('"'), key.Trim('"'), value.TrimStart()
                        .TrimStart('"')
                        .TrimEnd('"')
                        .Replace("\\n", "\n")
                        .Replace("\\\"", "\""));
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
            {
                MessageDirector.Lang.EN,
                "English"
            },
            {
                MessageDirector.Lang.DE,
                "Deutsch"
            },
            {
                MessageDirector.Lang.ES,
                "Español"
            },
            {
                MessageDirector.Lang.FR,
                "Français"
            },
            {
                MessageDirector.Lang.RU,
                "Pyccкий"
            },
            {
                MessageDirector.Lang.ZH,
                "中文"
            },
            {
                MessageDirector.Lang.JA,
                "日本語"
            },
            {
                MessageDirector.Lang.SV,
                "Svenska"
            },
            {
                MessageDirector.Lang.PT,
                "Português-Brasil"
            },
            {
                MessageDirector.Lang.KO,
                "한국어"
            }
        };
        internal static readonly Dictionary<string, Dictionary<string, string>> TRANSLATIONS = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "global",
                new Dictionary<string, string>()
            },
            {
                "actor",
                new Dictionary<string, string>()
            },
            {
                "pedia",
                new Dictionary<string, string>()
            },
            {
                "ui",
                new Dictionary<string, string>()
            },
            {
                "range",
                new Dictionary<string, string>()
            },
            {
                "build",
                new Dictionary<string, string>()
            },
            {
                "mail",
                new Dictionary<string, string>()
            },
            {
                "keys",
                new Dictionary<string, string>()
            },
            {
                "achieve",
                new Dictionary<string, string>()
            },
            {
                "exchange",
                new Dictionary<string, string>()
            },
            {
                "tutorial",
                new Dictionary<string, string>()
            }
        };
    }
}
