using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
/// <summary>
/// AssetBundle编辑
/// </summary>
public class AssetBundleEditor
{
    #region 自动做标记

    // 思路 :
    // 1. 找到资源保存的文件夹
    // 2. 遍历里面的每个场景的文件夹
    // 3. 遍历场景文件夹里的所有文件系统
    // 4. 如果访问的是文件夹 ， 再继续访问里面的所有文件系统，直到找到文件 (递归思想)；
    // 5. 找到文件 ， 就要修改它的 AssetBundle Labels ;
    // 6. 用 AssetImporter 类 ， 修改名称和后缀 ；
    // 7. 保存对应的文件夹名和具体路径 ；
    
    [MenuItem("AssetBundle/Set AssetBundle Labels(自动做标记) %#k")]
    public static void SetAssetBundleLabels()
    {
         // 移除没用到的包名
        AssetDatabase.RemoveUnusedAssetBundleNames();
        // 1.找到资源保存的文件夹
        // TODO 视图 - 任务列表
        string assetDirectory = Application.dataPath + "/Res";
        DirectoryInfo directoryInfo = new DirectoryInfo(assetDirectory);
         // 获取所有场景文件夹
        DirectoryInfo[] scenendirectories = directoryInfo.GetDirectories();
         // 2. 遍历里面的每个场景的文件夹
        foreach (DirectoryInfo tempDirectoryInfo in scenendirectories)
        {
             // 获取场景的文件夹
            string sceneDirectory = assetDirectory + "/" + tempDirectoryInfo.Name;
            DirectoryInfo sceneDirectoryInfo = new DirectoryInfo(sceneDirectory);
            if (sceneDirectoryInfo == null)
            {
                Debug.LogError("场景文件夹" + sceneDirectory + "不存在");
                Debug.LogError("该目录下不存在此文件夹!" + sceneDirectory);
                return;
            }
            else
            {
                // 7. 保存对应的文件夹名和具体路径 ；
                Dictionary<string, string> namePathDic = new Dictionary<string, string>();

                //  3. 遍历场景文件夹里的所有文件系统
                // sceneDirectory就是下面路径 [Windows路径]
                // D:\Unity_forWork\Unity_Project\AssetBundle02_Senior\Assets\Res\Scene1
                // [Unity路径],注意 \ / 
                // D:\Unity_forWork\Unity_Project\AssetBundle02_Senior\Assets\Res\Scene1
                int index = sceneDirectory.LastIndexOf('/');
                string sceneName = sceneDirectory.Substring(index + 1);
                //Debug.Log("场景名 : " + sceneName);
                //Debug.Log("路径 : " + sceneDirectory);
                OnSceneFileSystemInfo(sceneDirectoryInfo, sceneName,namePathDic);


                OnWriteConfig(sceneName,namePathDic);

            }
        }

        AssetDatabase.Refresh();
        Debug.Log("设置成功!");


    }
    /// <summary>
    /// 7. 保存对应的文件夹名和具体路径 ；
    /// </summary>
    private static  void OnWriteConfig(string sceneName , Dictionary<string,string> namePathDic )
    {
        string path = PathUtil.GetAssetBundleOutPath() + "/" + sceneName + "Record.config";
        // Debug.Log(path); // D:/Unity_forWork/Unity_Project/AssetBundle02_Senior/Assets/AssetBundles/Scene3Record.config

        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(namePathDic.Count);


                foreach (var item in namePathDic)
                {
                    sw.WriteLine(item.Key + "--" + item.Value);
                }  
            }
        }


    }

    /// <summary>
    /// 3. 遍历场景文件夹里的所有文件系统
    /// </summary>
    /// <param name="fileSystemInfo">文件</param>
    /// <param name="sceneName">场景名字</param>
    private static void OnSceneFileSystemInfo(FileSystemInfo fileSystemInfo , string sceneName, Dictionary<string, string> namePathDic)
    {
        if (!fileSystemInfo.Exists)
        {
            Debug.LogError(fileSystemInfo.FullName + "不存在！");
        }

        DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
         // 获取所有文件系统[包括文件夹和文件]
       FileSystemInfo[] fileSystemInfos =  directoryInfo.GetFileSystemInfos();
        foreach (FileSystemInfo tempfileSystemInfo in fileSystemInfos)
        {
            FileInfo fileInfo = tempfileSystemInfo as FileInfo;
            if (fileInfo==null)
            {
                // 代表强转失败 ,不是文件 , 是文件夹
                // 4. 如果访问的是文件夹 ， 再继续访问里面的所有文件系统，直到找到文件 (递归思想)；
                // 再调用本方法 
                OnSceneFileSystemInfo(tempfileSystemInfo, sceneName, namePathDic);
            }
            else
            {
                // 代表强转成功了 , 是文件 ;
                // 5. 找到文件 ， 就要修改它的 AssetBundle Labels ;
                SetLabels(fileInfo, sceneName,  namePathDic);
            }
        }
    }
    /// <summary>
    /// 5. 找到文件 ，就要修改它的 AssetBundle Labels ;
    /// </summary>
    private static void SetLabels(FileInfo fileInfo , string sceneName, Dictionary<string, string> namePathDic)
    {
         // Unity自身产生的 .meta文件 不用去读 ；
        if (fileInfo.Extension == ".meta")     
            return;  
        string bundleName =  GetBundleName(fileInfo,sceneName);
        //Debug.Log("包名为 :" + bundleName );

        // D:\Unity_forWork\Unity_Project\AssetBundle02_Senior\Assets\Res\Scene1\Character\NB\Player4.prefab
        // 获取Assetsy以及之后的目录
        int assetIndex = fileInfo.FullName.IndexOf("Assets");
        string assetPath =  fileInfo.FullName.Substring(assetIndex);
        //Debug.Log(assetPath); // Assets\Res\Scene1\Character\NB\Player4.prefab

         // 6.用 AssetImporter 类 ， 修改名称和后缀 ；
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath); // GetAtPath方法是获取Assets文件和之后的目录
        assetImporter.assetBundleName = bundleName.ToLower();
        if (fileInfo.Extension == ".unity")
            assetImporter.assetBundleVariant = "u3d";
        else       
            assetImporter.assetBundleVariant = "assetbundle";


        // 添加到字典里 
        string folderName = "";
        if (bundleName.Contains("/"))
            folderName = bundleName.Split('/')[1]; // key
        else
            folderName = bundleName;
        string bundlePath = assetImporter.assetBundleName + "." + assetImporter.assetBundleVariant; // value
        if (!namePathDic.ContainsKey(folderName))
            namePathDic.Add(folderName, bundlePath);

    }
    /// <summary>
    /// 获取包名
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="sceneName"></param>
    private static string  GetBundleName(FileInfo fileInfo , string sceneName)
    { 
         // windows 全路径 \ 
        string windowsPath = fileInfo.FullName;
         // D:\Unity_forWork\Unity_Project\AssetBundle02_Senior\Assets\Res\Scene1\Character\NB\Player4.prefab
         // Debug.Log(windowsPath);
         // 转换成Unity可识别的路径 把 \ 改为 /   [加@有奇效]     
        string unityPath = windowsPath.Replace(@"\","/");
        // D:/Unity_forWork/Unity_Project/AssetBundle02_Senior/Assets/Res/Scene1/Character/NB/Player4.prefab
        // D:/Unity_forWork/Unity_Project/AssetBundle02_Senior/Assets/Res/Scene1/Scene1.unity
        // Debug.Log(unityPath);

        // 获取Scene之后的部分 :  Character/NB/Player4.prefab
        int sceneIndex = unityPath.IndexOf(sceneName) + sceneName.Length;
        string bundlePath = unityPath.Substring(sceneIndex + 1);

        if (bundlePath.Contains("/"))
        {
            // 后续还有路径, 包含子目录[取第一个/后的名字]
            // Character/NB/Player4.prefab
            string tempPath = bundlePath.Split('/')[0];
            return sceneName + "/" + tempPath;
        }else
        {
            // 如果是场景
            // Scene1.unity
            return sceneName;

        }
        
    }
    #endregion

    #region 打包

    [MenuItem("AssetBundle/BuildAllAssetBundle(一键打包)")]
    static void BuildAllAssetBundles()
    {
        string outPath = PathUtil.GetAssetBundleOutPath();

        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }

    #endregion

    #region 删除
    [MenuItem("AssetBundle/DeleteAllAssetBundle(一键删除)")]
    private  static void DeleteAssetBundle()
    {
        string outPath = PathUtil.GetAssetBundleOutPath();
        Directory.Delete(outPath, true); // 删除 AssetBundle文件夹 , true 强势删除 ;
        File.Delete(outPath + ".meta"); // unity 自带的 .meta 文件也删掉 ;
        AssetDatabase.Refresh();

    }

    #endregion



}
