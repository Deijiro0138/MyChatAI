using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace AAA.OpenAI
{
    [Serializable]
    public class ChatGPTMessageModel
    {
        public string role;
        public string content;
    }

    //ChatGPT APIにRequestを送るためのJSON用クラス
    [Serializable]
    public class ChatGPTCompletionRequestModel
    {
        public string model;
        public List<ChatGPTMessageModel> messages;
    }

    //ChatGPT APIからのResponseを受け取るためのクラス
    [System.Serializable]
    public class ChatGPTResponseModel
    {
        public string id;
        public string @object;
        public int created;
        public Choice[] choices;
        public Usage usage;

        [System.Serializable]
        public class Choice
        {
            public int index;
            public ChatGPTMessageModel message;
            public string finish_reason;
        }

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }

    public class ChatGPTConnection
    {
        private readonly string _apiKey;
        //会話履歴を保持するリスト
        private readonly List<ChatGPTMessageModel> _messageList = new();

        public ChatGPTConnection(string apiKey)
        {
            _apiKey = apiKey;
            _messageList.Add(
                new ChatGPTMessageModel() {
                    role = "system",
                    content = @"以下の条件に従って、疑似的な感情をもつチャットボットとしてロールプレイをします。
                                      以後の会話では、あなたは下記の4つの感情パラメーターを持つかのように、振る舞うものとします。
                                      各感情パラメーターは会話を通じて変動するものとします。
                                      現在の感情パラメーターの値を反映するように、あなたの返答のトーンや発言は変化します。
                                      以後の会話ではまず現在の感情パラメータを出力し、その後に会話を出力してください。
                                      出力形式は以下のjsonフォーマットとします。このフォーマット以外で会話しないでください。
                                      {
                                            emotion: {
                                                  happy     : 0.0~1.0,
                                                  angry     : 0.0~1.0,
                                                  sad        : 0.0~1.0,
                                                  relaxed   : 0.0~1.0,
                                                  surprised : 0.0~1.0,
                                            }
                                            message: ""会話の文章""
                                      } "
                });
        }

        public async UniTask<ChatGPTResponseModel> RequestAsync(InputField userComment, Image lodingIcon)
        {
            string userMessage = userComment.text;
            //文章生成AIのAPIのエンドポイントを設定
            var apiUrl = "https://api.openai.com/v1/chat/completions";

            _messageList.Add(new ChatGPTMessageModel { role = "user", content = userMessage });

            //OpenAIのAPIリクエストに必要なヘッダー情報を設定
            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + _apiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };

            //文章生成で利用するモデルやトークン上限、プロンプトをオプションに設定
            var options = new ChatGPTCompletionRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = _messageList
            };
            var jsonOptions = JsonUtility.ToJson(options);

            Debug.Log("自分:" + userMessage);

            //OpenAIの文章生成(Completion)にAPIリクエストを送り、結果を変数に格納
            using var request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            lodingIcon.enabled = true;
            userComment.interactable = false;

            await request.SendWebRequest().ToUniTask(Progress.Create<float>(progress =>
            {
            }));

            lodingIcon.enabled = false;
            userComment.interactable = true;
            
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);

                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);

                Debug.Log("ChatGPT:" + responseObject.choices[0].message.content);

                _messageList.Add(responseObject.choices[0].message);

                return responseObject;
            }
        }
    }
}