using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace OrcaAssist {

    // Capture to Slack plugin setting singleton
    public class Capture2SlackSetting : ScriptableObject {

        [Header("Editor Setting")]
        public string backupPath;

        [Header("Slack Setting")]
        public string slackToken;
        public string title;
        public string channelName;
        public string comment;

        [MenuItem("Assets/OrcaAssist/Set C2D Backup Path", false, 1101)]
        private static void SetBackupPath() {
            string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string fullPath = Application.dataPath + "/" + selectionPath.Replace("Assets/", "");
            InstanceC2S.backupPath = fullPath;
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
            FileAttributes file_attr = File.GetAttributes(fullPath);
            if((file_attr & FileAttributes.Directory) != FileAttributes.Directory) {
                return false;
            }

            return true;
        }

        public bool IsCompletedData() {
            bool ret = true;

            if(string.IsNullOrEmpty(slackToken)) {
                ret = false;
                Debug.LogError("[C2D SYSTEM] Slack Token is NULL or Empty!");
            }

            if(!Directory.Exists(backupPath)) {
                ret = false;
                Debug.LogError("[C2D SYSTEM] Backup Path is Not Exists. Please write correct path!");
            }

            if(string.IsNullOrEmpty(channelName)) {
                ret = false;
                Debug.LogError("[C2D SYSTEM] Channel Name is NULL or Empty!");
            }

            return ret;
        }

        // Singleton Pattern
        private static Capture2SlackSetting _instance = null;
        public static Capture2SlackSetting InstanceC2S {
            get {
                if(!_instance) {
                    _instance = EditorGUIUtility.Load("C2D Setting.asset") as Capture2SlackSetting;
                }
                return _instance;
            }
        }
    }

    [CustomEditor(typeof(Capture2SlackSetting))]
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