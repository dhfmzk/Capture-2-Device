using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

using SlackHelper;

namespace OrcaAssist {

    public class Capture2Slack {

        // ---------------------------------------------------------------------------
        // Data
        // ---------------------------------------------------------------------------
        private string fileName = string.Empty;
        private UploadData uploadData = null;

        private Texture2D screenshot = null;

        private static Capture2Slack _instance = null;
        public static Capture2Slack Instance {
            get {
                if(_instance == null) {
                    _instance = new Capture2Slack();
                }
                return _instance;
            }
        }
        
        public Capture2SlackSetting SettingData {
            get {
                return Capture2SlackSetting.InstanceC2S;
            }
        }

        // ---------------------------------------------------------------------------
        // Public Method
        // ---------------------------------------------------------------------------
        [MenuItem("OrcaAssist/Capture 2 Slack/to Slack", false, 100)]
        public static void ToSlack() {
            EditorCoroutines.EditorCoroutines.StartCoroutine(Instance.UploadToSlack(), Instance);
        }

        [MenuItem("OrcaAssist/Capture 2 Slack/Setting", false, 200)]
        public static void FocusSettingFile() {
            // Focussing Singleton setting
            EditorGUIUtility.PingObject(Instance.SettingData);
        }

        // ---------------------------------------------------------------------------
        // Upload Worker
        // ---------------------------------------------------------------------------
        private IEnumerator UploadToSlack() {

            if(!SettingData.IsCompletedData()) {
                yield break;
            }

            // 0. Get file name
            fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = SettingData.backupPath + "/" + fileName + ".png";

            // 1. Start Capture
            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureGameView(filePath), this);

            // 2. Get image file
            screenshot = new Texture2D(1, 1);
            byte[] bytes;
            bytes = System.IO.File.ReadAllBytes(filePath);
            screenshot.LoadImage(bytes);

            uploadData.screenShot = screenshot;

            // 3. Start Upload
            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(SlackAPI.UploadScreenShot(uploadData, this.OnSuccess, this.OnError), this);

        }

        private IEnumerator CaptureGameView(string _filePath) {

            // 0. Make Upload Data
            uploadData = new UploadData {
                token = SettingData.slackToken,
                title = SettingData.title,
                initial_comment = SettingData.comment,
                filename = fileName,
                channels = SettingData.channelName
            };

            // 1. Caputre Game View
#if UNITY_2017_1_OR_NEWER
            ScreenCapture.CaptureScreenshot(_filePath);
#else
            Application.CaptureScreenshot(_filePath);
#endif
            Debug.Log("[Capture to Slack] Export scrennshot at " + _filePath);

            // 2. Wait file write complete
            while(true) {
                if(System.IO.File.Exists(_filePath)) {
                    break;
                }
                else {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            yield return null;
        }
        
        // ---------------------------------------------------------------------------
        // Callback Functions
        // ---------------------------------------------------------------------------
        void OnSuccess() {
            Debug.Log("[Capture to Slack] Upload Success!! Check your slack");
        }

        void OnError(string _e) {
            Debug.LogError("[Capture to Slack] Upload FAIL!! Error message is... " + _e);
        }
    }
}
