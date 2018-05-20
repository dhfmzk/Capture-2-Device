using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace OrcaAssist {

    /// <summary>
    /// Screenshot to slack plugin editor window class
    /// Made by Orca
    /// - blog : orcacode.tistory.com
    /// </summary>
    public class Capture2SlackWindow : EditorWindow {

        private Capture2Slack C2S;
        // Open Plugin Window
        [MenuItem("OrcaAssist/TA/Capture2Slack")]
        private static void Init() {
            EditorWindow.GetWindow<Capture2SlackWindow>(false, "Orca C2S", true).Show();
        }

        private Texture titleIcon;
        void OnEnable() {
            titleIcon = EditorGUIUtility.Load("C2S Title Icon 16x16.png") as Texture;
            this.titleContent.image = titleIcon;
            this.titleContent.text = "Orca C2S";
        }

        // Window interaction
        void OnGUI() {

            // 0. icon

            // Button
        }
        
    }
}