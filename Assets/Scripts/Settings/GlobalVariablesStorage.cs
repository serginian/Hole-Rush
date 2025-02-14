using UnityEngine;
using System.Collections.Generic;

namespace Settings
{
    public class GlobalVariablesStorage : ScriptableObject
    {
        public List<GlobalVariable> variables = new List<GlobalVariable>();

        public GlobalVariable GetVariable(string name)
        {
            return variables.Find(v => v.name == name);
        }

        public void AddVariable(string name, GlobalVariableType type)
        {
            if (GetVariable(name) == null)
            {
                variables.Add(new GlobalVariable {name = name, type = type});
            }
        }
        
    } // end of class
}