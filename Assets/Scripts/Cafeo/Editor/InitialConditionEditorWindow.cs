using Cafeo.Configs;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Cafeo.Editor
{
    public class InitialConditionEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Cafeo/Initial Config")]
        public static void OpenWindow()
        {
            var window = GetWindow<InitialConditionEditorWindow>();
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.Add("Initial Party", InitialParty.Instance);
            // tree.AddAllAssetsAtPath("", "Configs", typeof(InitialParty), true);
            return tree;
        }
    }
}