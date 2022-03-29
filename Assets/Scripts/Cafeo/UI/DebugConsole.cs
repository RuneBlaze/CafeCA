using System;
using System.Collections.Generic;
using Cafeo.Data;
using Cafeo.Templates;
using Cafeo.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Cafeo.UI
{
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField] private TMP_InputField commandText;
        private Dictionary<string, (ArgType[], Action<object[]>)> actions;

        private enum ArgType
        {
            Int,
            Real,
            String,
        }

        public RogueManager Scene => RogueManager.Instance;
        public TemplateFinder Db => TemplateFinder.Instance;

        private void Awake()
        {
            Assert.IsNotNull(commandText);
            actions = new Dictionary<string, (ArgType[], Action<object[]>)>();
        }

        private void Start()
        {
            RegisterCommand("spawnD", new [] { ArgType.String }, objects =>
            {
                var id = objects[0] as string;
                Scene.SpawnDroppable((Vector2) Scene.player.transform.position 
                                     + VectorUtils.OnUnitCircle(Random.Range(0, 4 * Mathf.PI)) * 2, 
                    Db.Generate<Charm>(id));
            });
        }

        private void RegisterCommand(string command, ArgType[] argTypes, Action<object[]> action)
        {
            actions.Add(command, (argTypes, action));
        }

        private void OnText(string text)
        {
            var tokens = text.Replace("\u200B","").Split(' ');
            string command = tokens[0];
            var argTypes = actions[command].Item1;
            var proc = actions[command].Item2;
            List<object> args = new List<object>(argTypes.Length);
            for (int i = 1; i < tokens.Length; i++)
            {
                args.Add(argTypes[i-1] switch
                {
                    ArgType.Int => int.Parse(tokens[i]),
                    ArgType.Real => float.Parse(tokens[i]),
                    ArgType.String => tokens[i],
                    _ => throw new ArgumentException("Invalid argument type"),
                });
            }
            proc(args.ToArray());
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && Scene.inputLocked)
            {
                var rawCommand = commandText.text;
                OnText(rawCommand);
                Debug.Log("Command: " + rawCommand);
                commandText.text = "";
            }
        }
    }
}