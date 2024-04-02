using KH.Console;
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

            instance.RegisterHandler("iget", (cmd) => {
                string var = ConsoleManager.ExpectString(cmd, 0);
                return PrintInkValue(null, var);
            });

            instance.RegisterHandler("isetstr", (cmd) => {
                string var = ConsoleManager.ExpectString(cmd, 0);
                var val = ConsoleManager.ExpectString(cmd, 1);
                string output = PrintInkValue("Old Val: ", var) + '\n';
                ink.Manager.SetStringVariable(var, val);
                return output + PrintInkValue("New Val: ", var);
            });

            instance.RegisterHandler("isetfloat", (cmd) => {
                string var = ConsoleManager.ExpectString(cmd, 0);
                var val = ConsoleManager.ExpectFloat(cmd, 1);
                string output = PrintInkValue("Old Val: ", var) + '\n';
                ink.Manager.SetFloatVariable(var, val);
                return output + PrintInkValue("New Val: ", var);
            });

            instance.RegisterHandler("isetint", (cmd) => {
                string var = ConsoleManager.ExpectString(cmd, 0);
                var val = ConsoleManager.ExpectInt(cmd, 1);
                string output = PrintInkValue("Old Val: ", var) + '\n';
                ink.Manager.SetIntVariable(var, val);
                return output + PrintInkValue("New Val: ", var);
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