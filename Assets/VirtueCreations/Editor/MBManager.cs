using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VIRTUE {
    [Serializable]
    public class SearchableType {
        [DisplayAsString]
        public Type tp;
        
        [Searchable]
        [InlineEditor (Expanded = false)]
        [ListDrawerSettings (IsReadOnly = true, Expanded = true, NumberOfItemsPerPage = 999)]
        public Object[] objects;
    }

    public class MBManager : OdinEditorWindow {
        static readonly Type[] TypesToDisplay = TypeCache.GetTypesWithAttribute<ManageableAttribute> ().OrderBy (m => m.Name).ToArray ();

        Color _col;
        Type _selectedType;
        List<bool> _isExpanded;

        [SerializeField]
        SearchableType searchableType;

        [MenuItem ("Developer/MB Manager")]
        static void OpenEditor () => GetWindow<MBManager> ();

        protected override void OnGUI () {
            if (GUIUtils.SelectButtonList (ref _selectedType, TypesToDisplay)) {
                searchableType = new SearchableType {
                    tp = _selectedType,
                    objects = FindObjectsOfType (_selectedType, true).OrderBy (x => x.name).ToArray ()
                };
            }
            base.OnGUI ();
        }
    }
}