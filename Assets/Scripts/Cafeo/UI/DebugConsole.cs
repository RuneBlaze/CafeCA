using System;
using System.Collections.Generic;
using System.Linq;
using Cafeo.Data;
using Cafeo.Templates;
using Cafeo.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Cafeo.UI
{
    public class DebugConsole : MonoBehaviour
    {
        [InfoBox("Use Shift + $num to run preset commands.")]
        [SerializeField] private TMP_InputField commandText;
        private Dictionary<string, (ArgType[], Action<object[]>)> actions;

        private enum ArgType
        {
            Int,
            Real,
            String,
        }

        private bool showing;
        
        public string[] registeredCommands;

        public RogueManager Scene => RogueManager.Instance;
        public TemplateFinder Db => TemplateFinder.Instance;

        private void Awake()
        {
            Assert.IsNotNull(commandText);
            actions = new Dictionary<string, (ArgType[], Action<object[]>)>();
        }

        private void Start()
        {
            var party = AllyParty.Instance;
            RegisterCommand("spawnD", new [] { ArgType.String }, objects =>
            {
                var id = objects[0] as string;
                Scene.SpawnDroppable((Vector2) Scene.player.transform.position 
                                     + VectorUtils.OnUnitCircle(Random.Range(0, 4 * Mathf.PI)) * 2, 
                    Db.Generate<IDroppable>(id));
            });
            
            RegisterCommand("spawnGood",new [] { ArgType.String, ArgType.Int }, objects =>
            {
                var id = objects[0] as string;
                var price = (int) objects[1];
                var droppable = Scene.SpawnDroppable((Vector2) Scene.player.transform.position 
                                     + VectorUtils.OnUnitCircle(Random.Range(0, 4 * Mathf.PI)) * 2, 
                    Db.Generate<IDroppable>(id));
                droppable.AttachPrice(price);
            });
            
            RegisterCommand("killAllies", new ArgType[] {} , _ =>
            {
                foreach (var ally in Scene.Allies())
                {
                    if (ally.IsLeaderAlly) continue;
                    ally.Kill();
                }
            });
            
            RegisterCommand("gainItem", new [] {ArgType.String}, objects =>
            {
                var id = objects[0] as string;
                var itemTemplate = TemplateFinder.Instance.RetrieveTemplate<OneTimeUseTemplate>(id);
                Scene.player.TryGainOneTimeUse(itemTemplate.Generate());
            });
            
            RegisterCommand("revive", new ArgType[] {}, _ =>
            {
                foreach (var ally in Scene.Allies())
                {
                    ally.Revive();
                }
            });

            RegisterCommand("addEffect", 
                new[] { ArgType.String, ArgType.Real, ArgType.Real, ArgType.Real, ArgType.Real }, 
                objects =>
            {
                var presetType = objects[0] as string;
                var userData = new Vector4((float) objects[1], (float) objects[2], (float) objects[3], (float) objects[4]);
                var preset = PresetPassiveEffect.FromPreset(presetType, userData);
                var owner = RogueManager.Instance.player;
                var status = new StatusEffect(owner, 30)
                {
                    passiveEffect = preset,
                    displayName = $"{presetType}",
                };
                owner.AddStatus(status);
            });

            commandText.DeactivateInputField();
        }

        private void RegisterCommand(string command, ArgType[] argTypes, Action<object[]> action)
        {
            actions.Add(command, (argTypes, action));
        }

        private void OnText(string text)
        {
            if (text.Contains(";"))
            {
                // repeat command for n times
                var modifiedTokens = text.Split(";").Select(it => it.Trim()).ToArray();
                int repeats;
                bool success = int.TryParse(modifiedTokens[0], out repeats);
                Assert.IsTrue(success);
                for (int i = 0; i < repeats; i++)
                {
                    OnText(modifiedTokens[1]);
                }
                return;
            } else if (text.Contains("&"))
            {
                var modifiedTokens = text.Split("&").Select(it => it.Trim()).ToArray();
                foreach (var token in modifiedTokens)
                {
                    OnText(token);
                }
                return;
            }
            var tokens = text.Replace("\u200B","").Split(' ');
            string command = tokens[0];
            var argTypes = actions[command].Item1;
            var proc = actions[command].Item2;
            List<object> args = new List<object>(argTypes.Length);
            for (int i = 1; i < tokens.Length; i++)
            {
                args.Add(argTypes[i-1] switch
                {
                    ArgType.Int => i < tokens.Length ? int.Parse(tokens[i]) : default,
                    ArgType.Real => i < tokens.Length ? float.Parse(tokens[i]) : default,
                    ArgType.String => i < tokens.Length ? tokens[i] : default,
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
                showing = false;
            }

            for (var i = 0; i < registeredCommands.Length; i++)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    OnText(registeredCommands[i]);
                    Debug.Log("Command: " + registeredCommands[i]);
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                showing = !showing;
                if (showing)
                {
                    commandText.ActivateInputField();
                    RogueManager.Instance.OnDebugEnabled();
                }
                else
                {
                    commandText.DeactivateInputField();
                    RogueManager.Instance.OnDebugDisabled();
                }
            }
        }
    }
}