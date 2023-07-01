using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniVRM10;

public class LoadVRMAvatar : MonoBehaviour
{
    [SerializeField] int _minBlinkTime;
    [SerializeField] int _maxBlinkTime;
    [SerializeField] float _blinkEyeCloseDuration = 0.06f;
    [SerializeField] float _blinkEyeOpeningSeconds = 0.03f;
    [SerializeField] float _blinkEyeClosingSeconds = 0.1f;
    [SerializeField] RuntimeAnimatorController _vrmAnimatorController;
    [SerializeField] Button _tohokuF01;
    [SerializeField] Button _mei;
    [SerializeField] Button _takumi;

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
        } catch (Exception e)
        {
            Debug.LogError("Failed to load");
            Debug.LogException(e);
            throw;
        }
    }
}
