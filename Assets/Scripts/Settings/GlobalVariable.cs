namespace Settings
{
    public enum GlobalVariableType
    {
        String,
        Integer,
        Float,
        Boolean
    }

    [System.Serializable]
    public class GlobalVariable
    {
        public string name;
        public GlobalVariableType type;
        public string stringValue;
        public int intValue;
        public float floatValue;
        public bool boolValue;
    }
}