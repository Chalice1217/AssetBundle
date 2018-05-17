using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
/// <summary>
/// 在游戏检测更新完之后开始调用
/// 参考xlua教程 / 加载Lua文件 / 自定义loader
/// </summary>
public class LuaManager : MonoBehaviour
{
    public static LuaManager Instance;

    LuaEnv luaEnv = new LuaEnv();

    private byte[] Bytes;

    private void Awake()
    {
        Instance = this;

        luaEnv.AddLoader(CustomLoader);
    }

    /// <summary>
    /// 调用Lua方法
    /// </summary>
    /// <param name="luaName"></param>
    /// <param name="mothedName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public object[] CallLuaFunction(string luaName,string mothedName , params object[] args)
    {

        LuaTable table = luaEnv.Global.Get<LuaTable>(luaName);

        LuaFunction func = table.Get<LuaFunction>(mothedName);

        return func.Call(args);

    }



    /// <summary>
    /// 执行lua代码
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="chunkName"></param>
    /// <param name="env"></param>
    public void DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
    {
        luaEnv.DoString(chunk,chunkName,env);
    }

    Dictionary<string, byte[]> luaDic = new Dictionary<string, byte[]>();

    private byte[] CustomLoader( ref string fileName)
    {
        Bytes = null;
        // 获取lua所在目录(一般在从服务器下载的AssetBundle目录下(我的是在Windows目录下))
        string luaPath = PathUtil.GetAssetBundleOutPath() + "/" + "Lua";
       
        // 也可以做一下优化,用一个字典存起来[空间换时间],但是lua很少会调用多次(基本上调用一次就可以),用不用字典存都可以
        if (luaDic.ContainsKey(fileName))
            return luaDic[fileName];

        ProcessDirectory(new DirectoryInfo(luaPath), fileName);

        return Bytes;
    }

    /// <summary>
    /// 遍历Lua文件下下所有文件系统
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private void  ProcessDirectory(FileSystemInfo fileSystemInfo,string fileName)
    {
        
        DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();

        foreach (FileSystemInfo item in fileSystemInfos)
        {
            FileInfo file = item as FileInfo;
            if (file == null)
                ProcessDirectory(item, fileName);
            else
            {
                // GameInit.lua => tempName = GameInit
                string tempName = item.Name.Split('.')[0];
                if (item.Extension == ".meta" || tempName != fileName)
                    continue;

                Bytes = File.ReadAllBytes(file.FullName);
                luaDic.Add(fileName, Bytes);
                
            }

        }

       
    }




}
