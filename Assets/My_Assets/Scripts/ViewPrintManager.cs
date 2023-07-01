using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;
using AAA.Rinna;


public class ViewPrintManager: MonoBehaviour
{
    [SerializeField] string rinnaAIApiKey;
    [SerializeField] string openAIApiKey;
    [SerializeField] Image loadingIcon;
    [SerializeField] Text chatHistory;
    [SerializeField] Text emotionError;

    public InputField userComment;

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

    public class RinnaAPISettings
    {
        public int voiceID;
        public int emotionID;
        public float speakSpeed;
        public string message;
        public float volume;
        public string format;
    }

    private void Start()
    {
        loadingIcon.enabled = false;

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

        var responseChatGPT = await chatGPTConnection.RequestAsync(userComment, loadingIcon);
        var responseJsonData = responseChatGPT.choices[0].message.content;

        try
        {
            var responseData = JsonConvert.DeserializeObject<AvatarReaction>(responseJsonData);
            Emotion reactionEmotion = responseData.emotion;
            string reactionMessage = responseData.message;

            vrmBodyControl.AvatarFaceControl(reactionEmotion);

            chatHistory.text += $"ChatGPT:{reactionMessage}\n";

            var rinnaAPIConnection = new RinnaAPIConnection(rinnaAIApiKey);
            var rinnaAPISettings = new RinnaAPISettings
            {
                voiceID = 27,
                emotionID = FindMaxEmotion(reactionEmotion),
                speakSpeed = 1f,
                message = reactionMessage,
                volume = 10f,
                format = "wav"
            };

            var responseRinna = await rinnaAPIConnection.RequestAsync(rinnaAPISettings);

            await vrmBodyControl.SpeakResponseMessage(responseRinna.mediaContentUrl);
        }
        catch (JsonException e)
        {
            EmotionError();
            Debug.LogError("Deserialization" + e.Message);
        }

        userComment.interactable = true;
    }

    private int FindMaxEmotion(Emotion emotion)
    {
        float maxEmotion = Mathf.Max(emotion.happy, emotion.angry, emotion.sad, emotion.relaxed, emotion.surprised);

        if (maxEmotion == emotion.happy)
            return 2;
        else if (maxEmotion == emotion.angry)
            return 5;
        else if (maxEmotion == emotion.sad)
            return 3;
        else if (maxEmotion == emotion.surprised)
            return 7;
        else
            return 1; // Except four emotions (relaxed etc)
    }

    IEnumerator EmotionError()
    {
        emotionError.enabled = true;

        yield return new WaitForSeconds(3);

        emotionError.enabled = false;
    }
}
