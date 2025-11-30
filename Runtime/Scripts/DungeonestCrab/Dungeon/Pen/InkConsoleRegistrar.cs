using Ink.Runtime;
using KH.Console;
using KH.Script;
using System.Linq;
using UnityEngine;

namespace DungeonestCrab.Dungeon.Pen {
    public class InkConsoleRegistrar : MonoBehaviour {
        private void Start() {
            var instance = ConsoleManager.INSTANCE;
            if (instance == null) {
                Debug.LogWarning($"Console does not exist. Cannot register.");
                return;
            }
            var ink = InkStateManager.INSTANCE;
            if (ink == null) {
                Debug.LogWarning($"InkStateManager does not exist. Cannot register.");
            }

            instance.RegisterHandler(new Command() {
                Name = "ink_get",
                Description = "Get the value of an ink variable.",
                RunCallback = (cmd) => {
                    string var = ConsoleManager.ExpectString(cmd, 0);
                    return PrintInkValue(null, var);
                },
                Autocomplete = (cmd) => {
                    if (cmd.Length == 2) {
                        return ink.Manager.AllVariables;
                    }
                    return null;
                }
            });

            instance.RegisterHandler(new Command() {
                Name = "ink_setString",
                Description = "Sets the ink variable to the given string.",
                RunCallback = (cmd) => {
                    string var = ConsoleManager.ExpectString(cmd, 0);
                    var val = ConsoleManager.ExpectString(cmd, 1);
                    string output = PrintInkValue("Old Val: ", var) + '\n';
                    ink.Manager.SetStringVariable(var, val);
                    return output + PrintInkValue("New Val: ", var);
                },
                Autocomplete = (cmd) => {
                    if (cmd.Length == 2) {
                        return ink.Manager.AllVariables.Where(x => ink.Manager.GetVariable(x) is StringValue);
                    }
                    return null;
                }
            });

            instance.RegisterHandler(new Command() {
                Name = "ink_setFloat",
                Description = "Sets the ink variable to the given float.",
                RunCallback = (cmd) => {
                    string var = ConsoleManager.ExpectString(cmd, 0);
                    var val = ConsoleManager.ExpectFloat(cmd, 1);
                    string output = PrintInkValue("Old Val: ", var) + '\n';
                    ink.Manager.SetFloatVariable(var, val);
                    return output + PrintInkValue("New Val: ", var);
                },
                Autocomplete = (cmd) => {
                    if (cmd.Length == 2) {
                        return ink.Manager.AllVariables.Where(x => ink.Manager.GetVariable(x) is FloatValue);
                    }
                    return null;
                }
            });

            instance.RegisterHandler(new Command() {
                Name = "ink_setInt",
                Description = "Sets the ink variable to the given integer.",
                RunCallback = (cmd) => {
                    string var = ConsoleManager.ExpectString(cmd, 0);
                    var val = ConsoleManager.ExpectInt(cmd, 1);
                    string output = PrintInkValue("Old Val: ", var) + '\n';
                    ink.Manager.SetIntVariable(var, val);
                    return output + PrintInkValue("New Val: ", var);
                },
                Autocomplete = (cmd) => {
                    if (cmd.Length == 2) {
                        return ink.Manager.AllVariables.Where(x => ink.Manager.GetVariable(x) is IntValue);
                    }
                    return null;
                }
            });
        }

        private string PrintInkValue(string prefix, string name) {
            var ink = InkStateManager.INSTANCE;
            var val = ink.Manager.GetVariable(name);
            if (prefix == null) prefix = "";
            if (val == null) {
                return $"{prefix}'{name}' does not exist.";
            }
            return $"{prefix}{name} (Type: {val.GetType().Name}) {val}";
        }
    }
}