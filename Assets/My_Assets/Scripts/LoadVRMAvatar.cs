using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniVRM10;

public class LoadVRMAvatar : MonoBehaviour
{
    [SerializeField] Vector3 vrmRespawnPosition;
    [SerializeField] List<string> vrmName = new List<string>();

    public static GameObject vrmAvatar;
    // Start is called before the first frame update
    private void Start()
    {
        string vrmPath = $"{Application.streamingAssetsPath}/{vrmName[0]}.vrm";
        VRMLoadAsync(vrmPath);
    }

    private async void VRMLoadAsync(string path)
    {
        try
        {
            var vrm10Instance = await Vrm10.LoadPathAsync(path);
            vrmAvatar = vrm10Instance.gameObject;

            vrmAvatar.transform.Rotate(vrmRespawnPosition);
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
