namespace Env0.Terminal.Terminal
{
    public enum OutputType
    {
        Standard,           // Normal command output
        Error,              // Errors or failed commands
        System,             // System messages (status, transitions)
        Boot,               // Boot sequence lines
        PromptInstruction,  // Pre-prompt guidance
        MOTD,               // SSH login banner
        Debug,              // Dev-only output
        Corrupted           // Glitched/corrupted narrative lines
    }
}