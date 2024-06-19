
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.PackageManager.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
#endif

public class MenuExten : MonoBehaviour
{
    public const string BUILD_VERSIONNAME = "0.0.1";
    public const int BUILD_VERSIONCODE = 001;

    const string BUILD_PASSWORD = "gabros2023!@#";
    public static readonly string[] DEFINE_BUILD = { /*"UNITY_IAP", "USE_DOTWEEN",
        "ACTIVE_FIREBASE", "ACTIVE_FIREBASE_REMOTE","ACTIVE_FIREBASE_CRASHLYTICS","ACTIVE_FIREBASE_ANALYTICS",
        "ACTIVE_FACEBOOK","ACTIVE_IRONSOURCE" ,"ODIN_INSPECTOR","TUTORIAL","SPINE_SKIP"*/};
    const string DEFINE_UNLOCKALL = "DEVELOPMENT";

    const string BUILD_NAME_DEV = "zName_{0}_dev";
    const string BUILD_NAME_FINAL = "zName_{0}_product_signed";

    const string MENU_NAME = "(=^M^=)";

    private const string ALT = "&";
    private const string SHIFT = "#";
    private const string CTRL = "%";


#if UNITY_EDITOR
    [MenuItem(MENU_NAME + "/Intro " + ALT + "1")]
    static void Boots()
    {
        LoadSceneByName("Loading");

    }


    [MenuItem(MENU_NAME + "/Map1 " + ALT + "2")]
    static void Map1()
    {
        LoadSceneByName("Map1-New");

    }

    [MenuItem(MENU_NAME + "/Map2 " + ALT + "3")]
    static void Map2()
    {
        LoadSceneByName("Map2-New");

    }

    static void LoadSceneByName(string _nameScene)
    {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/0_Main/_Scenes/" + _nameScene + ".unity");
    }


    //=====================
    [MenuItem(MENU_NAME + "/Config")]
    static void BuildCongfig()
    {
        PlayerSettings.keystorePass = BUILD_PASSWORD;
        PlayerSettings.keyaliasPass = BUILD_PASSWORD;
        //PlayerSettings.bundleVersion = BUILD_VERSIONNAME;
        //PlayerSettings.Android.bundleVersionCode = BUILD_VERSIONCODE;
        //PlayerSettings.iOS.buildNumber = BUILD_VERSIONCODE.ToString();
        Debug.Log("Config"
           + "\nPassword=  " + PlayerSettings.keystorePass
            + "\nVersion Name=  " + PlayerSettings.bundleVersion
            + "\nVersion Code=  " + PlayerSettings.Android.bundleVersionCode);
        //  SetupBuild(false, false);
        //EditorUserBuildSettings.activeScriptCompilationDefines=_script.ToArray();
    }

    //[MenuItem(MENU_NAME + "/Build/Config/ConfigDev")]
    static void BuildCongfigDev()
    {
        PlayerSettings.keystorePass = BUILD_PASSWORD;
        PlayerSettings.keyaliasPass = BUILD_PASSWORD;
        PlayerSettings.bundleVersion = BUILD_VERSIONNAME;
        PlayerSettings.Android.bundleVersionCode = BUILD_VERSIONCODE;

        Debug.Log("Config"
           + "\nPassword=  " + PlayerSettings.keystorePass
            + "\nVersion Name=  " + PlayerSettings.bundleVersion
            + "\nVersion Code=  " + PlayerSettings.Android.bundleVersionCode);
        ActiveDevOption(true);
    }
    //[MenuItem(MENU_NAME + "/Build/Config/ConfigNonDev")]
    static void BuildCongfigNonDev()
    {
        PlayerSettings.keystorePass = BUILD_PASSWORD;
        PlayerSettings.keyaliasPass = BUILD_PASSWORD;
        PlayerSettings.bundleVersion = BUILD_VERSIONNAME;
        PlayerSettings.Android.bundleVersionCode = BUILD_VERSIONCODE;
        Debug.Log("Config"
           + "\nPassword=  " + PlayerSettings.keystorePass
            + "\nVersion Name=  " + PlayerSettings.bundleVersion
            + "\nVersion Code=  " + PlayerSettings.Android.bundleVersionCode);
        ActiveDevOption(false);
    }

    //[MenuItem(MENU_NAME + "/Build/Build/Build Apk-Dev " + BUILD_VERSIONNAME)]
    public static void AutoBuildAPKDEV()
    {
        Debug.Log("Building APK-DEV...");
        SetupBuild(false, true);
    }
    //[MenuItem(MENU_NAME + "/Build/Build/Build Final-apk " + BUILD_VERSIONNAME)]
    public static void AutoBuildApk()
    {
        Debug.Log("Building apk-Final...");
        SetupBuild(false, false);
    }
    //[MenuItem(MENU_NAME + "/Build/Build/Build Final-aab " + BUILD_VERSIONNAME)]
    public static void AutoBuildaab()
    {
        Debug.Log("Building aab-Final...");
        SetupBuild(true, false);
    }
    [MenuItem(MENU_NAME + "/Build/Build/Build ALL " + BUILD_VERSIONNAME)]
    public static void AutoBuilAll()
    {
        Debug.Log("Building ALL...");
        Debug.Log("Building APK-DEV...");
        SetupBuild(false, true, () =>
        {
            Debug.Log("Building apk-Final...");
            SetupBuild(false, false, () =>
            {
                Debug.Log("Building aab-Final...");
                SetupBuild(true, false);
            });
        });
    }
    static void ActiveDevOption(bool buildDev)
    {
        //string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        //List<string> _script = definesString.Split(';').ToList();

        List<string> _script = new List<string>(DEFINE_BUILD);
        if (buildDev)
        {
            if (!_script.Contains(DEFINE_UNLOCKALL))
            {
                _script.Add(DEFINE_UNLOCKALL);
                Debug.Log("Add Build " + DEFINE_UNLOCKALL);
            }
#if UNITY_2020_1_11
			string definesString = "";
			foreach (var item in _script)
			{
				definesString += item + ";";
			}
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, definesString);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, definesString);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _script.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _script.ToArray());
#endif

        }
        else
        {
            if (_script.Contains(DEFINE_UNLOCKALL))
            {
                _script.Remove(DEFINE_UNLOCKALL);
                Debug.Log("Remove Build " + DEFINE_UNLOCKALL);
            }
#if UNITY_2020_1_11
			string definesString = "";
			foreach (var item in _script)
			{
				definesString += item + ";";
			}
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, definesString);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, definesString);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _script.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _script.ToArray());
#endif
        }
        Debug.Log("Build Define: " + _script.ToString());

    }
    static void SetupBuild(bool AppBundle = true, bool buildDev = true, System.Action OnBuildDone = null)
    {
        BuildCongfig();
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        //EditorBuildSettings.
        BuildOptions bo = BuildOptions.None;
        //if (buildDev)
        //    bo = BuildOptions.Development;

        AndroidArchitecture aac = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

        // EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        // PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        PlayerSettings.Android.targetArchitectures = aac;
        EditorUserBuildSettings.buildAppBundle = AppBundle;


        ActiveDevOption(buildDev);

        string BUILD_PATH = "E:\\GameAPK";

        string fileName = buildDev ? BUILD_NAME_DEV : BUILD_NAME_FINAL;
        fileName = string.Format(fileName, BUILD_VERSIONNAME);
        string path = BUILD_PATH + fileName;
        path += AppBundle ? ".aab" : ".apk";
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.Android, bo);

        BuildSummary summary = report.summary;
        Debug.Log("Building...");
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded:\nFile Name:" + summary.outputPath
                + "\nVersion Name" + PlayerSettings.bundleVersion
                 + "\nVersion Code" + PlayerSettings.Android.bundleVersionCode
                + "\nFile Size:" + summary.totalSize / 1024 / 1024 + " MB\nGood Luck");
            if (OnBuildDone != null)
                OnBuildDone();
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
        if (summary.result == BuildResult.Succeeded)
            OpenFolderInWin(BUILD_PATH);

    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }


    [MenuItem(MENU_NAME+"/Remove All Data")]
    static void RemoveAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }


    #region Extension
    public static void OpenFolderInWin(string path)
    {
        bool openInsidesOfFolder = false;

        // try windows
        string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

        if (System.IO.Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }

        try
        {
            System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
        }
        catch (System.ComponentModel.Win32Exception e)
        {
            // tried to open win explorer in mac
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
    }
    #endregion

#endif
}


#if UNITY_EDITOR
//public class ScriptOptions
//{
//    //Auto Refresh


//    //kAutoRefresh has two posible values
//    //0 = Auto Refresh Disabled
//    //1 = Auto Refresh Enabled


//    //This is called when you click on the 'Tools/Auto Refresh' and toggles its value
//    [MenuItem("Tools/Auto Refresh")]
//    static void AutoRefreshToggle()
//    {
//        var status = EditorPrefs.GetInt("kAutoRefresh");
//        if (status == 1)
//            EditorPrefs.SetInt("kAutoRefresh", 0);
//        else
//            EditorPrefs.SetInt("kAutoRefresh", 1);
//    }


//    //This is called before 'Tools/Auto Refresh' is shown to check the current value
//    //of kAutoRefresh and update the checkmark
//    [MenuItem("Tools/Auto Refresh", true)]
//    static bool AutoRefreshToggleValidation()
//    {
//        var status = EditorPrefs.GetInt("kAutoRefresh");
//        if (status == 1)
//            Menu.SetChecked("Tools/Auto Refresh", true);
//        else
//            Menu.SetChecked("Tools/Auto Refresh", false);
//        return true;
//    }


//    //Script Compilation During Play


//    //ScriptCompilationDuringPlay has three posible values
//    //0 = Recompile And Continue Playing
//    //1 = Recompile After Finished Playing
//    //2 = Stop Playing And Recompile


//    //The following methods assing the three possible values to ScriptCompilationDuringPlay
//    //depending on the option you selected
//    [MenuItem("Tools/Script Compilation During Play/Recompile And Continue Playing")]
//    static void ScriptCompilationToggleOption0()
//    {
//        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 0);
//    }


//    [MenuItem("Tools/Script Compilation During Play/Recompile After Finished Playing")]
//    static void ScriptCompilationToggleOption1()
//    {
//        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 1);
//    }


//    [MenuItem("Tools/Script Compilation During Play/Stop Playing And Recompile")]
//    static void ScriptCompilationToggleOption2()
//    {
//        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 2);
//    }


//    //This is called before 'Tools/Script Compilation During Play/Recompile And Continue Playing'
//    //is shown to check for the current value of ScriptCompilationDuringPlay and update the checkmark
//    [MenuItem("Tools/Script Compilation During Play/Recompile And Continue Playing", true)]
//    static bool ScriptCompilationValidation()
//    {
//        //Here, we uncheck all options before we show them
//        Menu.SetChecked("Tools/Script Compilation During Play/Recompile And Continue Playing", false);
//        Menu.SetChecked("Tools/Script Compilation During Play/Recompile After Finished Playing", false);
//        Menu.SetChecked("Tools/Script Compilation During Play/Stop Playing And Recompile", false);


//        var status = EditorPrefs.GetInt("ScriptCompilationDuringPlay");


//        //Here, we put the checkmark on the current value of ScriptCompilationDuringPlay
//        switch (status)
//        {
//            case 0:
//                Menu.SetChecked("Tools/Script Compilation During Play/Recompile And Continue Playing", true);
//                break;
//            case 1:
//                Menu.SetChecked("Tools/Script Compilation During Play/Recompile After Finished Playing", true);
//                break;
//            case 2:
//                Menu.SetChecked("Tools/Script Compilation During Play/Stop Playing And Recompile", true);
//                break;
//        }
//        return true;
//    } 
//}


//[InitializeOnLoad]
//public class CompilerOptionsEditorScript
//{
//    static CompilerOptionsEditorScript()
//    {
//        EditorApplication.playModeStateChanged += PlaymodeChanged;
//    }


//    static void PlaymodeChanged(PlayModeStateChange state)
//    {
//        //Enable assembly reload when leaving play mode/entering edit mode
//        if (state == PlayModeStateChange.ExitingPlayMode
//            || state == PlayModeStateChange.EnteredEditMode)
//        {
//            EditorApplication.UnlockReloadAssemblies();
//        }


//        //Disable assembly reload when leaving edit mode/entering play mode
//        if (state == PlayModeStateChange.EnteredPlayMode
//            || state == PlayModeStateChange.ExitingEditMode)
//        {
//            EditorApplication.LockReloadAssemblies();
//        }
//    }
//}
#endif