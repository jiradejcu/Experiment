//
//  JLocalCacheWindow.cs
//  Fast Platform Switch
//
//  Copyright (c) 2013-2014 jemast software.
//

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using JSFTCacheManager = Jemast.LocalCache.CacheManager;
using JSFTPreferences = Jemast.LocalCache.Preferences;
using JSFTShared = Jemast.LocalCache.Shared;

public class JLocalCacheWindow : EditorWindow
{
    private static readonly string[] BuildTargetOptions =
    {
        "Web Player",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
        "WebGL",
#endif
        "PC, Mac and Linux Standalone",
        "iOS",
        "Android",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
        "Blackberry",
        "Windows Store Apps",
        "Windows Phone 8",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
		"Google Native Client",
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		"Flash Player",
#endif
        "PS3",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
        "PS4",
        "PS Vita",
        "Playstation®Mobile",
#endif
        "Xbox 360",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
        "Xbox One"
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		"Wii"
#endif
    };

    private static readonly string[][] BuildTargetWithSubtargetsOptions =
    {
        new[] {"Web Player"},
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
        new [] {"WebGL"},
#endif
        new[] {"PC, Mac and Linux Standalone"},
        new[] {"iOS"},
        new[]
        {
            "Android (GENERIC)",
            "Android (DXT)",
            "Android (PVRTC)",
            "Android (ATC)",
            "Android (ETC1)",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            "Android (ETC2)",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            "Android (ASTC)"
#endif
        },
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
        new[]
        {
            "Blackberry (GENERIC)",
            "Blackberry (PVRTC)",
            "Blackberry (ATC)",
            "Blackberry (ETC1)"
        },
        new[] {"Windows Store Apps"},
        new[] {"Windows Phone 8"},
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
		new string[] { "Google Native Client" },
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		new string[] { "Flash Player" },
#endif
        new[] {"PS3"},
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
        new[] {"PS4"},
        new[] {"PS Vita"},
        new[] {"Playstation®Mobile"},
#endif
        new[] {"Xbox 360"},
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
        new[] {"Xbox One"}
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		new string[] { "Wii" }
#endif
    };

    private static readonly string[] AndroidTextureCompressionOptions =
    {
        "Don't override",
        "DXT (Tegra)",
        "PVRTC (PowerVR)",
        "ATC (Adreno)",
        "ETC1 or RGBA16 (GLES 2.0)",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
        "ETC2 (GLES 3.0)",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
        "ASTC"
#endif
    };

#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
    private static readonly string[] BlackberryTextureCompressionOptions =
    {
        "Don't override",
        "PVRTC (PowerVR)",
        "ATC (Adreno)",
        "ETC1 or RGBA16 (GLES 2.0)"
    };
#endif

    private static readonly string[] StandaloneBuildTargetOptions =
    {
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
        "Windows 32 bits (x86)",
        "Windows 64 bits (x86_64)",
        "OS X 32 bits (x86)",
        "OS X 64 bits (x86_64)",
        "OS X 32+64 bits (Universal)",
        "Linux 32 bits (x86)",
        "Linux 64 bits (x86_64)",
        "Linux 32+64 bits (Universal)"
#elif !UNITY_3_4 && !UNITY_3_5
        "Windows 32 bits (x86)",
        "Windows 64 bits (x86_64)",
        "OS X 32 bits (x86)",
        "Linux 32 bits (x86)",
        "Linux 64 bits (x86_64)",
        "Linux 32+64 bits (Universal)"
#else
        "Windows 32 bits (x86)",
        "Windows 64 bits (x86_64)",
        "OS X 32 bits (x86)"
#endif
    };

    private static readonly string[] WebPlayerBuildTargetOptions =
    {
        "Standard",
        "Streamed"
    };

    private Texture2D[] _buildTargetTextures;

    private Texture2D[] _androidSubtargetTextures;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
    private Texture2D[] _blackberrySubtargetTextures;
#endif

    private Texture2D _selectedBuildTargetTexture;
    private Texture2D _trashBuildTargetTexture;
    private Texture2D _zipBuildTargetTexture;
    private Texture2D _unzipBuildTargetTexture;

    private Texture2D _shadedBackgroundTexture;

    private JSFTShared.CacheTarget? _wantedCacheTarget;
    private JSFTShared.CacheSubtarget? _wantedCacheSubtarget;

    private int _toolbarIndex;
    private readonly string[] _toolbarOptions = {"PLATFORM", "STATUS", "SETTINGS"};
    private Vector2 _scrollPos1 = Vector2.zero;
    private Vector2 _scrollPos2 = Vector2.zero;
    private Vector2 _scrollPos3 = Vector2.zero;
    private Rect _scrollViewRect;
    private Rect[] _listViewRects;

    private struct CacheData
    {
        public bool CacheStatus;
        public bool CompressionStatus;
        public DateTime Date;
        public string Size;
    }

    private CacheData[][] _cacheData;
    private long _totalCacheSize;

    private bool _shouldSwitch;

    // Styles
    private readonly GUIStyle _listFontStyle = new GUIStyle();

    private readonly GUIStyle _imageStyle = new GUIStyle();
    private readonly GUIStyle _areaStyle = new GUIStyle();
    private readonly GUIStyle _areaStyleAlt = new GUIStyle();
    private readonly GUIStyle _areaStyleActive = new GUIStyle();

    private readonly GUIStyle _statusFontStyle = new GUIStyle();
    private readonly GUIStyle _statusFontCachedStyle = new GUIStyle();
    private readonly GUIStyle _statusFontCompressedStyle = new GUIStyle();
    private readonly  GUIStyle _smallTextGuiStyle = new GUIStyle();

    private GUIStyle _headerGuiStyle;
    private GUIStyle _toolbarGuiStyle;
    private GUIStyle _pipelineButtonGuiStyle;

    [MenuItem("Window/Fast Platform Switch")]
    public static void ShowWindow()
    {
        var w = GetWindow(typeof (JLocalCacheWindow), false, "Platform") as JLocalCacheWindow;
        if (w != null) w.Show();
    }

    private void OnEnable()
    {
        // Setup current wanted target
        _wantedCacheTarget = JSFTCacheManager.CurrentCacheTarget;
        _wantedCacheSubtarget = JSFTCacheManager.CurrentCacheSubtarget;

        EditorApplication.update += EditorUpdate;
    }

    private void OnGUI()
    {
        SetupGuiStyles(GUI.skin);

        bool isDirty = false;

        if ((_buildTargetTextures == null || _areaStyle.normal.background == null) && !JSFTCacheManager.PlatformRefreshInProgress)
        {
            SetupWindow();
            isDirty = true;
        }

        if (_buildTargetTextures == null || _buildTargetTextures.Length == 0)
        {
            ForceCleanup();
            isDirty = true;
        }
        else
        {
            _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, _toolbarOptions, _toolbarGuiStyle);

            GUILayout.Space(10);

            if (_toolbarIndex == 0)
            {
                _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1, GUILayout.ExpandHeight(true),
                    GUILayout.MaxHeight(40f*BuildTargetOptions.Length));
                for (int i = 0; i < BuildTargetOptions.Length; i++)
                {
                    if (_wantedCacheTarget.HasValue && i == (int) _wantedCacheTarget.Value)
                        EditorGUILayout.BeginHorizontal(_areaStyleActive, GUILayout.Height(40),
                            GUILayout.ExpandWidth(true));
                    else
                        EditorGUILayout.BeginHorizontal((i%2 == 0) ? _areaStyle : _areaStyleAlt, GUILayout.Height(40),
                            GUILayout.ExpandWidth(true));

                    GUILayout.Space(5);

                    // Image
                    EditorGUILayout.BeginVertical(GUILayout.Height(40));
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(_buildTargetTextures[i], _imageStyle);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(5);

                    // Text
                    EditorGUILayout.BeginVertical(GUILayout.Height(40));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(BuildTargetOptions[i], _listFontStyle);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    // Selected platform
                    if (i == (int) JSFTCacheManager.CurrentCacheTarget)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.Height(40));
                        GUILayout.FlexibleSpace();
                        GUILayout.Box(_selectedBuildTargetTexture, _imageStyle);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndVertical();

                        GUILayout.Space(5);
                    }

                    EditorGUILayout.EndHorizontal();

                    if (Event.current.type == EventType.Repaint)
                    {
                        _listViewRects[i] = GUILayoutUtility.GetLastRect();
                    }
                }
                EditorGUILayout.EndScrollView();

                if (Event.current.type == EventType.Repaint)
                {
                    _scrollViewRect = GUILayoutUtility.GetLastRect();
                    for (int i = 0; i < _listViewRects.Length; i++)
                        _listViewRects[i].y += _scrollViewRect.y;
                }

                if (_wantedCacheTarget == JSFTShared.CacheTarget.Android)
                {
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Texture Compression");
                    GUILayout.FlexibleSpace();

                    int androidCompressionOption =
                        CacheSubtargetToAndroidCompressionOption(_wantedCacheSubtarget ??
                                                                 JSFTCacheManager.CurrentCacheSubtarget);
                    androidCompressionOption = EditorGUILayout.Popup(androidCompressionOption,
                        AndroidTextureCompressionOptions);
                    _wantedCacheSubtarget = AndroidCompressionOptionToCacheSubtarget(androidCompressionOption);
                    GUILayout.EndHorizontal();
                }
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                else if (_wantedCacheTarget == JSFTShared.CacheTarget.BlackBerry)
                {
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Texture Compression");
                    GUILayout.FlexibleSpace();

                    int blackberryCompressionOption =
                        CacheSubtargetToBlackBerryCompressionOption(_wantedCacheSubtarget ??
                                                                    JSFTCacheManager.CurrentCacheSubtarget);
                    blackberryCompressionOption = EditorGUILayout.Popup(blackberryCompressionOption,
                        BlackberryTextureCompressionOptions);
                    _wantedCacheSubtarget = BlackBerryCompressionOptionToCacheSubtarget(blackberryCompressionOption);
                    GUILayout.EndHorizontal();
                }
#endif
                else if (_wantedCacheTarget == JSFTShared.CacheTarget.Standalone)
                {
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Build Target");
                    GUILayout.FlexibleSpace();
                    JSFTPreferences.DefaultStandaloneBuildTargetOption =
                        EditorGUILayout.Popup(JSFTPreferences.DefaultStandaloneBuildTargetOption,
                            StandaloneBuildTargetOptions);
                    GUILayout.EndHorizontal();
                }
                else if (_wantedCacheTarget == JSFTShared.CacheTarget.WebPlayer)
                {
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Build Target");
                    GUILayout.FlexibleSpace();
                    JSFTPreferences.DefaultWebPlayerBuildTargetOption =
                        EditorGUILayout.Popup(JSFTPreferences.DefaultWebPlayerBuildTargetOption,
                            WebPlayerBuildTargetOptions);
                    GUILayout.EndHorizontal();
                }
                else
                {
                    _wantedCacheSubtarget = null;
                }

                GUILayout.Space(10);

                GUI.enabled = !JSFTCacheManager.BackgroundCacheCompressionInProgress &&
                              !JSFTCacheManager.PlatformRefreshInProgress &&
                              !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling &&
                              !EditorApplication.isUpdating;
                if (GUILayout.Button("SWITCH PLATFORM", _pipelineButtonGuiStyle))
                {
                    _shouldSwitch = true; //PerformSwitch();
                }
                GUI.enabled = true;

                GUILayout.Space(10);

                if (JSFTCacheManager.BackgroundCacheCompressionInProgress)
                {
                    GUILayout.Label("Please wait for background compression to end...");
                    GUILayout.Space(10);
                }

                if (JSFTCacheManager.PlatformRefreshInProgress)
                {
                    GUILayout.Label("Please wait for platform refresh to end...");
                    GUILayout.Space(10);
                }
                else if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                {
                    GUILayout.Label("Please wait for script compilation to end...");
                    GUILayout.Space(10);
                }
                else if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    GUILayout.Label("Cannot switch platform during play mode...");
                    GUILayout.Space(10);
                }


                // Handle list view events at the end
                if (!JSFTCacheManager.PlatformRefreshInProgress && !EditorApplication.isPlayingOrWillChangePlaymode &&
                    !EditorApplication.isCompiling && !EditorApplication.isUpdating)
                {
                    Vector2 mousePosition = Event.current.mousePosition;
                    if (Event.current.type == EventType.MouseDown && Event.current.clickCount > 0 &&
                        Event.current.button == 0 && _scrollViewRect.Contains(mousePosition))
                    {
                        mousePosition.y += _scrollPos1.y;
                        for (int i = 0; i < _listViewRects.Length; i++)
                        {
                            if (!_listViewRects[i].Contains(mousePosition)) continue;

                            _wantedCacheTarget = (JSFTShared.CacheTarget) i;
                            _wantedCacheSubtarget = null;
                            isDirty = true;

                            if (Event.current.clickCount == 2 &&
                                _wantedCacheTarget != JSFTCacheManager.CurrentCacheTarget &&
                                !JSFTCacheManager.BackgroundCacheCompressionInProgress)
                            {
                                _shouldSwitch = true;
                            }

                            break;
                        }
                    }
                }
            }
            else if (_toolbarIndex == 1)
            {
                int totalOptionsLength = 0;
                for (int i = 0; i < BuildTargetWithSubtargetsOptions.Length; i++)
                    totalOptionsLength += BuildTargetWithSubtargetsOptions[i].Length;

                bool hasCacheData = _cacheData != null;
                bool cacheDataIsInvalid = false;
                if (!hasCacheData)
                {
                    _cacheData = new CacheData[BuildTargetWithSubtargetsOptions.Length][];
                    _totalCacheSize = 0;
                }

                _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2, GUILayout.ExpandHeight(true),
                    GUILayout.MaxHeight(40f*totalOptionsLength));
                bool altStyle = false;
                for (int i = 0; i < BuildTargetWithSubtargetsOptions.Length; i++)
                {
                    if (!hasCacheData)
                        _cacheData[i] = new CacheData[BuildTargetWithSubtargetsOptions[i].Length];

                    for (int j = 0; j < BuildTargetWithSubtargetsOptions[i].Length; j++)
                    {
                        var cacheTarget = (JSFTShared.CacheTarget) i;
                        JSFTShared.CacheSubtarget? cacheSubtarget = null;
                        if (cacheTarget == JSFTShared.CacheTarget.Android)
                            cacheSubtarget = (JSFTShared.CacheSubtarget) j + (int) JSFTShared.CacheSubtarget.Android_First + 1;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                        else if (cacheTarget == JSFTShared.CacheTarget.BlackBerry)
                            cacheSubtarget = (JSFTShared.CacheSubtarget) j + (int) JSFTShared.CacheSubtarget.BlackBerry_First +
                                             1;
#endif

                        // Cacheception
                        if (!hasCacheData)
                        {
                            _cacheData[i][j].CacheStatus = JSFTCacheManager.GetCacheStatus(cacheTarget, cacheSubtarget);
                            _cacheData[i][j].CompressionStatus = JSFTCacheManager.GetCacheCompression(cacheTarget,
                                cacheSubtarget);
                        }

                        EditorGUILayout.BeginHorizontal(altStyle == false ? _areaStyle : _areaStyleAlt,
                            GUILayout.Height(40), GUILayout.ExpandWidth(true));
                        altStyle = !altStyle;

                        GUILayout.Space(5);

                        // Image
                        EditorGUILayout.BeginVertical(GUILayout.Height(40));
                        GUILayout.FlexibleSpace();

                        if (cacheTarget == JSFTShared.CacheTarget.Android)
                            GUILayout.Box(_androidSubtargetTextures[j], _imageStyle);
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
                        else if (cacheTarget == JSFTShared.CacheTarget.BlackBerry)
                            GUILayout.Box(_blackberrySubtargetTextures[j], _imageStyle);
#endif
                        else
                            GUILayout.Box(_buildTargetTextures[i], _imageStyle);

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndVertical();

                        GUILayout.Space(5);

                        // Text
                        EditorGUILayout.BeginVertical(GUILayout.Height(40));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(_cacheData[i][j].CacheStatus ? "Cached" : "Not cached",
                            _cacheData[i][j].CacheStatus ? _statusFontCachedStyle : _statusFontStyle);
                        GUILayout.Label(
                            _cacheData[i][j].CacheStatus
                                ? (_cacheData[i][j].CompressionStatus ? "Compressed" : "Uncompressed")
                                : "-",
                            _cacheData[i][j].CompressionStatus ? _statusFontCompressedStyle : _statusFontStyle);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndVertical();

                        GUILayout.FlexibleSpace();

                        if (_cacheData[i][j].CacheStatus)
                        {
                            if (!hasCacheData)
                            {
                                _cacheData[i][j].Date = JSFTCacheManager.GetCacheDate(cacheTarget, cacheSubtarget);
                                long cacheSize = JSFTCacheManager.GetCacheSize(cacheTarget, cacheSubtarget);
                                _totalCacheSize += cacheSize;

                                if (cacheSize >= 1073741824)
                                    _cacheData[i][j].Size = (cacheSize*9.31322575*1E-10).ToString("F2") + " GB";
                                if (cacheSize >= 1048576)
                                    _cacheData[i][j].Size = (cacheSize*9.53674316*1E-7).ToString("F2") + " MB";
                                else
                                    _cacheData[i][j].Size = (cacheSize*0.0009765625).ToString("F2") + " kB";
                            }

                            EditorGUILayout.BeginVertical(GUILayout.Height(40));
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(_cacheData[i][j].Date.ToString("d"), _statusFontStyle);
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(_cacheData[i][j].Date.ToString("t"), _statusFontStyle);
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndVertical();

                            GUILayout.Space(5);

                            EditorGUILayout.BeginVertical(GUILayout.Height(40));
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(_cacheData[i][j].Size, _statusFontStyle);
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndVertical();

                            GUILayout.Space(5);

                            if (
                                !(JSFTPreferences.EnableHardLinks && cacheTarget == JSFTCacheManager.CurrentCacheTarget &&
                                  cacheSubtarget == JSFTCacheManager.CurrentCacheSubtarget))
                            {
                                EditorGUILayout.BeginVertical(GUILayout.Height(40));
                                GUILayout.FlexibleSpace();
                                GUI.enabled = !JSFTCacheManager.BackgroundCacheCompressionInProgress &&
                                              !JSFTCacheManager.PlatformRefreshInProgress &&
                                              !EditorApplication.isPlayingOrWillChangePlaymode &&
                                              !EditorApplication.isCompiling && !EditorApplication.isUpdating;
                                if (_cacheData[i][j].CompressionStatus)
                                {
                                    if (GUILayout.Button(_unzipBuildTargetTexture))
                                    {
                                        JSFTCacheManager.DecompressCache(cacheTarget, cacheSubtarget);
                                        cacheDataIsInvalid = true;
                                        isDirty = true;
                                    }
                                }
                                else
                                {
                                    if (GUILayout.Button(_zipBuildTargetTexture))
                                    {
                                        JSFTCacheManager.CompressCache(cacheTarget, cacheSubtarget);
                                        cacheDataIsInvalid = true;
                                        isDirty = true;
                                    }
                                }
                                GUI.enabled = true;
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndVertical();

                                GUILayout.Space(5);

                                EditorGUILayout.BeginVertical(GUILayout.Height(40));
                                GUILayout.FlexibleSpace();
                                GUI.enabled = !JSFTCacheManager.BackgroundCacheCompressionInProgress &&
                                              !JSFTCacheManager.PlatformRefreshInProgress &&
                                              !EditorApplication.isPlayingOrWillChangePlaymode &&
                                              !EditorApplication.isCompiling && !EditorApplication.isUpdating;
                                if (GUILayout.Button(_trashBuildTargetTexture))
                                {
                                    JSFTCacheManager.ClearCache(cacheTarget, cacheSubtarget);
                                    cacheDataIsInvalid = true;
                                    isDirty = true;
                                }
                                GUI.enabled = true;
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndVertical();

                                GUILayout.Space(5);
                            }
                            else
                            {
                                GUILayout.Space(110);
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        if (Event.current.type == EventType.Repaint)
                        {
                            _listViewRects[i] = GUILayoutUtility.GetLastRect();
                        }
                    }
                }
                EditorGUILayout.EndScrollView();

                GUILayout.Space(10);
                if (_totalCacheSize >= 1073741824)
                    GUILayout.Label("Total cache size: " + (_totalCacheSize*9.31322575*1E-10).ToString("F2") + " GB");
                else if (_totalCacheSize >= 1048576)
                    GUILayout.Label("Total cache size: " + (_totalCacheSize*9.53674316*1E-7).ToString("F2") + " MB");
                else
                    GUILayout.Label("Total cache size: " + (_totalCacheSize*0.0009765625).ToString("F2") + " kB");
                GUILayout.Space(10);

                if (cacheDataIsInvalid)
                    _cacheData = null;
            }
            else if (_toolbarIndex == 2)
            {
                _scrollPos3 = EditorGUILayout.BeginScrollView(_scrollPos3, GUILayout.ExpandHeight(true));

                GUI.enabled = !JSFTCacheManager.BackgroundCacheCompressionInProgress &&
                              !JSFTCacheManager.PlatformRefreshInProgress &&
                              !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling &&
                              !EditorApplication.isUpdating;

                EditorGUILayout.BeginHorizontal();
                if (JSFTPreferences.GlobalSettings)
                {
                    GUILayout.Label("Using system-wide settings", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Use project-wide settings"))
                    {
                        JSFTPreferences.GlobalSettings = false;
                        JSFTPreferences.Refresh();
                        JSFTCacheManager.HasCheckedHardLinkStatus = false;
                        JSFTCacheManager.ShouldPerformCleanup = true;
                        isDirty = true;
                    }
                }
                else
                {
                    GUILayout.Label("Using project-wide settings", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Use system-wide settings"))
                    {
                        if (!JSFTPreferences.CachePathIsAutomatic)
                        {
                            EditorUtility.DisplayDialog("Manual Cache Path Enabled",
                                "You need to reset cache path before re-enabling system-wide settings.",
                                "OK");
                        }
                        else
                        {
                            JSFTPreferences.GlobalSettings = true;
                            JSFTPreferences.Refresh();
                            JSFTCacheManager.HasCheckedHardLinkStatus = false;
                            JSFTCacheManager.ShouldPerformCleanup = true;
                            isDirty = true;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.Label("CACHE PATH", _headerGuiStyle);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(JSFTPreferences.CachePathIsAutomatic ? "Automatic" : "Manual", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (JSFTPreferences.GlobalSettings)
                {
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Label("Enable project-wide settings to set a custom path", _smallTextGuiStyle);
                }
                else
                {
                    if (GUILayout.Button("Set"))
                    {
                        if (JSFTPreferences.EnableHardLinks)
                        {
                            EditorUtility.DisplayDialog("Hard Links Enabled",
                                "You need to disable hard links first before changing the cache path. You can re-enable hard links afterwards.",
                                "OK");
                        }
                        else
                        { 
                            string cachePath = EditorUtility.SaveFolderPanel("Browse to a your desired cache folder", JSFTShared.ProjectPath, "LocalCache");
                            if (!string.IsNullOrEmpty(cachePath))
                            {
                                if (Directory.Exists(cachePath) && !JSFTShared.DirectoryIsEmpty(cachePath))
                                {
                                    EditorUtility.DisplayDialog("Directory Is Not Empty",
                                        "The directory you picked is not empty. Please empty the contents of that directory or pick an empty directory.",
                                        "OK");
                                }
                                else {
                                    JSFTPreferences.CachePath = cachePath;
                                    isDirty = true;
                                }
                            }
                        }
                    }
                    if (GUILayout.Button("Reset"))
                    {
                        if (JSFTPreferences.EnableHardLinks)
                        {
                            EditorUtility.DisplayDialog("Hard Links Enabled",
                                "You need to disable hard links first before changing the cache path. You can re-enable hard links afterwards.",
                                "OK");
                        }
                        else
                        {
                            if (!JSFTPreferences.CachePathIsAutomatic)
                            {
                                if (Directory.Exists(JSFTPreferences.AutomaticCachePath) &&
                                    !JSFTShared.DirectoryIsEmpty(JSFTPreferences.AutomaticCachePath))
                                {
                                    EditorUtility.DisplayDialog("Directory Is Not Empty",
                                        "Automatic cache directory is not empty. Please empty this directory manually:\n" +
                                        JSFTPreferences.AutomaticCachePath,
                                        "OK");
                                }
                                else
                                {
                                    JSFTPreferences.CachePath = null;
                                    isDirty = true;
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
                GUILayout.Label(JSFTPreferences.CachePath, _smallTextGuiStyle);

                GUILayout.Space(5);

                GUILayout.Label("HARD LINKS (EXPERIMENTAL)", _headerGuiStyle);

                GUILayout.Space(5);

                GUILayout.Label(
                    "Hard links boost performance by having the cache stay in place and instead change directory links during platform switch. This feature is still experimental and may cause issues on some platforms and under certain conditions. Cannot be used with custom cache path.",
                    _smallTextGuiStyle, GUILayout.Width(position.width - 25));

                GUILayout.Space(5);

                GUI.enabled = !JSFTCacheManager.BackgroundCacheCompressionInProgress &&
                              !JSFTCacheManager.PlatformRefreshInProgress &&
                              !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling &&
                              !EditorApplication.isUpdating;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Enable Hard Links");
                GUILayout.FlexibleSpace();

                bool enableHardLinks = EditorGUILayout.Toggle("", JSFTPreferences.EnableHardLinks, GUILayout.Width(15));
                if (enableHardLinks != JSFTPreferences.EnableHardLinks)
                {
                    if (enableHardLinks)
                    {
						if (Application.platform == RuntimePlatform.OSXEditor && JSFTPreferences.CachePath.StartsWith("/Volumes/")) {
							EditorUtility.DisplayDialog("I'm sorry, Dave. I'm afraid I can't do that.", "OS X hard links cannot point to different volumes. You need to pick a cache directory on the same volume as your project.", "OK");
						}
                        else if (EditorUtility.DisplayDialog("Enable hard links?",
                            "WARNING: Hard links is an expirmental feature. It is recommended you BACKUP YOUR PROJECTS before using it.\n\nHard links can fail under a number of conditions including: unsupported file system, network shares, unsupported platform...\n\nTHIS OPTION IS SYSTEM-WIDE AND WILL BE ACTIVATED ON OPENING ANY OF YOUR FAST PLATFORM SWITCH V1.4+ ENABLED PROJECTS.\n\nIn case of trouble, contact us at contact@jemast.com to resolve issues.",
                            "Ok, I understand!", "Wait... Cancel!"))
                        {
                            JSFTPreferences.EnableHardLinks = true;
                            JSFTCacheManager.HasCheckedHardLinkStatus = false;
                            JSFTCacheManager.ShouldPerformCleanup = true;
                            isDirty = true;
                        }
                    }
                    else
                    {
                        JSFTPreferences.EnableHardLinks = false;
                        JSFTCacheManager.HasCheckedHardLinkStatus = false;
                        JSFTCacheManager.ShouldPerformCleanup = true;
                        isDirty = true;
                    }
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.Label("COMPRESSION", _headerGuiStyle);

                GUILayout.Space(10);

                GUI.enabled = !JSFTCacheManager.BackgroundCacheCompressionInProgress &&
                              !JSFTCacheManager.PlatformRefreshInProgress &&
                              !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling &&
                              !EditorApplication.isUpdating;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Automatic Compression");
                GUILayout.FlexibleSpace();
                JSFTPreferences.AutoCompress = EditorGUILayout.Toggle("", JSFTPreferences.AutoCompress, GUILayout.Width(15));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Compression Algorithm");
                GUILayout.FlexibleSpace();
                JSFTPreferences.CompressionAlgorithm = EditorGUILayout.Popup(JSFTPreferences.CompressionAlgorithm,
                    JSFTShared.CompressionAlgorithmOptions);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Compression Quality");
                GUILayout.FlexibleSpace();
                if (JSFTPreferences.CompressionAlgorithm == 0)
                    JSFTPreferences.CompressionQualityLz4 = EditorGUILayout.Popup(JSFTPreferences.CompressionQualityLz4,
                        JSFTShared.CompressionQualityLz4Options);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);
                if (GUILayout.Button("Compress all cached data immediately"))
                {
                    JSFTCacheManager.CompressAllCache(false);
                    _cacheData = null;
                    isDirty = true;
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Decompress all cached data immediately"))
                {
                    JSFTCacheManager.DecompressAllCache();
                    _cacheData = null;
                    isDirty = true;
                }

                GUI.enabled = true;

                GUILayout.Space(10);

                GUILayout.Label("CLEAN UP", _headerGuiStyle);

                GUILayout.Space(10);

                GUI.enabled = !JSFTCacheManager.BackgroundCacheCompressionInProgress &&
                              !JSFTCacheManager.PlatformRefreshInProgress &&
                              !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling &&
                              !EditorApplication.isUpdating;
                if (GUILayout.Button("Delete all cached data immediately"))
                {
                    if (EditorUtility.DisplayDialog("Delete all cached data?",
                        "All cached data will be destroyed and switching platform will result in cache reinitialization and full asset reimport for that platform.",
                        "Ok, I understand!", "Wait... Cancel!"))
                    {
                        JSFTCacheManager.ClearAllCache();
                        _cacheData = null;
                        isDirty = true;
                    }
                }
                GUI.enabled = true;

                if (GUILayout.Button("Unlock user interface"))
                {
                    if (EditorUtility.DisplayDialog("Unlock user interface?",
                        "If the plugin user interface is stuck in either 'Waiting for platform refresh to end' or 'Waiting for background compression to end' and you are sure that the switch/compression operation halted or crashed and is over, you can use this functionnality to force unlocking the user interface.",
                        "Ok, I understand!", "Wait... Cancel!"))
                    {
                        ForceCleanup();
                    }
                }

                if (GUILayout.Button("Clear all cache & Reimport everything"))
                {
                    if (EditorUtility.DisplayDialog("Perform full clean up?",
                        "You'll need to accept to Reimport all assets when prompted. You will also be prompted to save your current scene if you haven't done it. Good luck...",
                        "Ok, I understand!", "Wait... Cancel!"))
                    {
                        JSFTCacheManager.FixIssues();
                        _cacheData = null;
                        isDirty = true;
                    }
                }

                GUILayout.Space(10);

                GUILayout.Label("VERSION CONTROL", _headerGuiStyle);

                GUILayout.Space(10);

                if (GUILayout.Button("Add Git ignore rule"))
                {
                    string vcFilePath = JSFTShared.ProjectPath + "/.gitignore";
                    if (!File.Exists(vcFilePath))
                    {
                        FileStream stream = File.Create(vcFilePath);
                        stream.Dispose();
                    }
                    string vcFileContent = File.ReadAllText(vcFilePath);
                    if (!vcFileContent.Contains("/LocalCache/"))
                    {
                        vcFileContent += "\n\n#Fast Platform Switch\n/LocalCache/";
                        File.WriteAllText(vcFilePath, vcFileContent);
                    }
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Add Mercurial ignore rule"))
                {
                    string vcFilePath = JSFTShared.ProjectPath + "/.hgignore";
                    if (!File.Exists(vcFilePath))
                    {
                        FileStream stream = File.Create(vcFilePath);
                        stream.Dispose();
                    }
                    string vcFileContent = File.ReadAllText(vcFilePath);
                    if (!vcFileContent.Contains("^LocalCache/"))
                    {
                        vcFileContent += "\n\n#Fast Platform Switch\nsyntax: regexp\n^LocalCache/";
                        File.WriteAllText(vcFilePath, vcFileContent);
                    }
                }
                GUILayout.Space(5);
                GUI.skin.label.wordWrap = true;
                GUILayout.Label(
                    "Those rules apply for automatic cache path. For other version control systems, make sure you ignore the \"LocalCache\" directory located in your project folder at the same level as the \"Assets\" directory.",
                    _smallTextGuiStyle, GUILayout.Width(position.width - 25));
                GUI.skin.label.wordWrap = false;

                GUILayout.Space(10);

                GUILayout.Label("DEBUG", _headerGuiStyle);

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Enable Log File");
                GUILayout.FlexibleSpace();
                JSFTPreferences.EnableLogFile = EditorGUILayout.Toggle("", JSFTPreferences.EnableLogFile, GUILayout.Width(15));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();
            }

            GUILayout.Space(10);
        }

        if (isDirty)
        {
            _cacheData = null;
            EditorUtility.SetDirty(this);
            Repaint();
        }
    }

    private void ForceCleanup()
    {
        JSFTCacheManager.ShouldPerformCleanup = true;
        JSFTCacheManager.PlatformRefreshInProgress = false;
        JSFTCacheManager.SwitchOperationInProgress = false;
        JSFTCacheManager.PlatformRefreshCurrentScene = "";
        JSFTCacheManager.SwitchOperationIsApi = false;
        JSFTCacheManager.PlatformRefreshShouldBustCache = false;
        File.Delete(JSFTPreferences.CachePath + "Background.txt");
        EditorUtility.ClearProgressBar();
    }

    private void SetupWindow()
    {
        if (JSFTCacheManager.PlatformRefreshInProgress)
            return;

        string assetsPath = JSFTShared.EditorAssetsPath;

        // Load textures
        _buildTargetTextures = new[]
        {
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WEBPLAYER.png", typeof (Texture2D)) as Texture2D,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WEBGL.png", typeof (Texture2D)) as Texture2D,
#endif
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_STANDALONE.png", typeof (Texture2D)) as
                Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_IOS.png", typeof (Texture2D)) as Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID.png", typeof (Texture2D)) as Texture2D,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10.png", typeof (Texture2D)) as Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_METRO.png", typeof (Texture2D)) as Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WP8.png", typeof (Texture2D)) as Texture2D,
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_NACL.png", typeof(Texture2D)) as Texture2D,
			#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_FLASH.png", typeof(Texture2D)) as Texture2D,
			#endif
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_PS3.png", typeof (Texture2D)) as Texture2D,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_PS4.png", typeof (Texture2D)) as Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_VITA.png", typeof (Texture2D)) as Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_PSM.png", typeof (Texture2D)) as Texture2D,
#endif
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_X360.png", typeof (Texture2D)) as Texture2D,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_XBONE.png", typeof (Texture2D)) as Texture2D
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_WII.png", typeof(Texture2D)) as Texture2D
#endif
        };

        _androidSubtargetTextures = new[]
        {
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID.png", typeof (Texture2D)) as Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_DXT.png", typeof (Texture2D)) as
                Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_PVRTC.png", typeof (Texture2D)) as
                Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ATC.png", typeof (Texture2D)) as
                Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ETC1.png", typeof (Texture2D)) as
                Texture2D,
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ETC2.png", typeof (Texture2D)) as
                Texture2D,
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_ANDROID_ASTC.png", typeof (Texture2D)) as
                Texture2D
#endif
        };

#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
        _blackberrySubtargetTextures = new[]
        {
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10.png", typeof (Texture2D)) as Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_PVRTC.png", typeof (Texture2D)) as
                Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_ATC.png", typeof (Texture2D)) as Texture2D,
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_BB10_ETC1.png", typeof (Texture2D)) as Texture2D
        };
#endif

        _selectedBuildTargetTexture =
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Selected.png", typeof (Texture2D)) as Texture2D;
        _trashBuildTargetTexture =
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Trash.png", typeof (Texture2D)) as Texture2D;
        _zipBuildTargetTexture =
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Zip.png", typeof (Texture2D)) as Texture2D;
        _unzipBuildTargetTexture =
            AssetDatabase.LoadAssetAtPath(assetsPath + "Images/Platform_Unzip.png", typeof (Texture2D)) as Texture2D;

        string sharedPath = JSFTShared.SharedEditorAssetsPath;
        _shadedBackgroundTexture =
            AssetDatabase.LoadAssetAtPath(sharedPath + "Images/shadedBackground.png", typeof (Texture2D)) as Texture2D;

        // Setup current wanted target
        _wantedCacheTarget = JSFTCacheManager.CurrentCacheTarget;
        _wantedCacheSubtarget = JSFTCacheManager.CurrentCacheSubtarget;

        // Setup rects for list view handling
        _listViewRects = new Rect[BuildTargetOptions.Length];

        // Setup font styles
        _listFontStyle.fontSize = 12;
        _statusFontStyle.fontSize = 10;
        _statusFontCachedStyle.fontSize = 10;
        _statusFontCompressedStyle.fontSize = 10;

        _smallTextGuiStyle.fontSize = 9;
        _smallTextGuiStyle.padding = new RectOffset(5,5,0,0);
        _smallTextGuiStyle.wordWrap = true;

        if (EditorGUIUtility.isProSkin)
        {
            _listFontStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
            _statusFontStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
            _statusFontCachedStyle.normal.textColor = new Color(0.0f, 0.6f, 0.0f, 1.0f);
            _statusFontCompressedStyle.normal.textColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
            _smallTextGuiStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }
        else
        {
            _listFontStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            _statusFontStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            _statusFontCachedStyle.normal.textColor = new Color(0.0f, 0.6f, 0.0f, 1.0f);
            _statusFontCompressedStyle.normal.textColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
            _smallTextGuiStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
        }

        // Set up GUI styles
        _areaStyle.normal.background = MakeTex(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.025f));
        _areaStyleAlt.normal.background = MakeTex(1, 1, new Color(1.0f, 1.0f, 1.0f, 0.05f));
        _areaStyleActive.normal.background = MakeTex(1, 1, new Color(0.0f, 0.5f, 1.0f, 0.5f));

        // Set up textures
        string[] allTextures =
        {
            assetsPath + "Images/Platform_WEBPLAYER.png",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 // UNITY 5.0+
            assetsPath + "Images/Platform_WEBGL.png",
#endif
            assetsPath + "Images/Platform_STANDALONE.png",
            assetsPath + "Images/Platform_IOS.png",
            assetsPath + "Images/Platform_ANDROID.png",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            assetsPath + "Images/Platform_BB10.png",
            assetsPath + "Images/Platform_BB10_PVRTC.png",
            assetsPath + "Images/Platform_BB10_ATC.png",
            assetsPath + "Images/Platform_BB10_ETC1.png",
            assetsPath + "Images/Platform_METRO.png",
            assetsPath + "Images/Platform_WP8.png",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3)
			assetsPath + "Images/Platform_NACL.png",
			#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
			assetsPath + "Images/Platform_FLASH.png",
			#endif
            assetsPath + "Images/Platform_PS3.png",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
            assetsPath + "Images/Platform_PS4.png",
            assetsPath + "Images/Platform_VITA.png",
            assetsPath + "Images/Platform_PSM.png",
#endif
            assetsPath + "Images/Platform_X360.png",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
            assetsPath + "Images/Platform_XBONE.png",
#endif
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			assetsPath + "Images/Platform_WII.png",
#endif
            assetsPath + "Images/Platform_ANDROID_DXT.png",
            assetsPath + "Images/Platform_ANDROID_PVRTC.png",
            assetsPath + "Images/Platform_ANDROID_ATC.png",
            assetsPath + "Images/Platform_ANDROID_ETC1.png",
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            assetsPath + "Images/Platform_ANDROID_ETC2.png",
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            assetsPath + "Images/Platform_ANDROID_ASTC.png",
#endif
            assetsPath + "Images/Platform_Selected.png",
            assetsPath + "Images/Platform_Trash.png",
            assetsPath + "Images/Platform_Zip.png",
            assetsPath + "Images/Platform_Unzip.png",
            sharedPath + "Images/shadedBackground.png"
        };

        foreach (string path in allTextures)
        {
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter == null)
                continue;

            bool hasChange = false;
            if (textureImporter.textureType != TextureImporterType.GUI)
            {
                textureImporter.textureType = TextureImporterType.GUI;
                hasChange = true;
            }
            if (textureImporter.filterMode != FilterMode.Bilinear)
            {
                textureImporter.filterMode = FilterMode.Bilinear;
                hasChange = true;
            }
            if (textureImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor)
            {
                textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                hasChange = true;
            }

            if (hasChange)
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    private void SetupGuiStyles(GUISkin skin)
    {
        _headerGuiStyle = new GUIStyle(skin.GetStyle("Label"))
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            padding = new RectOffset(10, 10, 5, 5),
            margin = new RectOffset(0, 0, 5, 5),
            normal = {background = _shadedBackgroundTexture}
        };

        _toolbarGuiStyle = new GUIStyle(skin.GetStyle("toolbarbutton"))
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            fixedHeight = 30
        };

        _pipelineButtonGuiStyle = new GUIStyle(skin.GetStyle("Button"))
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            margin = new RectOffset(10, 10, 10, 10)
        };
    }

    // Handy method by andeeeee...
    private static Texture2D MakeTex(int width, int height, Color col)
    {
        var pix = new Color[width*height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        var result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    private static int CacheSubtargetToAndroidCompressionOption(JSFTShared.CacheSubtarget? target)
    {
        switch (target)
        {
            case JSFTShared.CacheSubtarget.Android_GENERIC:
                return 0;
            case JSFTShared.CacheSubtarget.Android_ETC:
                return 4;
            case JSFTShared.CacheSubtarget.Android_DXT:
                return 1;
            case JSFTShared.CacheSubtarget.Android_PVRTC:
                return 2;
            case JSFTShared.CacheSubtarget.Android_ATC:
                return 3;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            case JSFTShared.CacheSubtarget.Android_ETC2:
                return 5;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            case JSFTShared.CacheSubtarget.Android_ASTC:
                return 6;
#endif
            default:
                return 0;
        }
    }

    private static JSFTShared.CacheSubtarget? AndroidCompressionOptionToCacheSubtarget(int option)
    {
        switch (option)
        {
            case 0:
                return JSFTShared.CacheSubtarget.Android_GENERIC;
            case 1:
                return JSFTShared.CacheSubtarget.Android_DXT;
            case 2:
                return JSFTShared.CacheSubtarget.Android_PVRTC;
            case 3:
                return JSFTShared.CacheSubtarget.Android_ATC;
            case 4:
                return JSFTShared.CacheSubtarget.Android_ETC;
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
            case 5:
                return JSFTShared.CacheSubtarget.Android_ETC2;
#endif
#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            case 6:
                return JSFTShared.CacheSubtarget.Android_ASTC;
#endif
            default:
                return null;
        }
    }


#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1
    private static int CacheSubtargetToBlackBerryCompressionOption(JSFTShared.CacheSubtarget? target)
    {
        switch (target)
        {
            case JSFTShared.CacheSubtarget.BlackBerry_GENERIC:
                return 0;
            case JSFTShared.CacheSubtarget.BlackBerry_PVRTC:
                return 1;
            case JSFTShared.CacheSubtarget.BlackBerry_ATC:
                return 2;
            case JSFTShared.CacheSubtarget.BlackBerry_ETC:
                return 3;
            default:
                return 0;
        }
    }


    private static JSFTShared.CacheSubtarget? BlackBerryCompressionOptionToCacheSubtarget(int option)
    {
        switch (option)
        {
            case 0:
                return JSFTShared.CacheSubtarget.BlackBerry_GENERIC;
            case 1:
                return JSFTShared.CacheSubtarget.BlackBerry_PVRTC;
            case 2:
                return JSFTShared.CacheSubtarget.BlackBerry_ATC;
            case 3:
                return JSFTShared.CacheSubtarget.BlackBerry_ETC;
            default:
                return null;
        }
    }
#endif

    private void EditorUpdate()
    {
        if (!_shouldSwitch) return;

        _shouldSwitch = false;
        PerformSwitch();
    }

    private void PerformSwitch()
    {
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		if (EditorSettings.externalVersionControl == ExternalVersionControl.Disabled) {
			if (EditorUtility.DisplayDialog("External Version Control Required", "You need to enable external version control for Fast Platform Switch proper operation.", "Enable Meta Files", "Cancel")) {
				EditorSettings.externalVersionControl = ExternalVersionControl.Generic;
			}
		}
		#endif

        AssetDatabase.Refresh();

#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		if (EditorSettings.externalVersionControl != ExternalVersionControl.Disabled) {
		#endif
        JSFTCacheManager.SwitchPlatform(_wantedCacheTarget, _wantedCacheSubtarget, false);
#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		}
		#endif
    }
}