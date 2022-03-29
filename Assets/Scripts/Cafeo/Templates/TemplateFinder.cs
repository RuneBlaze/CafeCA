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

        private void Register<T>(T template) where T : WithDisplayName, ITemplate<object>
        {
            if (template.id != null)
            {
                templates[template.id.Trim()] = template;
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
        }
    }
}