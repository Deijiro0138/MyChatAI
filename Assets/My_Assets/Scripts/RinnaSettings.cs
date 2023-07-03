using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace AAA.Rinna
{
    public class RinnaAPIConnection
    {
        private readonly string _subscriptionKey;

        public RinnaAPIConnection(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
        }

        public async UniTask<RinnaAPIResponseModel> RequestAsync(ViewPrintManager.RinnaAPISettings settings)
        {
            var apiUrl = "https://api.rinna.co.jp/models/cttse/v2";

            var options = new RinnaAPIRequestModel()
            {
                sid = settings.voiceID,
                tid = settings.emotionID,
                speed = settings.speakSpeed,
                text = settings.message,
                volume = settings.volume,
                format = settings.format
            };

            var jsonOptions = JsonUtility.ToJson(options);
            var postData = Encoding.UTF8.GetBytes(jsonOptions);

            using var request = new UnityWebRequest(apiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(postData),
                downloadHandler = new DownloadHandlerBuffer()
            };

            var headers = new Dictionary<string, string>
            {
                { "Content-type", "application/json"},
                { "Cache-Control", "no-cache"},
                { "Ocp-Apim-Subscription-Key", _subscriptionKey}
            };

            foreach(var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception();
            } else {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<RinnaAPIResponseModel>(responseString);

                return responseObject;
            }
        }
    }

    [System.Serializable]
    public class RinnaAPIRequestModel
    {
        public int sid;
        public int tid;
        public double speed;
        public string text;
        public double volume;
        public string format;
    }

    [System.Serializable]
    public class RinnaAPIResponseModel
    {
        public string mediaContentUrl;
        public string type;
    }

}

