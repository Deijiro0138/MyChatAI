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
    [SerializeField] Text emotionError;

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

        try
        {
            var responseData = JsonConvert.DeserializeObject<AvatarReaction>(responseJsonData);
            Emotion reactionEmotion = responseData.emotion;
            string reactionMessage = responseData.message;

            loadVRMAvatar.AvatarFaceControl(reactionEmotion);
            chatHistory.text += $"ChatGPT:{reactionMessage}\n";

            await vrmBodyControl.AvatarSpeakMessage(reactionEmotion, reactionMessage);
        }
        catch (JsonException e)
        {
            EmotionError();
            Debug.LogError("Deserialization" + e.Message);
        }

        userComment.interactable = true;
    }
    IEnumerator EmotionError()
    {
        emotionError.enabled = true;

        yield return new WaitForSeconds(3);

        emotionError.enabled = false;
    }
}
