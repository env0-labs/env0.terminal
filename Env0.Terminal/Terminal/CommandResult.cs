namespace Env0.Terminal.Terminal
{
    public class CommandResult
    {
        public string Output { get; set; }
        public bool IsError { get; set; }
        public bool RequiresPaging { get; set; }
        public bool StateChanged { get; set; }
        public SessionState UpdatedSession { get; set; }

        public CommandResult(
            string output,
            bool isError = false,
            bool requiresPaging = false,
            bool stateChanged = false,
            SessionState updatedSession = null)
        {
            Output = output;
            IsError = isError;
            RequiresPaging = requiresPaging;
            StateChanged = stateChanged;
            UpdatedSession = updatedSession;
        }
    }
}
