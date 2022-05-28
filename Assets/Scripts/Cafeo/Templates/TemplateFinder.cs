using System.Collections.Generic;
using Cafeo.Utils;
using UnityEditor;
using UnityEngine;

namespace Cafeo.Templates
{
    public class TemplateFinder : Singleton<TemplateFinder>
    {
        public static IEnumerable<T> AllTemplatesOfType<T>() where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<T>(path);
                yield return template;
            }
        }

        private Dictionary<string, ITemplate<object>> templates;

        public T Generate<T>(string id) where T: class
        {
            return (templates[id.Trim()]).Generate() as T;
        }

        public T RetrieveTemplate<T>(string id) where T : class
        {
            return templates[id.Trim()] as T;
        }

        private void Register<T>(T template) where T : WithDisplayName, ITemplate<object>
        {
            if (template.id != null)
            {
                templates[template.id.Trim()] = template;
                Debug.Log($"Registered {template.GetType().Name} with id {template.id}");
            }
        }
        
        protected override void Setup()
        {
            base.Setup();
            templates = new Dictionary<string, ITemplate<object>>();
            foreach (var charmTemplate in AllTemplatesOfType<CharmTemplate>())
            {
                Register(charmTemplate);
            }

            foreach (var treasureTemplate in AllTemplatesOfType<TreasureTemplate>())
            {
                Register(treasureTemplate);
            }
            
            foreach (var soulTemplate in AllTemplatesOfType<SoulTemplate>())
            {
                Register(soulTemplate);
            }
            
            foreach (var oneTimeUseTemplate in AllTemplatesOfType<OneTimeUseTemplate>())
            {
                Register(oneTimeUseTemplate);
            }
            
            foreach (var skillTemplate in AllTemplatesOfType<SkillTemplate>())
            {
                Register(skillTemplate);
            }
            
            
        }
    }
}