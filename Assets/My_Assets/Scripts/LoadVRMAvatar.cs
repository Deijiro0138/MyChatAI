using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniVRM10;

public class LoadVRMAvatar : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController vrmAnimatorController;

    public static string vrmName = "tohoku-f01";
    public static GameObject vrmAvatar;
    public static List<string> vrmNameList = new List<string> {
        "tohoku-f01",
        "mei",
        "takumi"
    };

    private void Start()
    {
        string vrmPath = $"{Application.streamingAssetsPath}/{vrmName}.vrm";
        VRMLoadAsync(vrmPath);
    }

    private async void VRMLoadAsync(string path)
    {
        try
        {
            var vrm10Instance = await Vrm10.LoadPathAsync(path);
            vrmAvatar = vrm10Instance.gameObject;

            Animator animator = vrmAvatar.GetComponent<Animator>();
            animator.runtimeAnimatorController = (RuntimeAnimatorController)
                RuntimeAnimatorController.Instantiate(vrmAnimatorController);
        } catch (Exception e)
        {
            Debug.LogError("Failed to load");
            Debug.LogException(e);
            throw;
        }
    }

    public void AvatarFaceControl(ViewPrintManager.Emotion emotion)
    {
        if (vrmAvatar != null)
        {
            var controller = vrmAvatar.GetComponent<Vrm10Instance>().Runtime.Expression;
            var facial = new Dictionary<ExpressionKey, float> {
                { ExpressionKey.Happy,     emotion.happy},
                { ExpressionKey.Angry,      emotion.angry},
                { ExpressionKey.Sad,        emotion.sad},
                { ExpressionKey.Relaxed,   emotion.relaxed},
                { ExpressionKey.Surprised, emotion.surprised},
             };
             controller.SetWeights(facial);
        } else
        {
            Debug.LogError("VRM Avatar is not loaded");
        }
    }
}
