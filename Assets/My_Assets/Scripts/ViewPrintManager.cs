using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;


public class ViewPrintManager: MonoBehaviour
{
    [SerializeField] string openAIApiKey;
    [SerializeField] InputField userComment;
    [SerializeField] Image loadingIcon;
    [SerializeField] Text chatHistory;

    private LoadVRMAvatar loadVRMAvatar;
    private VRMBodyControl vrmBodyControl;


    public class AvatarReaction
    {
        public Emotion emotion;
        public string message;
    }

    public class Emotion
    {
        public float happy;
        public float angry;
        public float sad;
        public float relaxed;
        public float surprised;
    }

    void Start()
    {
        loadingIcon.enabled = false;

        loadVRMAvatar = new LoadVRMAvatar();
        vrmBodyControl = new VRMBodyControl();

        userComment = userComment.GetComponent<InputField>();
        chatHistory = chatHistory.GetComponent<Text>();
    }

    public async void SendMessageToChatGPT()
    {
        var chatGPTConnection = new ChatGPTConnection(openAIApiKey);

        chatHistory.text += $"Ž©•ª:{userComment.text}\n";

        var response = await chatGPTConnection.RequestAsync(userComment, loadingIcon);
        var responseJsonData = response.choices[0].message.content;
        var responseData = JsonConvert.DeserializeObject<AvatarReaction>(responseJsonData);
        string reactionMessage = responseData.message;
        Emotion reactionEmotion = responseData.emotion;

        loadVRMAvatar.AvatarFaceControl(reactionEmotion);
        chatHistory.text += $"ChatGPT:{reactionMessage}\n";
        vrmBodyControl.AvatarSpeakMessage(reactionEmotion, reactionMessage);

    }

}
