using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

namespace Prashant
{
    [System.Serializable]
    public class MethodCollection
    {
        public List<MethodClass> methods;
        public MethodCollection()
        {
            methods = new List<MethodClass>();
        }

        public void Add(string methodName, bool status = true)
        {
            methods.Add(new MethodClass(methodName, status));
        }

        public void Remove(MethodClass tempClass)
        {
            methods.Remove(tempClass);
        }
    }

    [System.Serializable]
    public class MethodClass
    {
        public string methodName;
        public bool shouldAdd = true;

        public MethodClass(string m_methodName, bool m_shouldAdd)
        {
            methodName = m_methodName;
            m_shouldAdd = shouldAdd;
        }
    }
    public class CustomMonoEditor : EditorWindow
    {
        [MenuItem("[Master_Tools]/CustomMono")]
        private static void ShowWindow()
        {
            LoadSettings();
            EditorWindow window = (CustomMonoEditor)EditorWindow.GetWindow(typeof(CustomMonoEditor), true, "Mono Settings");
            window.minSize = new Vector2(600, 450);
            window.Show();
        }

        private void Awake()
        {
            LoadAllMethodNames();
        }

        static void LoadSettings()
        {
            customNameSpaceName = EditorPrefs.GetString("MyNamespace", "GameName");
        }
        static GUIStyle styleHelpboxInner;
        static GUIStyle titleLabel, normalLabel, subtitleLabel;
        MethodCollection m_methodNames = new MethodCollection();

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
        const string methodsKey = "MethodsList";
        static string customNameSpaceName = "GameName";
        // string scriptTemplatePath = "/prashantsingh/CustomMono/MyBehaviourTemplate.cs.txt";
        private void OnGUI()
        {
            SetupStyle();
            GUILayout.BeginVertical(styleHelpboxInner);

            customNameSpaceName = EditorGUILayout.TextField("Namespace", customNameSpaceName);
            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            for (int count = 0; count < m_methodNames.methods.Count; count++)
            {
                GUILayout.BeginHorizontal(styleHelpboxInner);
                m_methodNames.methods[count].methodName = EditorGUILayout.TextField(m_methodNames.methods[count].methodName);
                m_methodNames.methods[count].shouldAdd = EditorGUILayout.Toggle(m_methodNames.methods[count].shouldAdd);
                if (GUILayout.Button("+", GUILayout.MaxWidth(25)))
                {
                    m_methodNames.Add("MethodName");
                }
                if (GUILayout.Button("-", GUILayout.MaxWidth(25)))
                {
                    m_methodNames.Remove(m_methodNames.methods[count]);
                }
                GUILayout.EndHorizontal();
            }
            // GUILayout.BeginHorizontal(styleHelpboxInner);

            GUILayout.EndVertical();
            if (GUILayout.Button("Save"))
            {
                SaveEverything();
            }
            if (GUILayout.Button("Reset"))
            {
                EditorPrefs.DeleteKey(methodsKey);
            }
            GUILayout.EndVertical();
        }

        void SaveEverything()
        {
            EditorPrefs.SetString("MyNamespace", customNameSpaceName);
            SaveAllMethodNames();
        }


        void SaveAllMethodNames()
        {
            var m_data = JsonUtility.ToJson(m_methodNames);
            EditorPrefs.SetString(methodsKey, m_data);
            string m_methodsList = "";
            List<MethodClass> m_tempMethodsList = new List<MethodClass>(m_methodNames.methods);
            for (int count = 0; count < m_tempMethodsList.Count; count++)
            {
                if (!m_tempMethodsList[count].shouldAdd) m_tempMethodsList.RemoveAt(count);
            }
            for (int count = 0; count < m_tempMethodsList.Count; count++)
            {
                if (count > 0)
                    m_methodsList = m_methodsList + "," + m_tempMethodsList[count].methodName;
                else
                    m_methodsList = m_methodsList + m_tempMethodsList[count].methodName;
            }
            File.WriteAllText(Application.dataPath + "/prashantsingh/CustomMono/FinalMethodsList.txt", m_methodsList);
        }

        void LoadAllMethodNames()
        {
            m_methodNames = new MethodCollection();
            if (!EditorPrefs.HasKey(methodsKey))
            {
                m_methodNames.Add("Awake");
                m_methodNames.Add("Start");
                m_methodNames.Add("Update");
                SaveAllMethodNames();
            }
            else
            {
                m_methodNames = JsonUtility.FromJson<MethodCollection>(EditorPrefs.GetString(methodsKey));
            }

        }
    }
}

