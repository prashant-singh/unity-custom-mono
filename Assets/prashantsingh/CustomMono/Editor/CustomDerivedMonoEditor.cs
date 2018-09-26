using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Prashant
{
    public class CustomDerivedMonoEditor : EditorWindow
    {
        static GUIStyle styleHelpboxInner;
        static GUIStyle titleLabel, normalLabel, subtitleLabel;
        static List<string> m_baseClasses;

        static string m_path = "/prashantsingh/CustomMono/baseclassnames.txt";
        string m_baseClassName = "";
        int m_index = 0;
        const string m_baseClassKey = "baseClassKey";
        static EditorWindow window;
        [MenuItem("Assets/Create/Scripts/Drevied C# Script", false, 0)]
        // [MenuItem("[Master_Tools]/CustomMono")]
        private static void ShowWindow()
        {
            // LoadSettings();
            LoadBaseClasses();
            window = (CustomDerivedMonoEditor)EditorWindow.GetWindow(typeof(CustomDerivedMonoEditor), true, "Mono Settings");
            window.minSize = new Vector2(600, 450);
            window.Show();
        }
        // static void LoadSettings()
        // {
        //     customNameSpaceName = EditorPrefs.GetString("MyNamespace", "GameName");
        // }

        static void SetupStyle()
        {

            normalLabel = new GUIStyle();
            normalLabel.fontSize = 11;
            normalLabel.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            // normalLabel.normal.textColor = Color.white;
            normalLabel.fixedHeight = 25;
            normalLabel.alignment = TextAnchor.MiddleCenter;

            styleHelpboxInner = new GUIStyle("HelpBox");
            styleHelpboxInner.padding = new RectOffset(6, 6, 6, 6);
        }

        static void LoadBaseClasses()
        {
            var m_data = File.ReadAllText(Application.dataPath + m_path);
            if (!string.IsNullOrEmpty(m_data))
            {
                m_baseClasses = new List<string>(m_data.Split(','));
                // Debug.Log(m_baseClasses.Count + " data " + m_baseClasses[1]);
                m_popupArrayContents = m_baseClasses.ToArray();
            }
            else
            {
                m_baseClasses = new List<string>();
                SaveBaseClasses();
            }
        }

        static void SaveBaseClasses()
        {
            var temp_data = "";
            for (int count = 0; count < m_baseClasses.Count; count++)
            {
                if (count > 0)
                    temp_data = temp_data + "," + m_baseClasses[count];
                else
                    temp_data = temp_data + m_baseClasses[count];
            }
            File.WriteAllText(Application.dataPath + m_path, temp_data);
        }

        static string[] m_popupArrayContents;

        private void OnGUI()
        {
            SetupStyle();
            GUILayout.BeginVertical(styleHelpboxInner);
            // EditorGUILayout.Space();

            if (m_baseClasses.Count > 0)
            {
                GUILayout.BeginVertical(styleHelpboxInner);
                GUILayout.BeginHorizontal(styleHelpboxInner);
                m_baseClassName = EditorGUILayout.TextField("New Base Class Name", m_baseClassName, GUILayout.MinWidth(300));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create", GUILayout.MaxWidth(100)))
                {
                    if (!m_baseClasses.Contains(m_baseClassName))
                    {
                        m_baseClasses.Add(m_baseClassName);
                        m_popupArrayContents = m_baseClasses.ToArray();
                        SaveBaseClasses();
                    }
                    CustomMono.CreateScriptWithCustomBase(m_baseClassName);
                    window.Close();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(styleHelpboxInner);
                GUILayout.Label("Select Base Class");
                m_index = EditorGUILayout.Popup("", m_index, m_popupArrayContents, GUILayout.Width(200));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create", GUILayout.MaxWidth(100)))
                {
                    CustomMono.CreateScriptWithCustomBase(m_popupArrayContents[m_index]);
                    window.Close();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginHorizontal(styleHelpboxInner);
                m_baseClassName = EditorGUILayout.TextField("New Base Class Name", m_baseClassName, GUILayout.MinWidth(300));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create", GUILayout.MaxWidth(100)))
                {
                    if (!m_baseClasses.Contains(m_baseClassName))
                    {
                        m_baseClasses.Add(m_baseClassName);
                        m_popupArrayContents = m_baseClasses.ToArray();
                        SaveBaseClasses();
                    }
                    CustomMono.CreateScriptWithCustomBase(m_baseClassName);
                    window.Close();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}