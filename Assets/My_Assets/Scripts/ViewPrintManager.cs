using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AAA.OpenAI;
public class ViewPrintManager: MonoBehaviour
{
    [SerializeField] string _openAIApiKey;
    [SerializeField] InputField _inputField;
    [SerializeField] Image _circleLoading;
    [SerializeField] Text _response;

    void Start()
    {
        _inputField = _inputField.GetComponent<InputField>();
        _response = _response.GetComponent<Text>();
    }

    public void SendMessageToChatGPT()
    {
        var chatGPTConnection = new ChatGPTConnection(_openAIApiKey);
        chatGPTConnection.RequestAsync(_inputField.text);
    }

    public void StartLoading()
    {

    }
}
