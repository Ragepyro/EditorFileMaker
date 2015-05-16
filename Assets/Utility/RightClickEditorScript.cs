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
 * Originally written by Milo Keeble, source can be found on Github @ (Git link here when it's up)*/

public class RightClickEditorScript
{
    [MenuItem("Assets/Create/Make Editor Script %#e", false, 82)]
    static void CreateEditorScript(MenuCommand menuCommand)
    {
        //Making sure someone is using this right
        if(Selection.activeObject == null)
            return;

        Object selectedObj = Selection.activeObject;
        TextAsset template = new TextAsset();
        string ending = ".FAIL";

        //Determines whether the selected file is C# or Javascript and loads up the right template
        if (AssetDatabase.GetAssetPath(selectedObj).EndsWith(".cs"))
        {
            template = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorTemplateCS.txt", typeof(TextAsset)) as TextAsset;
            ending = ".cs";
        }
        else if (AssetDatabase.GetAssetPath(selectedObj).EndsWith(".js"))
        {
            template = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorTemplateJS.txt", typeof(TextAsset)) as TextAsset;
            ending = ".js";
        }
        else
        {
            Debug.Log("File should be a code file. Aborted creation.");
            return;
        }
        string templateText = "";

        //Switching out all the placeholder stuff in the template
        if (template != null)
        {
            templateText = template.text;
            templateText = templateText.Replace("@ORIGINALSCRIPTNAME@", selectedObj.name);
            templateText = templateText.Replace("@SCRIPTNAME@", selectedObj.name + "Editor");
        }
        //Make sure Editor folder exists, then write the file into it
        Directory.CreateDirectory(Application.dataPath + "/Editor");
        string datapath = Application.dataPath + "/Editor/" + selectedObj.name + "Editor" + ending;
        string filepath = "Assets/Editor/" + selectedObj.name + "Editor" + ending;

        StreamWriter writer = new StreamWriter(datapath);
        writer.Write(templateText);
        writer.Close();

        //Cleanup
        AssetDatabase.ImportAsset(filepath);
        AssetDatabase.Refresh();

        //Select new object
        Object createdObj = AssetDatabase.LoadAssetAtPath(filepath, typeof(Object)) as Object;
        Selection.activeObject = createdObj;
    }
}
