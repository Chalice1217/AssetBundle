using System.Collections;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
/// <summary>
/// 这个脚本最先执行
/// </summary>
public class GameInit : MonoBehaviour
{
    private string downloadPath;
    private void Awake()
    {
        downloadPath = PathUtil.GetAssetBundleOutPath();
        // 检测资源,进行比对 更新
        StartCoroutine("DownLoadResForTest");

        // 开始游戏主逻辑     
        gameObject.AddComponent<AssetBundleManager>();
        gameObject.AddComponent<LuaManager>();
        // 执行lua脚本
        LuaManager.Instance.DoString("require 'GameInit'");
        
        LuaManager.Instance.CallLuaFunction("GameInit", "Init");      
    }

    //#region 检测资源,对比MD5文件
    ///// <summary>
    ///// 检测资源
    ///// </summary>
    //private IEnumerator DownLoadRes()
    //{
    //    // 公司的服务器地址(以公司为准,此处是随便写的)
    //    string url = "http://127.0.0.1:8080/path/";

    //    string fileUrl = url + "file.txt";

    //    WWW www = new WWW(fileUrl);
    //    yield return www;
    //    if (www.error != null)     
    //        Debug.LogError("Error" + www.error);


    //    // 判断本地有没有这个目录
    //    // 如果发布到安卓端 : 游戏一运行,就把 streamingAssetsPath 的文件 拷贝到 persistentDataPath 路径下;


    //    if (!Directory.Exists(downloadPath))       
    //        Directory.CreateDirectory(downloadPath);
    //     // 把下载的文件写入本地
    //    File.WriteAllBytes(downloadPath + "/file.txt", www.bytes); ;

    //    // 读取里面的内容
    //    string fileText = www.text;
    //    string[] lines = fileText.Split('\n');

    //    for (int i = 0; i < lines.Length; i++)
    //    {
    //        if (string.IsNullOrEmpty(lines[i])) //有空行,就continue
    //            continue;

    //        string[] kv = lines[i].Split('|');
    //       //kv =  scene1 / character.assetbundle | 2503fdd66fea3185fa83930bc972bad2
    //        string fileName = kv[0];

    //        // 再拿到本地的这个文件
    //        string localFile = (downloadPath + "/" + fileName).Trim();

    //         // 如果没有本地文件 , 就下载
    //        if (!File.Exists(localFile))
    //        {
    //            string dir = Path.GetDirectoryName(localFile);
    //            Directory.CreateDirectory(dir);

    //            // 开始网络下载
    //            string tempUrl = url + fileName;
    //             www = new WWW(tempUrl);
    //            yield return www;
    //            if (www.error != null)
    //                Debug.LogError("Error" + www.error);

    //            File.WriteAllBytes(localFile, www.bytes);
    //        }
    //        else //如果文件有的话 , 就开始比对MD5 ,检测是否更新了
    //        {
    //            string md5 = kv[1];
    //            string localMD5 = GetFileMD5(localFile);

    //            // 开始比对
    //            if (md5 == localMD5)
    //            {
    //                // 相等的话,没更新
    //                Debug.Log("没更新");
    //            }
    //            else
    //            {
    //                // 不相等,说明更新了
    //                // 删除本地原来的旧文件
    //                File.Delete(localFile);

    //                //下载新文件
    //                string tmpUrl = url + fileName;
    //                www = new WWW(tmpUrl);
    //                yield return www;
    //                //进行一些网络的检测
    //                if (www.error != null)
    //                    Debug.LogError("Error" + www.error);

    //                File.WriteAllBytes(localFile, www.bytes);
    //            }
    //        }

    //    }

    //   yield return new WaitForEndOfFrame();

    //    Debug.Log("更新完成,可以开始游戏了");
    //}
    //#endregion


    /// <summary>
    /// 检测资源进行比对更新(模拟测试,本机使用)
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownLoadResForTest()
    {
        // 把unity  Assets/StreamingAssets/Windows下的所有文件考到"E:/LuaTest" 目录下作为服务器里面的文件 ;
        string url = "E:/LuaTest";

        string fileUrl = url + "/file.txt";

        //WWW www = new WWW(fileUrl);
        //yield return www;

        //if (www.error != null)
        //    Debug.LogError("Error" + www.error);


        // 判断本地有没有这个目录
        // 如果发布到安卓端 : 游戏一运行,就把 streamingAssetsPath 的文件 拷贝到 persistentDataPath 路径下;

        if (!Directory.Exists(downloadPath))
            Directory.CreateDirectory(downloadPath);

        //// 把下载的文件写入本地 file 文件里面
        //File.WriteAllBytes(downloadPath + "/file.txt", www.bytes); ;

        // 读取里面的内容
       // string fileText = File.ReadAllText(fileUrl);

        string[] lines = File.ReadAllLines(fileUrl);

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) //有空行,就continue
                continue;

            string[] kv = lines[i].Split('|');
            //kv =  scene1 / character.assetbundle | 2503fdd66fea3185fa83930bc972bad2
            string fileName = kv[0];

            // 再拿到本地的这个文件 (与下载下来的文件进行比对)
            string localFile = (downloadPath + "/" + fileName).Trim();

            // 如果没有本地文件 , 就下载
            if (!File.Exists(localFile))
            {
                Debug.Log("本地没有" + fileName + "文件,需要从服务器下载!");
                string dir = Path.GetDirectoryName(localFile);
                Directory.CreateDirectory(dir);

                // 开始网络下载
                //string tempUrl = url + fileName;
                //www = new WWW(tempUrl);
                //yield return www;
                //if (www.error != null)
                //    Debug.LogError("Error" + www.error);

                //File.WriteAllBytes(localFile, www.bytes);

                string tempUrl = url + "/" +  fileName;
                string tempText = File.ReadAllText(tempUrl);
                File.WriteAllText(localFile, tempText);
            }
            else //如果文件有的话 , 就开始比对MD5 ,检测是否更新了
            {
                string md5 = kv[1];
                string localMD5 = GetFileMD5(localFile);

                // 开始比对
                if (md5 == localMD5)
                {
                    Debug.Log("MD5文件相同,不需要从服务器下载更新!" + fileName);
                    // 相等的话,没更新
                  //  Debug.Log("没更新");
                }
                else
                {
                    Debug.Log("MD5文件不相同,需要从服务器下载更新!" + fileName);
                    // 不相等,说明更新了
                    // 删除本地原来的旧文件
                    File.Delete(localFile);

                    //下载新文件
                    //string tmpUrl = url + fileName;
                    //www = new WWW(tmpUrl);
                    //yield return www;
                    ////进行一些网络的检测
                    //if (www.error != null)
                    //    Debug.LogError("Error" + www.error);

                    //File.WriteAllBytes(localFile, www.bytes);

                    string tempUrl = url + "/" + fileName;
                    string tempText = File.ReadAllText(tempUrl);
                    File.WriteAllText(localFile, tempText);
                }
            }

        }

        yield return new WaitForEndOfFrame();

        Debug.Log("更新完成,可以开始游戏了");
    }

    /// <summary>
    /// 获取文件 MD5 值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private string GetFileMD5(string filePath)
    {
        FileStream fs = new FileStream(filePath, FileMode.Open);

        // 引入命名空间   using System.Security.Cryptography;
        MD5 md5 = new MD5CryptoServiceProvider();

        byte[] bt = md5.ComputeHash(fs);
        fs.Close();

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bt.Length; i++)
        {
            sb.Append(bt[i].ToString("x2"));
        }

        return sb.ToString();
    }



}
