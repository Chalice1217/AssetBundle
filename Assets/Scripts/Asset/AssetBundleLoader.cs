using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 02
/// 加载AssetBundle
/// </summary>
public class AssetBundleLoader
{
    /// <summary>
    /// 包里的资源[01对象]{上层定义一个下层的对象,来传递}
    /// </summary>
    private AssetLoader assetLoader;

    /// <summary>
    /// www对象
    /// </summary>
    private WWW www;

    /// <summary>
    /// 包名
    /// </summary>
    private string bundleName;

    /// <summary>
    /// 路径
    /// </summary>
    private string bundlePath;

    /// <summary>
    /// 下载进度
    /// </summary>
    private float progress;

    /// <summary>
    /// 加载进度回调
    /// </summary>
    private LoadProgress lp;

    /// <summary>
    /// 加载完成回调
    /// </summary>
    private LoadComplete lc;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="lc"></param>
    /// <param name="lp"></param>
    /// <param name="bundleName"></param>
    public AssetBundleLoader(LoadComplete lc, LoadProgress lp, string bundleName)
    {
        this.lc = lc;
        this.lp = lp;
        this.bundleName = bundleName;
        this.progress = 0;
        // TODO
        this.bundlePath = PathUtil.GetWWWPath() + "/" + bundleName;
        this.www = null;
        this.assetLoader = null;

    }


    /// <summary>
    /// 加载资源包
    /// </summary>
    /// <returns></returns>
    public IEnumerator Load()
    {
        www = new WWW(bundlePath);
        while (www.isDone)
        {
            progress = www.progress;
            // 每一帧都调用 , 用来更新加载进度
            if (lp != null)
            {
                lp(bundleName, progress);
            }
            yield return www;
        }

        progress = www.progress;

        // 加载完成了
        if (progress >= 1f)
        {
            //assetLoader = new AssetLoader();
            //assetLoader.AssetBundle = www.assetBundle;
            // 上面两行代码,用下面的构造方法一行代码即可
            assetLoader = new AssetLoader(www.assetBundle);
            if (lc != null)
            {
                lc(bundleName);
            }
        }
    }


    /// <summary>
    /// 获取单个资源
    /// </summary>
    /// <param name="assetName">资源名字</param>
    /// <returns>Obj类型的资源</returns>
    public Object LoadAsset(string assetName)
    {
        if (assetLoader == null)
        {
            Debug.LogError("当前assetLoader为空!" + assetName);
            return null;
        }

        return assetLoader.LoadAsset(assetName);
    }

    /// <summary>
    /// 获取包里的所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets()
    {
        if (assetLoader == null)
        {
            Debug.LogError("当前assetLoader为空!");
            return null;
        }
        return assetLoader.LoadAllAssets();
    }

    /// <summary>
    /// 获取带有子物体的资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string assetName)
    {
        if (assetLoader == null)
        {
            Debug.LogError("当前assetLoader为空!" + assetName);
            return null;
        }

        return assetLoader.LoadAssetWithSubAssets(assetName);
    }

    /// <summary>
    /// 卸载资源[Object类型]
    /// </summary>
    /// <param name="asset"></param>
    public void UnloadAsset(Object asset)
    {
        if (assetLoader == null)
        {
            Debug.LogError("当前assetLoader为空!");
        }
        else
            assetLoader.UnloadAsset(asset);
    }

    /// <summary>
    /// 释放资源包
    /// </summary>
    public void Dispose()
    {
        if (assetLoader == null)
            return;

        assetLoader.Dispose();
        // false : 只卸载包
        // true  : 卸载包和obj
        assetLoader = null; //释放资源后,把assetLoader置空
    }


    /// <summary>
    /// 调试专用
    /// </summary>
    public void GetAllAssetNames()
    {
        assetLoader.GetAllAssetNames();
    }



}
