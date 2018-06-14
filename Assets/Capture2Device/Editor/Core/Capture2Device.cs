using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace OrcaAssist {

    public class Capture2Device {

        private const string LogTag = "[Capture to Device] {0}";

        private string _fileName = string.Empty;
        private Texture2D _screenshot = null;

        private static Capture2Device _instance = null;
        public static Capture2Device Instance {
            get { return _instance ?? (_instance = new Capture2Device()); }
        }
        
        public Capture2DeviceSetting SettingData {
            get {
                return Capture2DeviceSetting.InstanceC2D;
            }
        }
        
        [MenuItem("OrcaAssist/Capture 2 Device/to Slack", false, 100)]
        public static void ToSlack() {
            EditorCoroutines.EditorCoroutines.StartCoroutine(Instance.UploadToSlack(), Instance);
        }

        [MenuItem("OrcaAssist/Capture 2 Device/to Telegram", false, 101)]
        public static void ToTelegram() {
            EditorCoroutines.EditorCoroutines.StartCoroutine(Instance.UploadToTelegram(), Instance);
        }

        [MenuItem("OrcaAssist/Capture 2 Device/Setting", false, 200)]
        public static void FocusSettingFile() {
            EditorGUIUtility.PingObject(Instance.SettingData);
        }
        
        private IEnumerator UploadToSlack() {
            
            if(!SettingData.IsCompletedSlackData()) {
                yield break;
            }

            _fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = SettingData.BackupPath + "/" + _fileName + ".png";

            SlackHelper.UploadData uploadData = new SlackHelper.UploadData {
                Token = SettingData.SlackToken,
                Title = SettingData.SlackTitle,
                InitialComment = SettingData.SlackComment,
                Filename = _fileName,
                Channels = SettingData.SlackChannelName,
            };

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureGameView(filePath), this);
            
            _screenshot = new Texture2D(1, 1);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            _screenshot.LoadImage(bytes);
            uploadData.ScreenShot = _screenshot;
            
            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(SlackHelper.SlackHelper.UploadScreenShot(uploadData, OnSuccess, OnError), this);

        }

        private IEnumerator UploadToTelegram() {

            if(!SettingData.IsCompletedTelegaramData()) {
                yield break;
            }
            
            _fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = SettingData.BackupPath + "/" + _fileName + ".png";

            TelegramHelper.UploadData uploadData = new TelegramHelper.UploadData {
                Token = SettingData.TelegramToken,
                FileName = _fileName,
                ChatId = SettingData.TelegramToken,
            };

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureGameView(filePath), this);

            _screenshot = new Texture2D(1, 1);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            _screenshot.LoadImage(bytes);
            uploadData.ScreenShot = _screenshot;

            yield return EditorCoroutines.EditorCoroutines.StartCoroutine(TelegramHelper.TelegramHelper.UploadScreenShot(uploadData, OnSuccess, OnError), this);

        }

        private static IEnumerator CaptureGameView(string filePath) {

            // 1. Caputre Game View
#if UNITY_2017_1_OR_NEWER
            ScreenCapture.CaptureScreenshot(filePath);
#else
            Application.CaptureScreenshot(filePath);
#endif
            Debug.Log(string.Format(LogTag, "Export scrennshot at " + filePath));

            // 2. Wait file write complete
            while(true) {
                if(System.IO.File.Exists(filePath)) {
                    break;
                }
                else {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            yield return null;
        }
        
        private static void OnSuccess() {
            Debug.Log(string.Format(LogTag, "Upload Success!! Check your slack"));
        }

        private static void OnError(string e) {
            Debug.LogError(string.Format(LogTag, "Upload FAIL!! Error message is... " + e));
        }
    }
}
