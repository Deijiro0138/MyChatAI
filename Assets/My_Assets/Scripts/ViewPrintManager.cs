using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;


public class ViewPrintManager: MonoBehaviour
{
    [SerializeField] string openAIApiKey;
    [SerializeField] Image loadingIcon;
    [SerializeField] Text chatHistory;

    public InputField userComment;

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

    private void Start()
    {
        loadingIcon.enabled = false;

        loadVRMAvatar = new LoadVRMAvatar();
        vrmBodyControl = new VRMBodyControl();

        userComment = userComment.GetComponent<InputField>();
        chatHistory = chatHistory.GetComponent<Text>();
    }

    private void Update()
    {
        if (LoadVRMAvatar.vrmName == null)
        {
            userComment.interactable = false;
        } else
        {
            userComment.interactable = true;
        }
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

        await vrmBodyControl.AvatarSpeakMessage(reactionEmotion, reactionMessage);

        userComment.interactable = true;

    }

}
