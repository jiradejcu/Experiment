//
//  Preferences.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using System;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Jemast.LocalCache
{
    public static class Preferences
    {
        private static PreferencesContainer _preferencesContainer;

        static Preferences()
        {
            Refresh();
        }

        public static void Refresh()
        {
            _preferencesContainer = !GlobalSettings ? LoadPreferencesContainer() : new PreferencesContainer();

            // Dumb but effective way to initialize values
            _preferencesContainer.AutoCompress = AutoCompress;
            _preferencesContainer.CompressionAlgorithm = CompressionAlgorithm;
            _preferencesContainer.CompressionQualityLz4 = CompressionQualityLz4;
            _preferencesContainer.LocalCacheVersion = LocalCacheVersion;
            _preferencesContainer.EnableLogFile = EnableLogFile;
            _preferencesContainer.EnableHardLinks = EnableHardLinks;
            _preferencesContainer.DefaultStandaloneBuildTargetOption = DefaultStandaloneBuildTargetOption;
            _preferencesContainer.DefaultWebPlayerBuildTargetOption = DefaultWebPlayerBuildTargetOption;

			// Prevent hard links being enabled while the cache path is pointing to another volume
			if (Application.platform == RuntimePlatform.OSXEditor && _preferencesContainer.EnableHardLinks == true && _preferencesContainer.CachePath != null && _preferencesContainer.CachePath.StartsWith("/Volumes/")) {
				CachePath = null;
			}

			// Check cache path exists or can be created, otherwise reset
			if (_preferencesContainer.CachePath != null)
			{
				try
				{
                    if (!Directory.Exists(_preferencesContainer.CachePath)) {
					    Directory.CreateDirectory(_preferencesContainer.CachePath);
                    }
				}
				catch
				{
                    _preferencesContainer.CachePath = null;
                    SavePreferences();
				}
			}
        }

        public static int LocalCacheVersion
        {
            get
            {
                if (_preferencesContainer.LocalCacheVersion.HasValue)
                    return _preferencesContainer.LocalCacheVersion.Value;

                if (GlobalSettings)
                {
                    _preferencesContainer.LocalCacheVersion = EditorPrefs.GetInt("Jemast_LocalCache_Version", 2);
                }
                else
                {
                    _preferencesContainer.LocalCacheVersion = 2;
                    SavePreferences();
                }

                return _preferencesContainer.LocalCacheVersion.Value;
            }
            set
            {
                if (_preferencesContainer.LocalCacheVersion == value) return;
                _preferencesContainer.LocalCacheVersion = value;

                if (GlobalSettings)
                {
                    EditorPrefs.SetInt("Jemast_LocalCache_Version", value);
                }
                else
                {
                    SavePreferences();
                }
            }
        }

        public static bool EnableLogFile
        {
            get
            {
                if (_preferencesContainer.EnableLogFile.HasValue) return _preferencesContainer.EnableLogFile.Value;

                if (GlobalSettings)
                {
                    _preferencesContainer.EnableLogFile = EditorPrefs.GetBool("Jemast_LocalCache_Enable_Log_File",
                        false);
                }
                else
                {
                    _preferencesContainer.EnableLogFile = false;
                    SavePreferences();
                }

                return _preferencesContainer.EnableLogFile.Value;
            }
            set
            {
                if (_preferencesContainer.EnableLogFile == value) return;
                _preferencesContainer.EnableLogFile = value;

                if (GlobalSettings)
                {
                    EditorPrefs.SetBool("Jemast_LocalCache_Enable_Log_File", value);
                }
                else
                {
                    SavePreferences();
                }
            }
        }

        public static bool EnableHardLinks
        {
            get
            {
                if (_preferencesContainer.EnableHardLinks.HasValue) return _preferencesContainer.EnableHardLinks.Value;

                if (GlobalSettings)
                {
                    _preferencesContainer.EnableHardLinks =
                        EditorPrefs.GetBool("Jemast_LocalCache_Enable_Hard_Links",
                            false);
                }
                else
                {
                    _preferencesContainer.EnableHardLinks = false;
                    SavePreferences();
                }

                return _preferencesContainer.EnableHardLinks.Value;
            }
            set
            {
                if (_preferencesContainer.EnableHardLinks == value) return;
                _preferencesContainer.EnableHardLinks = value;

                if (GlobalSettings)
                {
                    EditorPrefs.SetBool("Jemast_LocalCache_Enable_Hard_Links", value);
                }
                else
                {
                    SavePreferences();
                }
            }
        }

        public static bool AutoCompress
        {
            get
            {
                if (_preferencesContainer.AutoCompress.HasValue) return _preferencesContainer.AutoCompress.Value;

                if (GlobalSettings)
                {
                    _preferencesContainer.AutoCompress =
                        EditorPrefs.GetBool("Jemast_LocalCache_Automatic_Compression",
                            false);
                }
                else
                {
                    _preferencesContainer.AutoCompress = false;
                    SavePreferences();
                }

                return _preferencesContainer.AutoCompress.Value;
            }
            set
            {
                if (_preferencesContainer.AutoCompress == value) return;
                _preferencesContainer.AutoCompress = value;

                if (GlobalSettings)
                {
                    EditorPrefs.SetBool("Jemast_LocalCache_Automatic_Compression", value);
                }
                else
                {
                    SavePreferences();
                }
            }
        }

        public static int CompressionAlgorithm
        {
            get
            {
                if (_preferencesContainer.CompressionAlgorithm.HasValue)
                    return _preferencesContainer.CompressionAlgorithm.Value;

                if (GlobalSettings)
                {
                    _preferencesContainer.CompressionAlgorithm =
                        EditorPrefs.GetInt("Jemast_LocalCache_Compression_Algorithm", 0);
                }
                else
                {
                    _preferencesContainer.CompressionAlgorithm = 0;
                    SavePreferences();
                }

                return _preferencesContainer.CompressionAlgorithm.Value;
            }
            set
            {
                if (_preferencesContainer.CompressionAlgorithm == value) return;
                _preferencesContainer.CompressionAlgorithm = value;

                if (GlobalSettings)
                {
                    EditorPrefs.SetInt("Jemast_LocalCache_Compression_Algorithm", value);
                }
                else
                {
                    SavePreferences();
                }
            }
        }

        public static int CompressionQualityLz4
        {
            get
            {
                if (_preferencesContainer.CompressionQualityLz4.HasValue)
                    return _preferencesContainer.CompressionQualityLz4.Value;

                if (GlobalSettings)
                {
                    _preferencesContainer.CompressionQualityLz4 =
                        EditorPrefs.GetInt("Jemast_LocalCache_Compression_Quality_LZ4", 0);
                }
                else
                {
                    _preferencesContainer.CompressionQualityLz4 = 0;
                    SavePreferences();
                }

                return _preferencesContainer.CompressionQualityLz4.Value;
            }
            set
            {
                if (_preferencesContainer.CompressionQualityLz4 == value) return;
                _preferencesContainer.CompressionQualityLz4 = value;

                if (GlobalSettings)
                {
                    EditorPrefs.SetInt("Jemast_LocalCache_Compression_Quality_LZ4", value);
                }
                else
                {
                    SavePreferences();
                }
            }
        }

        public static int DefaultStandaloneBuildTargetOption
        {
            get
            {
                if (_preferencesContainer.DefaultStandaloneBuildTargetOption.HasValue)
                    return _preferencesContainer.DefaultStandaloneBuildTargetOption.Value;

                if (GlobalSettings)
                {
                    _preferencesContainer.DefaultStandaloneBuildTargetOption =
                        EditorPrefs.GetInt("Jemast_LocalCache_DefaultStandaloneBuildTargetOption", 0);
                }
                else
                {
                    _preferencesContainer.DefaultStandaloneBuildTargetOption = 0;
                    SavePreferences();
                }

                return _preferencesContainer.DefaultStandaloneBuildTargetOption.Value;
            }
            set
            {
                if (_preferencesContainer.DefaultStandaloneBuildTargetOption == value) return;
                _preferencesContainer.DefaultStandaloneBuildTargetOption = value;

                if (GlobalSettings)
                {
                    EditorPrefs.SetInt("Jemast_LocalCache_DefaultStandaloneBuildTargetOption", value);
                }
                else
                {
                    SavePreferences();
                }
            }
        }

        public static int DefaultWebPlayerBuildTargetOption
        {
            get
            {
                if (_preferencesContainer.DefaultWebPlayerBuildTargetOption.HasValue)
                    return _preferencesContainer.DefaultWebPlayerBuildTargetOption.Value;

                if (GlobalSettings)
                {
                    _preferencesContainer.DefaultWebPlayerBuildTargetOption =
                        EditorPrefs.GetInt("Jemast_LocalCache_DefaultWebPlayerBuildTargetOption", 0);
                }
                else
                {
                    _preferencesContainer.DefaultWebPlayerBuildTargetOption = 0;
                    SavePreferences();
                }

                return _preferencesContainer.DefaultWebPlayerBuildTargetOption.Value;
            }
            set
            {
                if (_preferencesContainer.DefaultWebPlayerBuildTargetOption == value) return;
                _preferencesContainer.DefaultWebPlayerBuildTargetOption = value;

                if (GlobalSettings)
                {
                    EditorPrefs.SetInt("Jemast_LocalCache_DefaultWebPlayerBuildTargetOption", value);
                }
                else
                {
                    SavePreferences();
                }
            }
        }

        public static string CachePath
        {
            get
            {
                if (GlobalSettings)
                {
                    return Shared.ProjectPath + "LocalCache/";
                }

                return _preferencesContainer.CachePath ?? Shared.ProjectPath + "LocalCache/";
            }
            set
            {
                if (GlobalSettings) return;

                try
                {
                    string newCachePath = string.IsNullOrEmpty(value)
                        ? Shared.ProjectPath + "LocalCache/"
                        : value.TrimEnd(new[] {'/'}) + "/";

                    if (newCachePath != CachePath)
                    {
                        Shared.DirectoryCopy(CachePath, newCachePath, true);
                        Shared.DeleteDirectory(CachePath);
                    }

                    _preferencesContainer.CachePath = string.IsNullOrEmpty(value) ? null : newCachePath;
                    SavePreferences();
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                }
            }
        }

        public static bool CachePathIsAutomatic
        {
            get { return _preferencesContainer.CachePath == null; }
        }

        public static string AutomaticCachePath
        {
            get { return Shared.ProjectPath + "LocalCache/"; }
        }

        public static bool GlobalSettings
        {
            get { return !File.Exists(Shared.SettingsFilePath); }
            set
            {
                if (!value)
                {
                    SavePreferences();
                }
                else
                {
                    File.Delete(Shared.SettingsFilePath);
                }
            }
        }

        private static void SavePreferences()
        {
            var xmlSerializer = new XmlSerializer(typeof (PreferencesContainer));
            using (var writer = new StreamWriter(Shared.SettingsFilePath))
            {
                xmlSerializer.Serialize(writer, _preferencesContainer);
            }
        }

        private static PreferencesContainer LoadPreferencesContainer()
        {
            // Deserialize
            try
            {
                var serializer = new XmlSerializer(typeof (PreferencesContainer));
                using (var stream = new FileStream(Shared.SettingsFilePath, FileMode.Open))
                {
                    return serializer.Deserialize(stream) as PreferencesContainer;
                }
            }
            catch
            {
                return new PreferencesContainer();
            }
        }
    }


    [XmlRoot("PreferencesCollection")]
    public class PreferencesContainer
    {
        public bool? AutoCompress;
        public string CachePath;
        public int? CompressionAlgorithm;
        public int? CompressionQualityLz4;
        public int? DefaultStandaloneBuildTargetOption;
        public int? DefaultWebPlayerBuildTargetOption;
        public bool? EnableHardLinks;
        public bool? EnableLogFile;
        public int? LocalCacheVersion;
    }
}