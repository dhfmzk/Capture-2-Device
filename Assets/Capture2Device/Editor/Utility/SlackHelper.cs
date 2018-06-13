using System;
using System.Collections;
using UnityEngine;

namespace SlackHelper {
    
    [Serializable]
    public class UploadData {
        public string Token = string.Empty;
        public string Filename = string.Empty;
        public string Title = string.Empty;
        public string InitialComment = string.Empty;
        public string Channels = string.Empty;
        public Texture2D ScreenShot = null;
    }

    public static class SlackHelper {

        private const string BaseUrl = "https://slack.com/api/{0}";

        public static IEnumerator UploadScreenShot(UploadData data, Action onSuccess = null, Action<string> onError = null) {
            yield return new WaitForSeconds(0.1f);
         
            WWWForm form = new WWWForm();
            byte[] contents = data.ScreenShot.EncodeToPNG();

            form.AddField("token", data.Token);
            form.AddField("title", data.Title);
            form.AddField("initial_comment", data.InitialComment);
            form.AddField("channels", data.Channels);

            form.AddBinaryData("file", contents, data.Filename, "image/png");
            
            WWW www = new WWW(string.Format(BaseUrl, "files.upload"), form);
            yield return www;
            string error = www.error;

            if(!string.IsNullOrEmpty(error)) {
                if(onError != null) {
                    onError(error);
                }
                yield break;
            }

            if(onSuccess != null) {
                onSuccess();
            }
        }
    }
}
