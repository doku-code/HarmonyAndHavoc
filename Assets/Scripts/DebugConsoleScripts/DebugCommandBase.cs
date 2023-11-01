namespace charles
{
    public class DebugCommandBase
    {
        private string _commandId;
        private string _commandDescription;
        private string _commandFormat;

        public string commandId { get { return _commandId; } }
        public string commandDescription { get { return _commandId; } }
        public string commandFormat { get { return _commandId; } }

        public DebugCommandBase(string id, string description, string format)
        {
            _commandId = id;
            _commandDescription = description;
            _commandFormat = format;
        }
    }
}

