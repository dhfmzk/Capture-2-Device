using System.IO;
using UnityEngine;
using UnityEditor;

namespace OrcaAssist {

    // Capture to Slack plugin setting singleton
    public class Capture2DeviceSetting : ScriptableObject {

        private const string LogTag = "[C2D Setting] {0}";

        [Header("Editor Setting")]
        public string BackupPath;

        [Header("Slack Setting")]
        public string SlackToken;
        public string SlackTitle;
        public string SlackChannelName;
        public string SlackComment;

        [Header("Telegram Setting")]
        public string TelegramToken;
        public string TelegramChatId;

        [Header("Discord Setting")]
        public string DiscordWebhookId;
        public string DiscordToken;
        public string DiscordChatId;

        [MenuItem("Assets/OrcaAssist/Set C2D Backup Path", false, 1101)]
        private static void SetBackupPath() {
            string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string fullPath = Application.dataPath + "/" + selectionPath.Replace("Assets/", "");
            InstanceC2D.BackupPath = fullPath;
        }

        [MenuItem("Assets/OrcaAssist/Set C2D Backup Path", true)]
        private static bool CheckMenu() {

            if(Selection.objects.Length > 1) {
                return false;
            }

            string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(selectionPath.Length == 0) {
                return false;
            }
            
            string fullPath = Application.dataPath + "/" + selectionPath.Replace("Assets/", "");
            FileAttributes fileAttr = File.GetAttributes(fullPath);

            return (fileAttr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public bool IsCompletedSlackData() {
            bool ret = true;

            if(string.IsNullOrEmpty(SlackToken)) {
                ret = false;
                Debug.LogError($"{LogTag} Slack Token is NULL or Empty!");
            }

            if(!Directory.Exists(BackupPath)) {
                ret = false;
                Debug.LogError($"{LogTag} Backup Path is Not Exists. Please write correct path!");
            }

            if(string.IsNullOrEmpty(SlackChannelName)) {
                ret = false;
                Debug.LogError($"{LogTag} Channel Name is NULL or Empty!");
            }

            return ret;
        }

        public bool IsCompletedTelegaramData() {
            bool ret = true;

            if(string.IsNullOrEmpty(TelegramToken)) {
                ret = false;
                Debug.LogError($"{LogTag} Telegram Token is NULL or Empty!");
            }

            if(!Directory.Exists(BackupPath)) {
                ret = false;
                Debug.LogError($"{LogTag} Backup Path is Not Exists. Please write correct path!");
            }

            if(string.IsNullOrEmpty(TelegramChatId)) {
                ret = false;
                Debug.LogError($"{LogTag} Chat ID is NULL or Empty!");
            }

            return ret;
        }

        public bool IsCompletedDiscordData() {
            bool ret = true;

            if(string.IsNullOrEmpty(DiscordToken)) {
                ret = false;
                Debug.LogError($"{LogTag} Discord Token is NULL or Empty!");
            }
            
            if(string.IsNullOrEmpty(DiscordWebhookId)) {
                ret = false;
                Debug.LogError($"{LogTag} Discord Webhook Id is NULL or Empty!");
            }

            if(!Directory.Exists(BackupPath)) {
                ret = false;
                Debug.LogError($"{LogTag} Backup Path is Not Exists. Please write correct path!");
            }

            return ret;
        }

        // Singleton Pattern
        private static Capture2DeviceSetting _instance = null;
        public static Capture2DeviceSetting InstanceC2D {
            get {
                if(!_instance) {
                    _instance = EditorGUIUtility.Load("C2D Setting.asset") as Capture2DeviceSetting;
                }
                return _instance;
            }
        }
    }

    [CustomEditor(typeof(Capture2DeviceSetting))]
    public class Capture2SlackSettingEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if(GUILayout.Button("Open Slack API Site")) {
                Application.OpenURL("https://api.slack.com/apps");
            }
        }
    }
}