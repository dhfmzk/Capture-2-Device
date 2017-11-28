using UnityEngine;
using UnityEditor;
using SlackHelper;
using System;
using System.Collections;

/// <summary>
/// Screenshot to slack plugin editor window class
/// Made by Orca
/// - blog : orcacode.tistory.com
/// </summary>
public class Capture2SlackWindow : EditorWindow {

    GUIStyle imageStyle = null;
    Texture2D logoImage = null;

    private string fileName = string.Empty;
    private string backupDirPath = string.Empty;
    private string backupPath = string.Empty;

    private string slackToken = string.Empty;
    private string slackChannel = string.Empty;
    private string slackComment = string.Empty;
    private UploadData uploadData = null;

    private Texture2D screenshot = null;

    // Open Plugin Window
    [MenuItem("OrcaTools/Screenshot to Slack")]
    private static void Init() {
        EditorWindow.GetWindow(typeof(Capture2SlackWindow)).Show();
    }

    void OnEnable() {

    }

    // Window interaction
    void OnGUI() {

        // 0. Title
        logoImage = Resources.Load("C2S Logo Icon", typeof(Texture2D)) as Texture2D;
        this.imageStyle = new GUIStyle(GUI.skin.label);
        imageStyle.alignment = TextAnchor.MiddleCenter;
        imageStyle.imagePosition = ImagePosition.ImageOnly;
        imageStyle.fixedHeight = 70f;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent(logoImage), this.imageStyle, GUILayout.Height(70.0f));
        EditorGUILayout.Space();


        // 1. Backup Path
        EditorGUILayout.LabelField("Screenshot Setting", EditorStyles.boldLabel);
        this.backupDirPath = EditorGUILayout.TextField("Backup Path :", backupDirPath);
        
        EditorGUILayout.BeginHorizontal();
        var slectOutputButton = GUILayout.Button("Select Backup Directory", GUILayout.Height(25));
        var openOutputButton = GUILayout.Button("Open Backup Directory", GUILayout.Height(25));
        EditorGUILayout.EndHorizontal();

        if(slectOutputButton) {
            string path = EditorUtility.OpenFolderPanel("Select Backup Directory", backupDirPath, Application.dataPath);
            if(!string.IsNullOrEmpty(path)) {
                backupDirPath = path;
            }
        }

        if(openOutputButton) {
            System.Diagnostics.Process.Start(backupDirPath);
        }

        EditorGUILayout.Space();


        // 2. Slack Token
        EditorGUILayout.LabelField("Slack Setting", EditorStyles.boldLabel);
        this.slackToken = EditorGUILayout.TextField("Token : ", this.slackToken);
        this.slackChannel = EditorGUILayout.TextField("Channel Name : ", this.slackChannel);
        this.slackComment = EditorGUILayout.TextField("Comment : ", this.slackComment);

        var openSlackButton = GUILayout.Button("Check slack token", GUILayout.Height(25));
        if(openSlackButton) {
            Application.OpenURL("https://api.slack.com/apps");
        }

        EditorGUILayout.Space();
        

        // 3. Take screenshot and send to slack
        var buttonImage = Resources.Load("Capture Button Icon", typeof(Texture2D)) as Texture2D;
        var captureButton = GUILayout.Button(buttonImage, GUILayout.Height(50));

        if(captureButton) {
            EditorCoroutines.EditorCoroutines.StartCoroutine(CaptureAndUpload(), this);
        }
    }

    IEnumerator CaptureAndUpload() {

        this.fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
        this.backupPath = backupDirPath + "/" + fileName + ".png";

        uploadData = new UploadData {
            token = this.slackToken,
            title = "Screenshot from Unity",
            initial_comment = this.slackComment,
            filename = fileName,
            channels = this.slackChannel
        };

#if UNITY_2017
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
