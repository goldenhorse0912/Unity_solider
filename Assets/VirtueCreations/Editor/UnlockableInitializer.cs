using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace VIRTUE {
    public class UnlockableInitializer : OdinEditorWindow {
        [MenuItem ("Developer/Unlockable Setter/Open")]
        static void Open () {
            GetWindow<UnlockableInitializer> ();
        }

        public TextAsset asset;
        /*[ListDrawerSettings (OnTitleBarGUI = "FindUnlockables")]
        public Unlockable[] allUnlockables;*/

        /*void FindUnlockables () {
            if (SirenixEditorGUI.ToolbarButton (EditorIcons.MagnifyingGlass)) {
                allUnlockables = FindObjectsOfType<Unlockable> ().OrderBy (x => x.name).ToArray ();
            }
        }

        [Button]
        void SetData () {
            JSONNode jsonNode = JSON.Parse (asset.text);
            JSONArray jsonArray = jsonNode.AsArray;
            for (var i = 0; i < allUnlockables.Length; i++) {
                string fileName = jsonArray[i]["FileName"];
                string unlockableTitle = jsonArray[i]["Title"];
                List<string> resourceTypes = new();
                foreach (var node in jsonArray[i]["ResourceType"].Values) {
                    resourceTypes.Add (node);
                }
                List<int> prices = new();
                foreach (var node in jsonArray[i]["Price"].Values) {
                    prices.Add (node);
                }
                allUnlockables[i].Initialize (fileName, unlockableTitle, resourceTypes.ToArray (), prices.ToArray ());
            }
        }*/
    }
}