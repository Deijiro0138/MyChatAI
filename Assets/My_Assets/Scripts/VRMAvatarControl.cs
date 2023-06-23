using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniVRM10;

public class VRMAvatarControl : MonoBehaviour
{
    [SerializeField] Vector3 vrmRespawnPosition;
    [SerializeField] List<string> vrmName = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        string vrmPath = $"{Application.streamingAssetsPath}/{vrmName[0]}.vrm";
        VRMLoadAsync(vrmPath);
    }

    private async void VRMLoadAsync(string path)
    {
        try
        {
            var vrm10Instance = await Vrm10.LoadPathAsync(path);
            GameObject vrm = vrm10Instance.gameObject;

            vrm.transform.Rotate(vrmRespawnPosition);
        } catch (Exception e)
        {
            Debug.LogError("Failed to load");
            Debug.LogException(e);
            throw;
        }
    }
}
