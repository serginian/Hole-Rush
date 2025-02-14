using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Settings
{
    public static class GlobalVariables
    {
        private static GlobalVariablesStorage _storage;

        private static GlobalVariablesStorage Storage
        { 
            get
            {
                if (_storage == null)
                    _storage = Addressables.LoadAssetAsync<GlobalVariablesStorage>("Game Parameters").WaitForCompletion();

                return _storage;
            }
        }

        public static string GetString(string name)
        {
            var variable = Storage.GetVariable(name);
            return variable?.type == GlobalVariableType.String ? variable.stringValue : null;
        }

        public static int GetInt(string name)
        {
            var variable = Storage.GetVariable(name);
            return variable?.type == GlobalVariableType.Integer ? variable.intValue : 0;
        }

        public static float GetFloat(string name)
        {
            var variable = Storage.GetVariable(name);
            return variable?.type == GlobalVariableType.Float ? variable.floatValue : 0f;
        }

        public static bool GetBool(string name)
        {
            var variable = Storage.GetVariable(name);
            return variable?.type == GlobalVariableType.Boolean ? variable.boolValue : false;
        }

        public static void SetString(string name, string value)
        {
            var variable = Storage.GetVariable(name);
            if (variable != null && variable.type == GlobalVariableType.String)
            {
                variable.stringValue = value;
            }
        }

        public static void SetInt(string name, int value)
        {
            var variable = Storage.GetVariable(name);
            if (variable != null && variable.type == GlobalVariableType.Integer)
            {
                variable.intValue = value;
            }
        }

        public static void SetFloat(string name, float value)
        {
            var variable = Storage.GetVariable(name);
            if (variable != null && variable.type == GlobalVariableType.Float)
            {
                variable.floatValue = value;
            }
        }

        public static void SetBool(string name, bool value)
        {
            var variable = Storage.GetVariable(name);
            if (variable != null && variable.type == GlobalVariableType.Boolean)
            {
                variable.boolValue = value;
            }
        }

        public static void AddVariable(string name, GlobalVariableType type)
        {
            Storage.AddVariable(name, type);
        }
        
        
    } // end of class
}