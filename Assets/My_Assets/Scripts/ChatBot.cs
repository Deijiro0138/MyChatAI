using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAA.OpenAI;

public class ChatBot : MonoBehaviour
{
    [SerializeField] string _openAIApiKey;
    // Start is called before the first frame update
    void Start()
    {
        var chatGPTConnection = new ChatGPTConnection(_openAIApiKey);
        chatGPTConnection.RequestAsync("�׋������āI");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
