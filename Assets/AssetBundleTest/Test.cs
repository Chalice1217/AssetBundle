using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
      //  AssetBundleManager.Instance.LoadAssetBundle("Scene1", "Character", LoadProgress);
      
    }

    void LoadProgress(string bundleName,float progress)
    {
        Debug.Log(progress + "---" + bundleName);
        if (progress >=1)     
           Instantiate( AssetBundleManager.Instance.LoadAsset("Scene1", "Character", "Player2"));        
    }



}
