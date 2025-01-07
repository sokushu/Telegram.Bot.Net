﻿//  <Telegram.Bot.Framework>
//  Copyright (C) <2022 - 2025>  <Azumo-Lab> see <https://github.com/Azumo-Lab/Azumo.Telegram.Bot.Framework>
//
//  This file is part of <Telegram.Bot.Framework>: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Telegram.Bot.Framework.Properties;

namespace Telegram.Bot.Framework.InternalCore.StaticI18N
{
    /// <summary>
    /// 
    /// </summary>
    internal static class LanguageManager
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<string, LanguageConfig> LanguageDictionary =
#if NET8_0_OR_GREATER
            [];
#else
            new Dictionary<string, LanguageConfig>();
#endif

        /// <summary>
        /// 
        /// </summary>
        static LanguageManager()
        {
            var DirPath = new DirectoryInfo("i18N");
            if (!DirPath.Exists)
                DirPath.Create();

            var stringBuilder = new StringBuilder();
            _ = stringBuilder.AppendLine("Please put the language files in this folder");
            _ = stringBuilder.AppendLine("请把语言文件放在这个文件夹中");
            _ = stringBuilder.AppendLine("言語ファイルをこのフォルダに入れてください");
            File.WriteAllText(Path.Combine(DirPath.FullName, "README.TXT"), stringBuilder.ToString());

            var json_List = new List<string>
            {
                Encoding.UTF8.GetString(Resources.en_US),
                Encoding.UTF8.GetString(Resources.zh_CN),
            };
            json_List.AddRange(DirPath.GetFiles("*.json", SearchOption.AllDirectories).Select(x =>
            {
                using (var reader = new StreamReader(x.OpenRead()))
                {
                    return reader.ReadToEnd();
                }
            }));

            foreach (var jsonText in json_List)
            {
                try
                {
                    var config = JsonSerializer.Deserialize<LanguageConfig>(jsonText);
                    if (config == null)
                        continue;

                    if (string.IsNullOrEmpty(config.Name))
                        continue;

                    var addFlag = LanguageDictionary.TryAdd(config.Name, config);
                    if (!addFlag)
                        LanguageDictionary[config.Name] = config;

                    if (LanguageDictionary.Count == 1)
                        Current = config;
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static LanguageConfig? Current;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string TryGet(Func<LanguageConfig, string?> func) =>
            Current != null ? func.Invoke(Current) ?? string.Empty : string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void Change(string key)
        {
            if (LanguageDictionary.TryGetValue(key, out var outObj))
                Current = outObj;
        }
    }
}
