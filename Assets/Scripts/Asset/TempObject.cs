using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 06
/// 对Obj的封装
/// </summary>
public class TempObject
{
    /// <summary>
    /// 资源列表
    /// </summary>
    private List<Object> assetList;

    public List<Object> AssetList
    {
        get { return assetList; }
    }


    public TempObject(params Object[] assets)
    {
        assetList = new List<Object>(assets);
    }

 
     /// <summary>
     /// 卸载资源
     /// </summary>
    public void UnLoadAsset()
    {
        for (int i = assetList.Count-1; i >=0 ; i--)
        {
            Resources.UnloadAsset(assetList[i]);
        }
    }

}
