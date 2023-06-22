using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;
public class ViewPrintManager: MonoBehaviour
{
    [SerializeField] string openAIApiKey;
    [SerializeField] InputField userComment;
    [SerializeField] Image loadingIcon;
    [SerializeField] Text chatHistory;

    void Start()
    {
        userComment = userComment.GetComponent<InputField>();
        chatHistory = chatHistory.GetComponent<Text>();
    }

    public void SendMessageToChatGPT()
    {
        var chatGPTConnection = new ChatGPTConnection(openAIApiKey);
        chatHistory.text += $"Ž©•ª:{userComment.text}\n";
        chatGPTConnection.RequestAsync(userComment, loadingIcon, chatHistory);
    }

    public void StartLoading()
    {

    }
}
