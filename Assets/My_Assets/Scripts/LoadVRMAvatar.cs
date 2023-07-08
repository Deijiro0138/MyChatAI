using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniVRM10;
using uLipSync;

public class LoadVRMAvatar : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController _vrmAnimatorController;
    [SerializeField] Button _tohokuF01;
    [SerializeField] Button _mei;
    [SerializeField] Button _takumi;
    [SerializeField] uLipSync.Profile _maleProfile;
    [SerializeField] uLipSync.Profile _femaleProfile;

    public static string vrmName;
    public static GameObject vrmAvatar;
    public static List<string> vrmNameList = new List<string> {
        "tohoku-f01",
        "mei",
        "takumi"
    };


    private void Start()
    {
        _tohokuF01.onClick.AddListener(() => SelectVRMAvatar(vrmNameList[0]));
        _mei.onClick.AddListener(() => SelectVRMAvatar(vrmNameList[1]));
        _takumi.onClick.AddListener(() => SelectVRMAvatar(vrmNameList[2]));

    }

    public void SelectVRMAvatar(string name)
    {
        if (vrmAvatar != null)
        {
            Destroy(vrmAvatar);
        }

        var vrmPath = $"{Application.streamingAssetsPath}/{name}.vrm";
        vrmName = name;

        VRMLoadAsync(vrmPath);
        Debug.Log(vrmName);
    } 

    private async void VRMLoadAsync(string path)
    {
        try
        {
            var vrm10Instance = await Vrm10.LoadPathAsync(path);
            vrmAvatar = vrm10Instance.gameObject;

            Animator animator = vrmAvatar.GetComponent<Animator>();
            animator.runtimeAnimatorController = (RuntimeAnimatorController)
                RuntimeAnimatorController.Instantiate(_vrmAnimatorController);

            vrmAvatar.AddComponent<AudioSource>();

            ULipSyncInit();
        } catch (Exception e)
        {
            Debug.LogError("Failed to load");
            Debug.LogException(e);
            throw;
        }
    }

    private void ULipSyncInit()
    {
        var ulipSync = vrmAvatar.AddComponent<uLipSync.uLipSync>();
        var ulipSyncBlendShape = vrmAvatar.AddComponent<uLipSync.uLipSyncBlendShape>();

        ulipSync.profile = _femaleProfile;
        LipSyncInfo lipSyncInfo = new LipSyncInfo();
        {

        };
        //ulipSync.onLipSyncUpdategameObject.GetComponent<uLipSyncBlendShape>().OnLipSyncUpdate(lipSyncInfo);

    }
}
