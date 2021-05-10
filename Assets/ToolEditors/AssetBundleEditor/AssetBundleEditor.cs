using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleEditor : AssetPostprocessor
{
    public static BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
    [MenuItem("Assets/PackAb")]
    public static void BuildAssetBundle()
    {
        BuildAllAssetBundlesWithPath(GetBuildTarget(), null);
    }

    public static void BuildAllAssetBundlesWithPath(BuildTarget target, string path = null)
    {
        bundleOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
        BuildTarget buildTarget = target;
        if(path == null)
        {
            path = Path.Combine(Application.dataPath, "../_AssetsBundles/");
        }
        path = Path.Combine(path, buildTarget.ToString().ToUpper());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles(path, bundleOptions, buildTarget);
        AssetDatabase.Refresh();
    }

    public static BuildTarget GetBuildTarget()
    {
        BuildTarget platformTarget = BuildTarget.StandaloneWindows;
#if UNITY_IPHONE
        platformTarget = BuildTarget.iOS;
#endif
#if UNITY_ANDROID
            platformTarget = BuildTarget.Android;
#endif
        return platformTarget;
    }
}
