using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Text.RegularExpressions;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.Experimental;
namespace Prashant
{

    public static class CustomMono
    {

        [MenuItem("Assets/Create/Scripts/C# Script", false, 0)]
        public static void CreateScript()
        {
            var templatePath = Path.Combine(Application.dataPath + "/prashantsingh/CustomMono/", "MyBehaviourTemplate.cs.txt");
            // CreateNewCSScriptWithTemplate("NewFileName", templatePath);
            var endNameEditAction = ScriptableObject.CreateInstance<DoCreateAssetWithContent>();
            endNameEditAction.filecontent = templatePath;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endNameEditAction, "NewFileName", EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D, null);
        }

        public static void CreateScriptWithCustomBase(string _baseName)
        {
            var templatePath = Path.Combine(Application.dataPath + "/prashantsingh/CustomMono/", "MyBehaviourTemplate.cs.txt");
            var endNameEditAction = ScriptableObject.CreateInstance<DoCreateAssetWithContent>();
            endNameEditAction.filecontent = templatePath;
            endNameEditAction.baseClassName = _baseName;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endNameEditAction, "NewFileName", EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D, null);
        }

        public static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile, string baseClassName = "")
        {
            string content = File.ReadAllText(resourceFile);

            // #NOTRIM# is a special marker that is used to mark the end of a line where we want to leave whitespace. prevent editors auto-stripping it by accident.
            content = content.Replace("#NOTRIM#", "");

            // macro replacement
            string baseFile = Path.GetFileNameWithoutExtension(pathName);
            //Set the namespace now
            content = content.Replace("#NAMESPACENAME#", EditorPrefs.GetString("MyNamespace", "GameName"));
            content = content.Replace("#NAME#", baseFile);
            var m_baseClassName = string.IsNullOrEmpty(baseClassName) ? "MonoBehaviour" : baseClassName;
            content = content.Replace("#BASECLASS#", m_baseClassName);
            content = InsertMethods(content);
            string baseFileNoSpaces = baseFile.Replace(" ", "");
            content = content.Replace("#SCRIPTNAME#", baseFileNoSpaces);

            // if the script name begins with an uppercase character we support a lowercase substitution variant
            if (char.IsUpper(baseFileNoSpaces, 0))
            {
                baseFileNoSpaces = char.ToLower(baseFileNoSpaces[0]) + baseFileNoSpaces.Substring(1);
                content = content.Replace("#SCRIPTNAME_LOWER#", baseFileNoSpaces);
            }
            else
            {
                // still allow the variant, but change the first character to upper and prefix with "my"
                baseFileNoSpaces = "my" + char.ToUpper(baseFileNoSpaces[0]) + baseFileNoSpaces.Substring(1);
                content = content.Replace("#SCRIPTNAME_LOWER#", baseFileNoSpaces);
            }

            return CreateScriptAssetWithContent(pathName, content);
        }

        static string InsertMethods(string content)
        {
            // MethodCollection m_col = new MethodCollection();
            string[] m_methodsList = File.ReadAllText(Application.dataPath + "/prashantsingh/CustomMono/FinalMethodsList.txt").Split(',');
            string completeMethods = "";
            for (int count = 0; count < m_methodsList.Length; count++)
            {
                completeMethods = completeMethods +
                "\n\t\tvoid " + m_methodsList[count] + " ()\n" + "\t\t{" +
                    "\n\t\t\t//do something here\n\t\t}\n";
            }
            // var completeMethods = "";
            content = content.Replace("#METHODS#", completeMethods);
            return content;
        }

        public static Object CreateScriptAssetWithContent(string pathName, string templateContent)
        {
            templateContent = SetLineEndings(templateContent, EditorSettings.lineEndingsForNewScripts);

            string fullPath = Path.GetFullPath(pathName);

            // utf8-bom encoding was added for case 510374 in 2012. i think this was the wrong solution. BOM's are
            // problematic for diff tools, naive readers and writers (of which we have many!), and generally not
            // something most people think about. you wouldn't believe how many unity source files have BOM's embedded
            // in the middle of them for no reason. copy paste problem? bad tool? unity should instead have been fixed
            // to read all files that have no BOM as utf8 by default, and then we just strip them all, always, from
            // files we control. perhaps we'll do this one day and this next line can be removed. -scobi
            var encoding = new System.Text.UTF8Encoding(/*encoderShouldEmitUTF8Identifier:*/ true);

            File.WriteAllText(fullPath + ".cs", templateContent, encoding);
            try
            {
                AssetDatabase.ImportAsset(pathName + ".cs");
                AssetDatabase.Refresh();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            // Import the asset
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }
        internal static string SetLineEndings(string content, LineEndingsMode lineEndingsMode)
        {
            const string windowsLineEndings = "\r\n";
            const string unixLineEndings = "\n";

            string preferredLineEndings;

            switch (lineEndingsMode)
            {
                case LineEndingsMode.OSNative:
                    if (Application.platform == RuntimePlatform.WindowsEditor)
                        preferredLineEndings = windowsLineEndings;
                    else
                        preferredLineEndings = unixLineEndings;
                    break;
                case LineEndingsMode.Unix:
                    preferredLineEndings = unixLineEndings;
                    break;
                case LineEndingsMode.Windows:
                    preferredLineEndings = windowsLineEndings;
                    break;
                default:
                    preferredLineEndings = unixLineEndings;
                    break;
            }

            content = Regex.Replace(content, @"\r\n?|\n", preferredLineEndings);

            return content;
        }

    }

    class DoCreateAssetWithContent : EndNameEditAction
    {
        public string filecontent;
        public string baseClassName;
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Object o = CustomMono.CreateScriptAssetFromTemplate(pathName, filecontent, baseClassName);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }
    }
}
