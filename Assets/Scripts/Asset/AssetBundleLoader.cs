using System.Collections;
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
    /// <returns>www.assetBundle</returns>
    public IEnumerator Load()
    {
        // 注 : 如果发布到手机端,就不需要www加载了,通过下面方式加载:
        // AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(bundlePath);
        //  yield return abcr;


        www = new WWW(bundlePath);
        while (!www.isDone)
        {
            this.progress = www.progress;
            // 每一帧都调用 , 用来更新加载进度
            if (lp != null)           
                lp(bundleName, progress);
            
            yield return www;
        }

        this. progress = www.progress;

        // 加载完成了
        if (progress >= 1f)
        {           
            // 上面两行代码,用下面的构造方法一行代码即可
            assetLoader = new AssetLoader(www.assetBundle);

            // 每一帧都调用 , 用来更新加载进度
            if (lp != null)
                lp(bundleName, progress);

            if (lc != null)          
                lc(bundleName);           
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
            Debug.LogError("当前assetLoader为空!" );
            return null;
        }

        return assetLoader.LoadAsset(assetName);
    }

    /// <summary>
    /// 获取包里的所有资源
    /// </summary>
    /// <returns>obj类型的数组</returns>
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
            Debug.LogError("当前assetLoader为空!");
            return null;
        }

        return assetLoader.LoadAssetWithSubAssets(assetName);
    }

    /// <summary>
    /// 卸载资源[Object类型]
    /// </summary>
    /// <param name="asset">obj</param>
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
