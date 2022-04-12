using System.Linq;
using Cafeo.Templates;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Cafeo.Editor
{
    public class DefinitionsEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Cafeo/Definitions Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<DefinitionsEditorWindow>();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true)
            {
                DefaultMenuStyle =
                {
                    IconSize = 24,
                    IconPadding = 2
                },
                Config =
                {
                    DrawSearchToolbar = true
                }
            };
            tree.AddAllAssetsAtPath("", "Resources/Templates", typeof(WithDisplayName), true);
            tree.EnumerateTree().AddIcons<WithIcon>(x => x.icon);
            return tree;
        }

        protected void AddCreateButtonToToolbar<T>(string buttonLabelSuffix, string directoryPath) where T : WithDisplayName
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("+" + buttonLabelSuffix)))
            {
                var skillTemplatePath = $"Assets/Resources/Templates/{directoryPath}";
                ScriptableObjectCreator.ShowDialog<T>(skillTemplatePath, obj =>
                {
                    obj.displayName = obj.name;
                    TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                });
            }
        }

        protected override void OnBeginDrawEditors()
        {
            base.OnBeginDrawEditors();
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }
                
                AddCreateButtonToToolbar<SkillTemplate>("Skill", "Skills");
                AddCreateButtonToToolbar<SkillTreeTemplate>("Skill Tree", "SkillTrees");
                AddCreateButtonToToolbar<WearableTemplate>("Wearable", "Wearables");
                AddCreateButtonToToolbar<JobTemplate>("Job", "Jobs");
                AddCreateButtonToToolbar<WeaponTypeTemplate>("Weapon Type", "WeaponTypes");
                AddCreateButtonToToolbar<TreasureTemplate>("Treasure", "Treasures");
                AddCreateButtonToToolbar<CharmTemplate>("Charm", "Charms");
                AddCreateButtonToToolbar<OneTimeUseTemplate>("OneTimeUse", "OneTimes");
                AddCreateButtonToToolbar<ProjectileTypeTemplate>("ProjectileType", "ProjectileTypes");
                AddCreateButtonToToolbar<SoulTemplate>("Soul", "Souls");
                AddCreateButtonToToolbar<FashionShopTemplate>("FashionShop", "FashionShops");
                AddCreateButtonToToolbar<EnemyTemplate>("Enemy", "Enemies");
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}