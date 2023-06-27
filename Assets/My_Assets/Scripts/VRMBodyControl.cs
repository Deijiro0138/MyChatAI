using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using JTalkDll;

public class VRMBodyControl : MonoBehaviour
{
    private LoadVRMAvatar loadVRMAvatar;
    private void Start()
    {
        loadVRMAvatar = new LoadVRMAvatar();
    }

    public async UniTask AvatarSpeakMessage(ViewPrintManager.Emotion emotion, string msg)
    {
        var voiceEmotion = CalculateVoiceEmotion(emotion);

        foreach(var avatar in LoadVRMAvatar.vrmNameList.Select((name, index) => new { name, index}))
        {
            if (LoadVRMAvatar.vrmName == avatar.name)
            {
                string voiceName;

                if (avatar.name == "tohoku-f01")
                {
                    voiceName = avatar.name + "-";
                    if (voiceEmotion == "normal")
                    {
                        voiceName += "neutral";
                    } else {
                        voiceName += voiceEmotion;
                    }
                } else {
                    voiceName = avatar.name + "_" + voiceEmotion;
                }

                await UniTask.Run(() => OpenJTalk.Speak(msg, voiceName));
                break;
            }
        }
    }

    private string CalculateVoiceEmotion(ViewPrintManager.Emotion emotion)
    {
        var emotionVoice = new List<string>
        {
            "angry",
            "happy",
            "normal",
            "sad"
        };

        var weights = new List<float>
        {
            0.3f,
            0.2f,
            0.1f,
            0.4f
        };

        var scores = new List<float>()
        {
            emotion.angry   * weights[0],
            emotion.happy  * weights[1],
            emotion.relaxed * weights[2],
            emotion.sad      * weights[3]
        };

        var maxScore = scores.Max();
        var maxIndex = scores.IndexOf(maxScore);

        return emotionVoice[maxIndex];

    }
}
