using System.Collections;
using System.Collections.Generic;
using HotTotemAssets.EpicMenu;
using UnityEditor;
using UnityEngine;
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT MODIFY THE NAMING OF THE CLASS OR THE ACTION BELOW
//DO NOT REMOVE THE FOLLOWING COMMENT
//DISPLAYNAME:Remove Duplicate Index from Selection
//DO NOT REMOVE THE PREVIOUS COMMENT
namespace HotTotemAssets.EpicMenu
{
    public class RemoveDuplicateFromSelection : EpicMenuAction
    {
        //Use this Action to write your Custom Code inside
        //The passed argument is the center point of where the
        //EpicMenu pops up (the Mouseposition at the moment of pressing the shortcut)
        //You can use this to make interactions e.g. with the underlying SceneView
        public override void Action(Ray sceneViewToEpicMenuCenterRay)
        {
            var objects = Selection.gameObjects;
            foreach (var selectedObject in objects)
            {
                string objName = selectedObject.name;
                char lastChar = objName[objName.Length - 1];
                int indexOfAddedString = objName.LastIndexOf(" (");
                int uniqueNumberLength = objName.Length - (indexOfAddedString + 3);
                string uniqueNumberString = objName.Substring(indexOfAddedString + 2, uniqueNumberLength);
                int uniqueNumber;
                if (lastChar == ')' && indexOfAddedString != -1 && int.TryParse(uniqueNumberString, out uniqueNumber))
                {
                    Undo.RecordObject(selectedObject, "Remove Auto-Name");
                    selectedObject.name = objName.Substring(0, indexOfAddedString);
                }
            }
        }
    }
}