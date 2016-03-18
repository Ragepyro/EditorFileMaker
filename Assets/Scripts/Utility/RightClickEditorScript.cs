#region USING
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
#endregion

/* RightClickEditorScript.cs
 *
 * Adds a function to the context menu to make a custom editor script for the selected file.
 *
 * Originally written by Milo Keeble, source can be found on Github @ https://github.com/Ragepyro/EditorFileMaker */

public class RightClickEditorScript
{
	private static string templateCS = "using UnityEngine;\nusing UnityEditor;\nusing System.Collections;\n\n/* @SCRIPTNAME@.cs\n *\n * An Editor script for @ORIGINALSCRIPTNAME@\n *\n * Originally made by Milo Keeble, source can be found on Github @ https://github.com/Ragepyro/EditorFileMaker */\n \n [CustomEditor(typeof(@ORIGINALSCRIPTNAME@))]\n public class @SCRIPTNAME@ : Editor {\n \n\tpublic override void OnInspectorGUI(){\n\t\t@ORIGINALSCRIPTNAME@ myTarget = (@ORIGINALSCRIPTNAME@)target;\n\t\tDrawDefaultInspector();\n\t}\n\t\n }";
	private static string templateJS = "#pragma strict\n\n/* @SCRIPTNAME@.js\n *\n * An Editor script for @ORIGINALSCRIPTNAME@\n *\n * Originally made by Milo Keeble, source can be found on Github @ https://github.com/Ragepyro/EditorFileMaker */\n\n@CustomEditor(@ORIGINALSCRIPTNAME@)\nclass @SCRIPTNAME@ extends Editor{\n    function OnInspectorGUI(){\n\n    }\n}";

    [MenuItem("Assets/Create/Make Editor Script %#e", false, 82)]
    static void CreateEditorScript(MenuCommand menuCommand)
    {
        //Making sure someone is using this right
        if(Selection.activeObject == null)
            return;

        Object selectedObj = Selection.activeObject;
		string template = "";
        string ending = ".FAIL";

        //Determines whether the selected file is C# or Javascript and loads up the right template
        if (AssetDatabase.GetAssetPath(selectedObj).EndsWith(".cs"))
        {
			template = templateCS;
            ending = ".cs";
        }
        else if (AssetDatabase.GetAssetPath(selectedObj).EndsWith(".js"))
        {
            template = templateJS;
            ending = ".js";
        }
        else
        {
            Debug.Log("File should be a code file. Aborted creation.");
            return;
        }

        template = template.Replace("@ORIGINALSCRIPTNAME@", selectedObj.name);
        template = template.Replace("@SCRIPTNAME@", selectedObj.name + "Editor");

        //Make sure Editor folder exists, then write the file into it
        Directory.CreateDirectory(Application.dataPath + "/Editor");
        string datapath = Application.dataPath + "/Editor/" + selectedObj.name + "Editor" + ending;
        string filepath = "Assets/Editor/" + selectedObj.name + "Editor" + ending;

        StreamWriter writer = new StreamWriter(datapath);
        writer.Write(template);
        writer.Close();

        //Cleanup
        AssetDatabase.ImportAsset(filepath);
        AssetDatabase.Refresh();

        //Select new object
        Object createdObj = AssetDatabase.LoadAssetAtPath(filepath, typeof(Object)) as Object;
        Selection.activeObject = createdObj;
    }
}
