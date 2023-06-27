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

            StartCoroutine("AvatarBlink");
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
        } else {
            Debug.LogError("VRM Avatar is not loaded");
        }
    }

    IEnumerator AvatarBlink()
    {
        while (true)
        {
            var expressionController = vrmAvatar.GetComponent<Vrm10Instance>().Runtime.Expression;

            yield return new WaitForSeconds(UnityEngine.Random.Range(_minBlinkTime, _maxBlinkTime));

            var value = 0f;
            var closedSpeed = 1.0f / _blinkEyeClosingSeconds;
            while (value < 1f)
            {
                expressionController.SetWeight(ExpressionKey.Blink, value);
                value += Time.deltaTime * closedSpeed;
                yield return null;
            }
            expressionController.SetWeight(ExpressionKey.Blink, 1);

            yield return new WaitForSeconds(_blinkEyeCloseDuration);

            value = 1f;
            var openSpeed = 1.0f / _blinkEyeOpeningSeconds;

            while (value > 0)
            {
                expressionController.SetWeight(ExpressionKey.Blink, value);
                value -= Time.deltaTime * openSpeed;
                yield return null;
            }
            expressionController.SetWeight(ExpressionKey.Blink, 0);
        }
    }
}
