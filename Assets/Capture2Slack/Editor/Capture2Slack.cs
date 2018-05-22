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
        private string backupDirPath = string.Empty;
        private string backupPath = string.Empty;
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

            fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            backupPath = backupDirPath + "/" + fileName + ".png";

            uploadData = new UploadData {
                token = SettingData.slackToken,
                title = SettingData.title,
                initial_comment = SettingData.defaultComment,
                filename = fileName,
                channels = SettingData.defaultChannelName
            };

#if UNITY_2017_1_OR_NEWER
            ScreenCapture.CaptureScreenshot(backupPath);
#else
            Application.CaptureScreenshot(backupPath);
#endif
            Debug.Log("[Capture to Slack] Export scrennshot at " + backupPath);

            while(true) {
                if(System.IO.File.Exists(backupPath)) {
                    break;
                }
                else {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            screenshot = new Texture2D(1, 1);
            byte[] bytes;
            bytes = System.IO.File.ReadAllBytes(backupPath);
            screenshot.LoadImage(bytes);

            uploadData.screenShot = screenshot;

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(SlackAPI.UploadScreenShot(uploadData, this.OnSuccess, this.OnError), this);

        }


            void OnSuccess() {
            Debug.Log("[Capture to Slack] Upload Success!! Check your slack");
        }

        void OnError(string _e) {
            Debug.LogError("[Capture to Slack] Upload FAIL!! Error message is... " + _e);
        }
    }
}
