using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UniVRM10;

public class VRMBodyControl : MonoBehaviour
{
    private LoadVRMAvatar loadVRMAvatar;
    private void Start()
    {
        loadVRMAvatar = new LoadVRMAvatar();
        
    }

    public async UniTask SpeakResponseMessage(string url)
    {
        var audioSource = LoadVRMAvatar.vrmAvatar.GetComponent<AudioSource>();
        var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);

        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var responseAudioClip = DownloadHandlerAudioClip.GetContent(www);

            audioSource.PlayOneShot(responseAudioClip);
        } else {
            throw new Exception();
        }
    }

    public void AvatarFaceControl(ViewPrintManager.Emotion emotion)
    {
        var vrm = LoadVRMAvatar.vrmAvatar;
        if (vrm != null)
        {
            var controller = vrm.GetComponent<Vrm10Instance>().Runtime.Expression;
            var facial = new Dictionary<ExpressionKey, float> {
                { ExpressionKey.Happy,     emotion.happy},
                { ExpressionKey.Angry,      emotion.angry},
                { ExpressionKey.Sad,        emotion.sad},
                { ExpressionKey.Relaxed,   emotion.relaxed},
                { ExpressionKey.Surprised, emotion.surprised},
             };
            controller.SetWeights(facial);
        }
        else
        {
            Debug.LogError("VRM Avatar is not loaded");
        }
    }
}
